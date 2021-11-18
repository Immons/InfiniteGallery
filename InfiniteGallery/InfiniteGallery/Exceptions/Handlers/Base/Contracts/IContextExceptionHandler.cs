using System;

namespace InfiniteGallery.Exceptions.Handlers.Base.Contracts
{
    public interface IContextExceptionHandler<TException, TContext> : IExceptionHandler<TException> where TException : Exception where TContext : class
    {
        WeakReference<TContext> Context { get; }
    }
}