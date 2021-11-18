using InfiniteGallery.Collections.Contracts;
using InfiniteGallery.Commands.Contracts;
using InfiniteGallery.Models.Data;
using InfiniteGallery.Services.Data.Contracts;
using InfiniteGallery.ViewModels.Base;
using InfiniteGallery.ViewModels.Contracts;
using Xamarin.CommunityToolkit.ObjectModel;

namespace InfiniteGallery.ViewModels
{
    public class MainViewModel : BaseViewModel<string>, IMainViewModel
    {
        private readonly IPhotoService _photoService;

        public MainViewModel(
            ILoadPhotosCommandBuilder loadPhotosCommandBuilder,
            IPhotoService photoService)
        {
            _photoService = photoService;
            LoadPhotosCommand = loadPhotosCommandBuilder.RegisterDataContext(this).BuildCommand();
        }

        public IAsyncCommand LoadPhotosCommand { get; }

        public IExtendedObservableCollection<PhotoDTO> Photos => _photoService.Photos;

        public override async void OnAppearing()
        {
            base.OnAppearing();
            await LoadPhotosCommand.ExecuteAsync();
        }
    }
}