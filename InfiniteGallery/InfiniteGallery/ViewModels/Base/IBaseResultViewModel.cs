using System.Threading.Tasks;

namespace InfiniteGallery.ViewModels.Base
{
    public interface IBaseResultViewModel<TNavigationParameter, TResult> : IBaseViewModel<TNavigationParameter>
    {
        TaskCompletionSource<TResult> CompletionSource { get; }
    }
}