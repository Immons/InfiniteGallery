using System;
using System.Threading.Tasks;

namespace InfiniteGallery.Helpers.Contracts
{
    public interface IMainThreadInvoker
    {
        void BeginInvokeOnMainThread(Action action);
        Task InvokeOnMainThreadAsync(Action action);
    }
}