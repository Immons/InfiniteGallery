using System;
using System.Windows.Input;
using InfiniteGallery.Configuration.Ioc.Contracts;
using InfiniteGallery.Exceptions.Base.Contracts;
using InfiniteGallery.Presentation.Base.Contracts;
using Xamarin.Forms.Internals;

namespace InfiniteGallery.ViewModels.Base
{
    public interface IBaseViewModel : IBindable, IDisposable, IInjector<IExceptionGuard>
    {
        bool IsBusy { get; set; }
        string Title { get; set; }
        void OnAppearing();
        void OnDisappearing();
        ICommand CloseCommand { get; }
        object NavigationParameter { get; set; }
        void ScreenOrientationChanged(DeviceOrientation orientation);
    }

    public interface IBaseViewModel<TNavigationParameter> : IBaseViewModel
    {
	    TNavigationParameter NavigationParameter { get; }
        void Prepare(TNavigationParameter parameter);
    }
}