using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace InfiniteGallery.Presentation.Base.Contracts
{
    public interface IBindable : INotifyPropertyChanged
    {
        void RaisePropertyChanged([CallerMemberName] string propertyName = "");
    }
}