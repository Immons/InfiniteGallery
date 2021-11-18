using System.Threading.Tasks;
using InfiniteGallery.Exceptions.Handlers.Base.Attributes;
using InfiniteGallery.Exceptions.Handlers.Base.Contracts;

namespace InfiniteGallery.Exceptions.Handlers.Base.Default
{
    [DefaultExceptionHandler]
    public class TaskCanceledExceptionHandler : ExceptionHandler<TaskCanceledException>
    {
        public override Task Handle(IExceptionHandlingAction<TaskCanceledException> exceptionHandlingAction)
        {
            //ignore
            exceptionHandlingAction.HandlingShouldFinish = true;
            return Task.CompletedTask;
        }
    }
}
