using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using InfiniteGallery.Collections.Contracts;
using InfiniteGallery.Helpers;

namespace InfiniteGallery.Collections
{
    public class ExtendedObservableCollection<T> : ObservableCollection<T>, IExtendedObservableCollection<T>
    {
        public bool ShouldNotCallCollectionChanged { get; set; }

        public void RefreshAll()
        {
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        public void AddRange(IEnumerable<T> items)
        {
            var changeIndex = Count;
            var itemsList = items.ToList();

            if (itemsList.Count == 0)
                return;


            ShouldNotCallCollectionChanged = true;
            foreach (var item in itemsList) Add(item);

            ShouldNotCallCollectionChanged = false;

            MainThread.BeginInvokeOnMainThread(() =>
            {
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add,
                    itemsList,
                    changeIndex));
            });
        }

        public async Task AddRangeAsync(IEnumerable<T> items)
        {
            var changeIndex = Count;
            var itemsList = items.ToList();

            if (itemsList.Count == 0)
                return;


            ShouldNotCallCollectionChanged = true;
            foreach (var item in itemsList) Add(item);

            ShouldNotCallCollectionChanged = false;

            await MainThread.InvokeOnMainThreadAsync(() =>
            {
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add,
                    itemsList,
                    changeIndex));
            });
        }

        public void RemoveItems(IEnumerable<T> items)
        {
            var changeIndex = 0;
            var itemsList = items.ToList();

            MainThread.BeginInvokeOnMainThread(() =>
            {
                ShouldNotCallCollectionChanged = true;
                foreach (var item in itemsList) Remove(item);

                ShouldNotCallCollectionChanged = false;
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove,
                    itemsList, changeIndex));
            });
        }

        public async Task ReplaceWith(IEnumerable<T> items)
        {
            await MainThread.InvokeOnMainThreadAsync(() =>
            {
                ShouldNotCallCollectionChanged = true;

                Clear();
                foreach (var item in items) Add(item);

                ShouldNotCallCollectionChanged = false;
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            });
        }

        public void ReplaceItem(int index, T item)
        {
            var changeIndex = index;

            ShouldNotCallCollectionChanged = true;
            RemoveAt(index);
            Insert(index, item);

            ShouldNotCallCollectionChanged = false;
            OnCollectionChanged(
                new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, item, changeIndex));
        }

        public void RefreshItem(T item)
        {
            OnCollectionChanged(
                new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, item, item));
        }

        public void InsertRange(int index, IEnumerable<T> items)
        {
            ShouldNotCallCollectionChanged = true;
            var currentIndex = index;
            foreach (var item in items) InsertItem(currentIndex++, item);

            ShouldNotCallCollectionChanged = false;
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, items, index));
        }

        public void ReplaceItem(T newItem, T oldItem)
        {
            ShouldNotCallCollectionChanged = true;
            var index = IndexOf(oldItem);
            Remove(oldItem);
            Insert(index, newItem);

            ShouldNotCallCollectionChanged = false;
            MainThread.BeginInvokeOnMainThread(() =>
            {
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace,
                    newItem, oldItem));
            });
        }

        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (ShouldNotCallCollectionChanged)
                return;

            try
            {
                base.OnCollectionChanged(e);
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception);
            }
        }

        protected override void ClearItems()
        {
            MainThread.BeginInvokeOnMainThread(() => base.ClearItems());
        }

        public void Replace(IEnumerable<T> items)
        {
            var itemsList = items.ToList();

            ShouldNotCallCollectionChanged = true;
            Clear();
            foreach (var item in itemsList) Add(item);

            ShouldNotCallCollectionChanged = false;
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }
    }
}