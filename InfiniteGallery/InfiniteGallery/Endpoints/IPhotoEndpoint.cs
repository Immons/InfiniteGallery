using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using InfiniteGallery.Models.Data;
using Refit;

namespace InfiniteGallery.Endpoints
{
    public interface IPhotoEndpoint
    {
        [Get("/v2/list")]
        Task<List<PhotoDTO>> GetImages(int page, int limit, CancellationToken token = default);
    }
}