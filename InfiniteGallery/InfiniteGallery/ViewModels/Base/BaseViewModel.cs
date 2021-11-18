using System.Windows.Input;
using InfiniteGallery.Configuration.Ioc.Contracts;
using InfiniteGallery.Exceptions.Base.Contracts;
using InfiniteGallery.Presentation.Base;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using InfiniteGallery.Services.Contracts;

namespace InfiniteGallery.ViewModels.Base
{
    public abstract class BaseViewModel : Bindable,
        IBaseViewModel,
        IInjector<INavigationService>,
        IDisposingView
    {
        private bool _alreadyDisappeared;
        private bool _disposing;
        private bool _isBusy;
        private object _navigationParameter;
        private string _title = string.Empty;
        private DeviceOrientation _orientation;

        protected BaseViewModel()
        {
            Connectivity.ConnectivityChanged += ConnectivityOnConnectivityChanged;
        }

        private void ConnectivityOnConnectivityChanged(object sender, ConnectivityChangedEventArgs e)
        {
            RaisePropertyChanged(nameof(HasConnectionToInternet));
        }

        public bool HasConnectionToInternet => Connectivity.NetworkAccess == NetworkAccess.Internet;

        public virtual void ScreenOrientationChanged(DeviceOrientation orientation)
        {
            Orientation = orientation;
        }

        public DeviceOrientation Orientation
        {
            get => _orientation;
            private set => SetProperty(ref _orientation, value);
        }

        public INavigationService NavigationService { get; private set; }
        public IExceptionGuard ExceptionGuard { get; private set; }

        public ICommand CloseCommand => new Command(Close);

        public bool IsBusy
        {
            get => _isBusy;
            set => SetProperty(ref _isBusy, value);
        }

        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        public virtual void OnAppearing()
        {
            _alreadyDisappeared = false;
            RaisePropertyChanged(nameof(HasConnectionToInternet));
        }

        public virtual void OnDisappearing()
        {
            _alreadyDisappeared = true;
        }

        void IInjector<IExceptionGuard>.Inject(IExceptionGuard service)
        {
            ExceptionGuard = service;
        }

        public object NavigationParameter
        {
            get => _navigationParameter;
            set => SetProperty(ref _navigationParameter, value);
        }

        public virtual void Dispose()
        {
            _disposing = true;
            Connectivity.ConnectivityChanged -= ConnectivityOnConnectivityChanged;
        }

        bool IDisposingView.IsDisposing { get; set; }

        void IInjector<INavigationService>.Inject(INavigationService navigationService)
        {
            NavigationService = navigationService;
        }

        protected virtual async void Close()
        {
            if (_alreadyDisappeared) return;
            await NavigationService.Close();
        }

        protected void Prepare()
        {
        }
    }

    public abstract class BaseViewModel<TParameter> : BaseViewModel, IBaseViewModel<TParameter>, IPreparableViewModel
    {
        public new TParameter NavigationParameter
        {
            get => (TParameter) base.NavigationParameter;
            private set
            {
                base.NavigationParameter = value;
                RaisePropertyChanged(nameof(NavigationParameter));
            }
        }

        public virtual void Prepare(TParameter parameter)
        {
            base.Prepare();
            NavigationParameter = parameter;
        }

        void IPreparableViewModel.Prepare(object parameter)
        {
            Prepare((TParameter) parameter);
        }
    }
}