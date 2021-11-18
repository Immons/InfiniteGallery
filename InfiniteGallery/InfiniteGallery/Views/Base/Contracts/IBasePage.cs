using System;
using InfiniteGallery.ViewModels.Base;

namespace InfiniteGallery.Views.Base.Contracts
{
    public interface IBasePage<TViewModel, TParameter> : IBasePage where TViewModel : IBaseViewModel<TParameter>
    {
        TViewModel ViewModel { get; set; }
    }

    public interface IBasePage : IDisposable
    {
        IBaseViewModel ViewModel { get; set; }

        bool IsNavigationPage { get; }

        bool IsNavigationBarVisible { get; }
    }
}