using Xamarin.Forms;

namespace InfiniteGallery.Presentation.Controls
{
    public class ScrollOptions
    {
        public object ScrollToObject { get; }
        public ScrollToPosition Position { get; }
        public bool Animated { get; }

        public ScrollOptions(object scrollToObject, ScrollToPosition position, bool animated)
        {
            ScrollToObject = scrollToObject;
            Position = position;
            Animated = animated;
        }
    }
}