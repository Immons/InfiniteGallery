using System;
using System.Threading.Tasks;

namespace InfiniteGallery.Exceptions.Handlers.Base.Contracts
{
    public interface IExceptionHandler
    {
        Task Handle(IExceptionHandlingAction exception);
        Task<bool> CanHandle(Exception exception);
    }
}