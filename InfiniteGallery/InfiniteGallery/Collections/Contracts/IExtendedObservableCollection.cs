using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Threading.Tasks;

namespace InfiniteGallery.Collections.Contracts
{
	public interface IExtendedObservableCollection<T> : IList<T>, INotifyCollectionChanged, INotifyPropertyChanged
	{
		void AddRange(IEnumerable<T> items);
        Task AddRangeAsync(IEnumerable<T> items);
		void RemoveItems(IEnumerable<T> items);
        Task ReplaceWith(IEnumerable<T> items);
        void ReplaceItem(int index, T item);
        void ReplaceItem(T newItem, T oldItem);
		void RefreshItem(T item);
		void InsertRange(int index, IEnumerable<T> items);
        bool ShouldNotCallCollectionChanged { get; set; }
        void RefreshAll();
    }
}