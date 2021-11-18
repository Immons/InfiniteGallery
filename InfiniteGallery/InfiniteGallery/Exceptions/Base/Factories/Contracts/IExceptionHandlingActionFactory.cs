using System;
using InfiniteGallery.Exceptions.Handlers.Base;
using InfiniteGallery.Exceptions.Handlers.Base.Contracts;

namespace InfiniteGallery.Exceptions.Base.Factories.Contracts
{
    public interface IExceptionHandlingActionFactory
    {
        IExceptionHandlingAction Create(Exception exception);
    }
}