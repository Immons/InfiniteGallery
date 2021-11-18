using System;
using System.Threading.Tasks;
using InfiniteGallery.Helpers.Contracts;

namespace InfiniteGallery.Helpers
{
    public static class MainThread
    {
        public static IMainThreadInvoker Instance { get; set; }

        public static void BeginInvokeOnMainThread(Action action)
        {
            if (Instance != null)
                Instance.BeginInvokeOnMainThread(action);
            else
                action?.Invoke();
        }

        public static async Task InvokeOnMainThreadAsync(Action action)
        {
            if (Instance != null)
                await Instance.InvokeOnMainThreadAsync(action);
            else
                action?.Invoke();
        }
    }
}