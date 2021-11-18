using InfiniteGallery.Commands.Base.Contracts;
using InfiniteGallery.ViewModels.Contracts;

namespace InfiniteGallery.Commands.Contracts
{
    public interface ILoadPhotosCommandBuilder : IAsyncGuardedDataContextCommandBuilder<IMainViewModel>
    {
    }
}