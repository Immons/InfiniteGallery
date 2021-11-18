using System.ComponentModel;
using InfiniteGallery.ViewModels.Base;
using InfiniteGallery.Views.Base.Contracts;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;
using NavigationPage = Xamarin.Forms.NavigationPage;
using Page = Xamarin.Forms.PlatformConfiguration.iOSSpecific.Page;

namespace InfiniteGallery.Views.Base
{
    public class BasePage<TViewModel, TParameter> : ContentPage, IBasePage<TViewModel, TParameter>
        where TViewModel : IBaseViewModel<TParameter> where TParameter : class
    {
        private TViewModel _viewModel;

        protected BasePage()
        {
            NavigationPage.SetHasNavigationBar(this, IsNavigationBarVisible);
            Device.Info.PropertyChanged += InfoOnPropertyChanged;
        }

        public bool IsModal { get; private set; }

        public virtual bool HasFloatingButton => true;

        public TViewModel ViewModel
        {
            get => _viewModel;
            set
            {
                _viewModel = value;
                BindingContext = _viewModel;
                ViewModel?.ScreenOrientationChanged(Device.Info.CurrentOrientation);
            }
        }

        IBaseViewModel IBasePage.ViewModel
        {
            get => ViewModel;
            set => ViewModel = (TViewModel)value;
        }

        public virtual bool IsNavigationPage => false;

        public virtual bool IsNavigationBarVisible => false;

        public async void Dispose()
        {
            ViewModel?.Dispose();
            await MainThread.InvokeOnMainThreadAsync(() => ViewModel = default);
        }

        private void InfoOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Device.Info.CurrentOrientation))
                ScreenOrientationChanged(Device.Info.CurrentOrientation);
        }

        private void ScreenOrientationChanged(DeviceOrientation orientation)
        {
            ViewModel?.ScreenOrientationChanged(orientation);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            ViewModel?.OnAppearing();

            Page.SetUseSafeArea(On<iOS>(), false);
            if (Device.RuntimePlatform == Device.iOS)
            {
                var contentGrid = GetContentGrid();
                if (contentGrid == null) return;

                var modalStyle = Page.ModalPresentationStyle(On<iOS>());
                var pageHeight = Device.Info.ScaledScreenSize.Height;
                var pageWidth = Device.Info.ScaledScreenSize.Width;
                var topPaddingToUse = 0;

                if (IsModal && modalStyle == UIModalPresentationStyle.FormSheet)
                {
                    topPaddingToUse = 20;
                }
                else
                {
                    if (pageHeight != pageWidth)
                        topPaddingToUse = pageHeight > 670 ? 50 : 20;
                    else
                        topPaddingToUse = 50;
                }

                contentGrid.Padding = new Thickness(0, topPaddingToUse, 0, 0);
            }
            else if (Device.RuntimePlatform == Device.Android)
            {
                var contentGrid = GetContentGrid();
                if (contentGrid == null) return;

                contentGrid.Padding = new Thickness(0, 20, 0, 0);
            }
        }

        private Grid GetContentGrid()
        {
            var content = Content as Layout<View>;
            var contentGrid = content?.FindByName<Grid>("ContentGrid");
            return contentGrid;
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            ViewModel?.OnDisappearing();
        }

        protected override bool OnBackButtonPressed()
        {
            var cannotClose = base.OnBackButtonPressed();
            if (!cannotClose || IsModal)
                Dispose();
            return cannotClose;
        }
    }
}