using System.Diagnostics;
using Autofac;
using InfiniteGallery.Configuration.Ioc.Contracts;
using InfiniteGallery.ViewModels.Base;
using Xamarin.Forms;

namespace InfiniteGallery.Configuration
{
    public abstract class BaseFormsApp : Application
    {
	    protected readonly IContainer Container;

	    protected BaseFormsApp(IContainer container)
	    {
		    Container = container;
	    }

	    protected void ResolveAppStart<TViewModel, TParameter>(TParameter parameter = null) where TViewModel : class, IBaseViewModel<TParameter> where TParameter : class
        {
            var viewResolver = Container.Resolve<IViewAndViewModelResolver>();
            var resolved = viewResolver.ResolveViewModelAndPage<TViewModel, TParameter>();
            resolved.Item1.Prepare(parameter);

            var mainPage = resolved.Item2 as Page;

            if (resolved.Item2.IsNavigationPage)
            {
                mainPage = new NavigationPage(mainPage);
                NavigationPage.SetHasNavigationBar(mainPage, resolved.Item2.IsNavigationBarVisible);
            }

            MainPage = mainPage;
        }

        protected override void OnStart()
        {
            Debug.WriteLine("****** starting the app");
        }

    }
}