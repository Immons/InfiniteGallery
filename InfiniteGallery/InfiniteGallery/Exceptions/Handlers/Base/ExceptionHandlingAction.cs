using System;
using InfiniteGallery.Exceptions.Handlers.Base.Contracts;

namespace InfiniteGallery.Exceptions.Handlers.Base
{
    public class ExceptionHandlingAction<TException> : IExceptionHandlingAction<TException> where TException : Exception
    {
        public ExceptionHandlingAction(TException exception)
        {
            Exception = exception;
        }

        public bool HandlingShouldFinish { get; set; }

        public TException Exception { get; }
    }
}