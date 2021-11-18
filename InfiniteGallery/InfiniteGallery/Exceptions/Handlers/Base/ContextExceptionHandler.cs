using System;
using InfiniteGallery.Exceptions.Handlers.Base.Contracts;

namespace InfiniteGallery.Exceptions.Handlers.Base
{
    public abstract class ContextExceptionHandler<TException, TContext> : ExceptionHandler<TException>, IContextExceptionHandler<TException, TContext> where TException : Exception where TContext : class
    {
        public WeakReference<TContext> Context { get; } = new WeakReference<TContext>(null);
    }
}