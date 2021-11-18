using System;
using System.Collections;
using System.Windows.Input;
using InfiniteGallery.Presentation.Controls;
using Xamarin.Forms;

namespace InfiniteGallery.Controls
{
    public class ExtendedListView : ListView, IDisposable
    {
        public static readonly BindableProperty ItemSelectedCommandProperty = BindableProperty.Create(
            nameof(ItemSelectedCommand),
            typeof(ICommand),
            typeof(ExtendedListView));

        public static readonly BindableProperty ItemLongClickCommandProperty = BindableProperty.Create(
            nameof(ItemLongClickCommand),
            typeof(ICommand),
            typeof(ExtendedListView));

        public static readonly BindableProperty ScrollToItemCommandProperty = BindableProperty.Create(
            nameof(ScrollToItemCommand),
            typeof(ICommand),
            typeof(ExtendedListView),
            defaultBindingMode:BindingMode.OneWayToSource);

        public static readonly BindableProperty LoadMoreCommandProperty = BindableProperty.Create(
            nameof(LoadMoreCommand),
            typeof(ICommand),
            typeof(ExtendedListView));

        public ExtendedListView() : base(ListViewCachingStrategy.RecycleElement)
        {
            ItemSelected += OnItemSelected;
            ItemAppearing += InfiniteListView_ItemAppearing;
            ScrollToItemCommand = new Command(async o =>
            {
                var options = o as ScrollOptions;
                if (options is null) return;
                await Device.InvokeOnMainThreadAsync(() =>
                {
                    ScrollTo(options.ScrollToObject, options.Position, options.Animated);
                });
            });
        }

        private void OnItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem != null)
                ItemSelectedCommand?.Execute(e.SelectedItem);
            SelectedItem = null;
        }

        public ICommand LoadMoreCommand
        {
            get => (ICommand) GetValue(LoadMoreCommandProperty);
            set => SetValue(LoadMoreCommandProperty, value);
        }

        public ICommand ItemSelectedCommand
        {
            get => (ICommand)GetValue(ItemSelectedCommandProperty);
            set => SetValue(ItemSelectedCommandProperty, value);
        }

        public ICommand ItemLongClickCommand
        {
            get => (ICommand)GetValue(ItemLongClickCommandProperty);
            set => SetValue(ItemLongClickCommandProperty, value);
        }

        public ICommand ScrollToItemCommand
        {
            get => (ICommand)GetValue(ScrollToItemCommandProperty);
            private set => SetValue(ScrollToItemCommandProperty, value);
        }

        private void InfiniteListView_ItemAppearing(object sender, ItemVisibilityEventArgs e)
        {
            if (ItemsSource is IList items && items.Count > 0 && e.Item == items[items.Count - 1])
            {
                if (LoadMoreCommand != null && LoadMoreCommand.CanExecute(null))
                    LoadMoreCommand.Execute(null);
            }
        }

        public void Dispose()
        {
            ItemSelected -= OnItemSelected;
            ItemAppearing -= InfiniteListView_ItemAppearing;
            ScrollToItemCommand = null;
        }
    }
}