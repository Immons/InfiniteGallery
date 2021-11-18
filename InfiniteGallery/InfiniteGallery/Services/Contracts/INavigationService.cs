using System;
using System.Threading.Tasks;
using InfiniteGallery.ViewModels.Base;

namespace InfiniteGallery.Services.Contracts
{
    public interface INavigationService
    {
        Task Close<TViewModel>(TViewModel viewModel) where TViewModel : IBaseViewModel;

        Task Close(bool animated = true);

        Task CloseToRoot(bool animated = true);

        Task NavigateTo<TViewModel, TParameter>(TParameter parameter = default, bool forcePush = false)
            where TViewModel : class, IBaseViewModel<TParameter> where TParameter : class;

        Task<TResult> NavigateTo<TViewModel, TParameter, TResult>(TParameter parameter = default, bool forcePush = false)
            where TViewModel : class, IBaseResultViewModel<TParameter, TResult> where TParameter : class;

        Task NavigateTo(Type viewModelType, object parameter);
    }
}