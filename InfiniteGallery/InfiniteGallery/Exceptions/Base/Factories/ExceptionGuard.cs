using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using InfiniteGallery.Exceptions.Base.Contracts;
using InfiniteGallery.Exceptions.Base.Factories.Contracts;
using InfiniteGallery.Exceptions.Handlers.Base.Attributes;
using InfiniteGallery.Exceptions.Handlers.Base.Contracts;
using InfiniteGallery.Exceptions.Handlers.Base.Default;
using InfiniteGallery.Extensions.Collections;
using InfiniteGallery.Extensions.Types;

namespace InfiniteGallery.Exceptions.Base.Factories
{
    public class ExceptionGuard : IExceptionGuard
    {
        private readonly IExceptionHandlingActionFactory _exceptionHandlingActionFactory;

        public ExceptionGuard(IEnumerable<IExceptionHandler> handlers, IExceptionHandlingActionFactory exceptionHandlingActionFactory)
        {
            _exceptionHandlingActionFactory = exceptionHandlingActionFactory;
            ExceptionHandlers.AddRange(handlers);
        }

        public IList<IExceptionHandler> ExceptionHandlers { get; } = new List<IExceptionHandler>();

        private IExceptionHandler this[Type type]
            => ExceptionHandlers.SingleOrDefault(h => h.GetType()
                .GetInterfaces()
                .FirstOrDefault(i => i == type) != null || h.GetType() == type);

        public void SetContextFor<THandler>(object context) where THandler : IExceptionHandler
        {
            var handler = this[typeof(THandler)];
            AssignContextIfValid(handler, context);
        }

        public async Task Guard(object sender, Func<Task> job, Func<Task> onFinally = null)
        {
            try
            {
                await job();
            }
            catch (Exception exception)
            {
                await HandleException(sender, exception);
            }
            finally
            {
                if (onFinally != null)
                    await onFinally();
            }
        }

        public async Task<TResult> Guard<TResult>(object sender, Func<Task<TResult>> job, Func<Task> onFinally = null)
        {
            TResult result = default;
            try
            {
                result = await job();
            }
            catch (Exception exception)
            {
                await HandleException(sender, exception);
            }
            finally
            {
                if (onFinally != null)
                    await onFinally();
            }

            return result;
        }

        private async Task HandleException(object sender, Exception exception)
        {
            if (ExceptionHandlers == null || !ExceptionHandlers.Any())
                return;

            var commonExceptionHandler = ExceptionHandlers
                .FirstOrDefault(h => h is CommonExceptionHandler);
            var handlers = new List<IExceptionHandler>();

            var compliantExceptionHandlers = ExceptionHandlers.Where(
                h => h != commonExceptionHandler
                     && HandlerShouldBePossibleToHandleException(
                         h,
                         exception));

            foreach (var handler in compliantExceptionHandlers)
            {
                var handlingOnAttribute = handler.GetAttribute<ExceptionHandlerForAttribute>();
                if (handlingOnAttribute != null)
                {
                    if (!handlingOnAttribute.TypesToHandleOn.Any(t => t.IsInstanceOfType(sender)))
                        continue;
                }
                else if (handler.GetAttribute<DefaultExceptionHandlerAttribute>() == null)
                    continue;

                if (await handler.CanHandle(exception))
                    handlers.Add(handler);
            }

            var exceptionHandlingAction = _exceptionHandlingActionFactory.Create(exception);

            if (!handlers.Any())
            {
                if (commonExceptionHandler != null)
                    await commonExceptionHandler.Handle(exceptionHandlingAction);
                //TODO probably we would like to throw if exception should not be handled
                return;
            }

            var handlersToUse = handlers.ToList();
            var customHandlers = handlers.Where(h => h.GetAttribute<DefaultExceptionHandlerAttribute>() == null)
                .ToList();

            if (customHandlers.Any())
            {
                handlersToUse = customHandlers.OrderBy(h => h.GetAttribute<ExceptionHandlerForAttribute>().PriorityLevel).ToList();
            }

            foreach (var exceptionHandler in handlersToUse)
            {
                if (exceptionHandlingAction.HandlingShouldFinish)
                    return;

                await exceptionHandler.Handle(exceptionHandlingAction);
            }
        }

        private bool HandlerShouldBePossibleToHandleException(IExceptionHandler exceptionHandler, Exception exception)
        {
            var type = GetExceptionTypeHandledByHandler(exceptionHandler);
            var isInstanceOfType = type.IsInstanceOfType(exception);
            return isInstanceOfType;
        }

        public void AssignContextToValidExceptionHandlers(object context)
        {
            var customHandlers = ExceptionHandlers
                .Where(h => h.GetAttribute<DefaultExceptionHandlerAttribute>() == null)
                .ToList();

            foreach (var exceptionHandler in customHandlers)
            {
                AssignContextIfValid(exceptionHandler, context);
            }
        }

        private void AssignContextIfValid(IExceptionHandler exceptionHandler, object context)
        {
            var arguments = exceptionHandler.GetType()
                .GetInterfaces()
                .FirstOrDefault(
                    i => i.IsClosedTypeOf(typeof(IContextExceptionHandler<,>))
                         && i.IsGenericType
                         && i.GenericTypeArguments.Length == 2)
                ?.GenericTypeArguments;

            if (arguments != null)
            {
                var contextArgument = arguments[1];

                if (contextArgument.IsInstanceOfType(context))
                {
                    var propertyInfo = typeof(IContextExceptionHandler<,>)
                        .MakeGenericType(arguments)
                        .GetProperty(nameof(IContextExceptionHandler<Exception, object>.Context));

                    var method = typeof(WeakReference<>)
                        .MakeGenericType(contextArgument)
                        .GetMethod(nameof(WeakReference<Exception>.SetTarget));

                    var weakContext = propertyInfo.GetValue(exceptionHandler);
                    method.Invoke(weakContext, new[] {context});
                }
            }
        }

        private Type GetExceptionTypeHandledByHandler(IExceptionHandler exceptionHandler)
        {
            var arguments = exceptionHandler.GetType()
                .GetInterfaces()
                .FirstOrDefault(
                    i => i.IsClosedTypeOf(typeof(IExceptionHandler<>))
                         && i.IsGenericType
                         && i.GenericTypeArguments.Length == 1)
                ?.GenericTypeArguments;

            return arguments?[0];
        }
    }
}