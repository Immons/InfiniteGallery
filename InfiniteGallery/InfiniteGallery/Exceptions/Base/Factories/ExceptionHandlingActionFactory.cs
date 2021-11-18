using System;
using InfiniteGallery.Exceptions.Base.Factories.Contracts;
using InfiniteGallery.Exceptions.Handlers.Base;
using InfiniteGallery.Exceptions.Handlers.Base.Contracts;

namespace InfiniteGallery.Exceptions.Base.Factories
{
    public class ExceptionHandlingActionFactory : IExceptionHandlingActionFactory
    {
        public IExceptionHandlingAction Create(Exception exception)
        {
            var genericType = typeof(ExceptionHandlingAction<>)
                .MakeGenericType(exception.GetType());
            return Activator.CreateInstance(genericType, exception) as IExceptionHandlingAction;
        }
    }
}