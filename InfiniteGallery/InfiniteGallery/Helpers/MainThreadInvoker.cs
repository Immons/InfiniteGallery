using System;
using System.Threading.Tasks;
using InfiniteGallery.Helpers.Contracts;

namespace InfiniteGallery.Helpers
{
    public class MainThreadInvoker : IMainThreadInvoker
    {
        public void BeginInvokeOnMainThread(Action action)
        {
            Xamarin.Essentials.MainThread.BeginInvokeOnMainThread(action);
        }

        public Task InvokeOnMainThreadAsync(Action action)
        {
            return Xamarin.Essentials.MainThread.InvokeOnMainThreadAsync(action);
        }
    }
}