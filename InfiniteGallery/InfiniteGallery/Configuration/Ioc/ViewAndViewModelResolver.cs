using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Autofac;
using InfiniteGallery.Configuration.Ioc.Contracts;
using InfiniteGallery.Exceptions.Views;
using InfiniteGallery.ViewModels.Base;
using InfiniteGallery.Views.Base.Contracts;
using Xamarin.Forms;

namespace InfiniteGallery.Configuration.Ioc
{
    public class ViewAndViewModelResolver : IViewAndViewModelResolver
	{
		private readonly IComponentContext _componentContext;
		private Action _presentAction;

		public ViewAndViewModelResolver(IComponentContext componentContext)
		{
			_componentContext = componentContext;
		}

		public (TViewModel, IBasePage<TViewModel, TParameter>) ResolveViewModelAndPage<TViewModel, TParameter>() where TViewModel : class, IBaseViewModel<TParameter> where TParameter : class
		{
			var viewModel = _componentContext.Resolve<TViewModel>();
			var page = GetPageByNameContract<TViewModel, TParameter>(viewModel);
			page.ViewModel = viewModel;
			return (viewModel, page);
		}

		public IBaseViewModel ResolveViewModel(Type viewModelType)
		{
			return _componentContext.Resolve(viewModelType) as IBaseViewModel;
		}

		private IBasePage<TViewModel, TParameter> GetPageByNameContract<TViewModel, TParameter>(TViewModel viewModel) where TViewModel : class, IBaseViewModel<TParameter> where TParameter : class
		{
			try
			{
				var page = GetFormsPage(viewModel.GetType());
				return page as IBasePage<TViewModel, TParameter>;
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
				throw;
			}
		}

		public Page GetFormsPage(Type viewModelType)
		{
			var name = viewModelType.Name;
			var pageName = name.Replace("ViewModel", "Page");
			var types = GetPageTypesRegistered();
			Type pageType = null;
			try
			{
				pageType = types.FirstOrDefault(p => p.Name == pageName);
			}
			catch (Exception e)
			{
				Debug.WriteLine(e);
				throw new PageNotFoundException(pageName, e);
			}
			finally
			{
				if (pageType == null)
				{
					throw new PageNotFoundException(pageName);
				}
			}

			try
			{
				var page = _componentContext.Resolve(pageType);
				return page as Page;
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex);
				throw;
			}
		}

		private IEnumerable<Type> GetPageTypesRegistered()
		{
			var types = GetType().Assembly.GetTypes();
			var pageTypes = new List<Type>();
			foreach (var type in types)
			{
				if (type.GetInterfaces().Contains(typeof(IBasePage)))
				{
					pageTypes.Add(type);
				}
			}

			return pageTypes;
		}
	}
}