using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using InfiniteGallery.Collections;
using InfiniteGallery.Collections.Contracts;
using InfiniteGallery.Configuration.Ioc.Modules.Base;
using InfiniteGallery.Endpoints;
using InfiniteGallery.Models.Data;
using InfiniteGallery.Services.Data.Contracts;

namespace InfiniteGallery.Services.Data
{
    [SingleInstanceIoCRegistration]
    public class PhotoService : IPhotoService
    {
        public const int PhotosPullLimit = 2;

        private readonly IPhotoEndpoint _photoEndpoint;

        public PhotoService(IPhotoEndpoint photoEndpoint)
        {
            _photoEndpoint = photoEndpoint;
        }

        public async Task<List<PhotoDTO>> GetImages(int? limit = PhotosPullLimit, CancellationToken cancellationToken = default)
        {
            var result = await _photoEndpoint.GetImages(Photos.Count, limit ?? PhotosPullLimit, cancellationToken);
            await Photos.AddRangeAsync(result);
            return result;
        }

        public IExtendedObservableCollection<PhotoDTO> Photos { get; } = new ExtendedObservableCollection<PhotoDTO>();
    }
}