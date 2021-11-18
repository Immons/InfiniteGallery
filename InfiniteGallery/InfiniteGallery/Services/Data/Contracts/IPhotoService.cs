using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using InfiniteGallery.Collections.Contracts;
using InfiniteGallery.Commands;
using InfiniteGallery.Models.Data;

namespace InfiniteGallery.Services.Data.Contracts
{
    public interface IPhotoService
    {
        Task<List<PhotoDTO>> GetImages(int? limit = PhotoService.PhotosPullLimit, CancellationToken cancellationToken = default);
        IExtendedObservableCollection<PhotoDTO> Photos { get; }
    }
}