using System;
using InfiniteGallery.ViewModels.Base;
using InfiniteGallery.Views.Base.Contracts;
using Xamarin.Forms;

namespace InfiniteGallery.Configuration.Ioc.Contracts
{
    public interface IViewAndViewModelResolver
    {
        (TViewModel, IBasePage<TViewModel, TParameter>) ResolveViewModelAndPage<TViewModel, TParameter>()
            where TViewModel : class, IBaseViewModel<TParameter> where TParameter : class;

	    IBaseViewModel ResolveViewModel(Type viewModelType);

	    Page GetFormsPage(Type viewModelType);
    }
}