using System;
using System.Threading.Tasks;

namespace InfiniteGallery.Exceptions.Handlers.Base.Contracts
{
    public interface IExceptionHandlingAction
    {
        bool HandlingShouldFinish { get; set; }
    }

    public interface IExceptionHandlingAction<out TException> : IExceptionHandlingAction where TException : Exception
    {
        TException Exception { get; }
    }

    public interface IExceptionHandler<in TException> : IExceptionHandler where TException : Exception
    {
        Task Handle(IExceptionHandlingAction<TException> exceptionHandlingAction);
    }
}