using System.Threading.Tasks;
using InfiniteGallery.Commands.Base;
using InfiniteGallery.Commands.Contracts;
using InfiniteGallery.Services.Data.Contracts;
using InfiniteGallery.ViewModels.Contracts;

namespace InfiniteGallery.Commands
{
    public class LoadPhotosCommandBuilder : AsyncGuardedDataContextCommandBuilder<IMainViewModel>, ILoadPhotosCommandBuilder
    {
        private readonly IPhotoService _photoService;

        public LoadPhotosCommandBuilder(IPhotoService photoService)
        {
            _photoService = photoService;
        }

        protected override Task ExecuteCommandAction()
        {
            return _photoService.GetImages();
        }
    }
}