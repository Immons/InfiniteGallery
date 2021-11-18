using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using InfiniteGallery.Configuration.Ioc.Contracts;
using InfiniteGallery.Helpers.Contracts;
using InfiniteGallery.Services.Contracts;
using InfiniteGallery.ViewModels.Base;
using InfiniteGallery.ViewModels.Base.Contracts;
using InfiniteGallery.Views.Base.Contracts;
using Xamarin.Forms;
using Application = Xamarin.Forms.Application;
using NavigationPage = Xamarin.Forms.NavigationPage;
using Page = Xamarin.Forms.Page;

namespace InfiniteGallery.Services
{
    public class FormsNavigationService : INavigationService
    {
        private readonly IViewAndViewModelResolver _viewAndViewModelResolver;
        private readonly IMainThreadInvoker _mainThreadInvoker;
        private bool _navigationInProgress;

        public FormsNavigationService(
            IViewAndViewModelResolver viewAndViewModelResolver,
            IMainThreadInvoker mainThreadInvoker)
        {
            _viewAndViewModelResolver = viewAndViewModelResolver;
            _mainThreadInvoker = mainThreadInvoker;
        }

        private INavigation FormsNavigation => Application.Current.MainPage.Navigation;

        public async Task Close<TViewModel>(TViewModel viewModel) where TViewModel : IBaseViewModel
        {
            await _mainThreadInvoker.InvokeOnMainThreadAsync(async () =>
            {
                for (var i = FormsNavigation.NavigationStack.Count - 1; i >= 0; i--)
                {
                    var page = FormsNavigation.NavigationStack[i];
                    if (page is IBasePage basePage)
                    {
                        if (basePage.ViewModel is TViewModel)
                        {
                            if (i == FormsNavigation.NavigationStack.Count)
                                await FormsNavigation.PopAsync(true);
                            else
                                FormsNavigation.RemovePage(page);

                            return;
                        }
                    }
                    else if (page is NavigationPage naviPage)
                    {
                        if ((naviPage.CurrentPage.BindingContext) is TViewModel)
                        {
                            await FormsNavigation.PopAsync(true);
                            return;
                        }
                    }
                }
            });
        }

        public async Task Close(bool animated = true)
        {
            await _mainThreadInvoker.InvokeOnMainThreadAsync(async () =>
            {
                if (FormsNavigation.ModalStack.Count > 0)
                {
                    if (FormsNavigation.ModalStack.LastOrDefault() is IDisposable disposable)
                        disposable.Dispose();

                    await FormsNavigation.PopModalAsync(animated);
                    return;
                }

                if (Application.Current.MainPage is NavigationPage navigationPage)
                {
                    (navigationPage.CurrentPage as IDisposable)?.Dispose();
                    await navigationPage.PopAsync(animated);
                }
                else
                {
                    (FormsNavigation.NavigationStack.LastOrDefault() as IDisposable)?.Dispose();
                    await FormsNavigation.PopAsync(animated);
                }
            });
        }

        public async Task CloseToRoot(bool animated = true)
        {
            await _mainThreadInvoker.InvokeOnMainThreadAsync(async () =>
            {
                if (Application.Current.MainPage is NavigationPage naviPage)
                {
                    await naviPage.PopToRootAsync(animated);
                }
            });
        }

        public async Task NavigateTo<T, TParameter>(TParameter parameter, bool forcePush = false) where T : class, IBaseViewModel<TParameter> where TParameter : class
        {
            if (_navigationInProgress) return;
            _navigationInProgress = true;
            IBasePage page = null;

            try
            {
                var resolved = _viewAndViewModelResolver.ResolveViewModelAndPage<T, TParameter>();
                resolved.Item1.Prepare(parameter);
                page = resolved.Item2;
                await _mainThreadInvoker.InvokeOnMainThreadAsync(async () => await PushPage(page));
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);

                try
                {
                    await _mainThreadInvoker.InvokeOnMainThreadAsync(async () => await PushPage(page));
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e);
                    throw;
                }
            }
            finally
            {
                _navigationInProgress = false;
            }
        }

        public async Task<TResult>
            NavigateTo<TViewModel, TParameter, TResult>(TParameter parameter, bool forcePush = false) where TViewModel : class, IBaseResultViewModel<TParameter, TResult> where TParameter : class
        {
            if (_navigationInProgress) return default;
            _navigationInProgress = true;
            IBasePage page = null;

            try
            {
                var resolved = _viewAndViewModelResolver.ResolveViewModelAndPage<TViewModel, TParameter>();
                resolved.Item1.Prepare(parameter);
                page = resolved.Item2;
                await _mainThreadInvoker.InvokeOnMainThreadAsync(async () => await PushPage(page));
                return await resolved.Item1.CompletionSource.Task;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);

                try
                {
                    await _mainThreadInvoker.InvokeOnMainThreadAsync(async () => await PushPage(page));
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e);
                    throw;
                }

                return default;
            }
            finally
            {
                _navigationInProgress = false;
            }
        }

        private async Task PushPage(IBasePage page)
        {
            if (page.ViewModel is IRootViewModel)
            {
                Application.Current.MainPage = GetPageBasedOnConfiguration(page);
                return;
            }

            if (FormsNavigation.ModalStack.FirstOrDefault() is NavigationPage modalNavigationPage)
            {
                await modalNavigationPage.PushAsync(GetPageBasedOnConfiguration(page));
            }
            else if (Application.Current.MainPage is NavigationPage naviPage)
            {
                var disposingView = naviPage.CurrentPage.BindingContext ??
                                    ((NavigationPage) naviPage.CurrentPage).CurrentPage.BindingContext;

                await naviPage.PushAsync(GetPageBasedOnConfiguration(page));
                ((IDisposingView)disposingView).IsDisposing = false;
            }
            else
                await FormsNavigation.PushAsync(GetPageBasedOnConfiguration(page));
        }

        public async Task NavigateTo(Type viewModelType, object parameter)
        {
            var viewModel = _viewAndViewModelResolver.ResolveViewModel(viewModelType);
            var page = (IBasePage)_viewAndViewModelResolver.GetFormsPage(viewModelType);
            page.ViewModel = viewModel;
            ((IPreparableViewModel)viewModel).Prepare(parameter);

            await _mainThreadInvoker.InvokeOnMainThreadAsync(async () => await PushPage(page));
        }

        public static Page GetPageBasedOnConfiguration(IBasePage page)
        {
            if (page.IsNavigationPage)
            {
                var naviPage = new NavigationPage(page as Page);
                NavigationPage.SetHasNavigationBar(naviPage, page.IsNavigationBarVisible);
                return naviPage;
            }

            return page as Page;
        }
    }
}