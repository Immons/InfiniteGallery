using System;
using System.Threading.Tasks;
using InfiniteGallery.Exceptions.Handlers.Base.Contracts;

namespace InfiniteGallery.Exceptions.Handlers.Base
{
    public abstract class ExceptionHandler<TException> : IExceptionHandler<TException> where TException : Exception
    {
        public abstract Task Handle(IExceptionHandlingAction<TException> exceptionHandlingAction);
        public virtual Task<bool> CanHandle(TException exception) => Task.FromResult(exception != null);
        Task IExceptionHandler.Handle(IExceptionHandlingAction exceptionHandlingAction) => Handle(exceptionHandlingAction as IExceptionHandlingAction<TException>);
        Task<bool> IExceptionHandler.CanHandle(Exception exception) => CanHandle(exception as TException);
    }
}