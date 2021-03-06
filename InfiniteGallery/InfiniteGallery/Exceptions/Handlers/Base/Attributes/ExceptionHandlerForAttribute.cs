using System;

namespace InfiniteGallery.Exceptions.Handlers.Base.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface)]
    public class ExceptionHandlerForAttribute : Attribute
    {
        public ExceptionHandlerForAttribute(int priorityLevel, params Type[] typesToHandleOn)
        {
            PriorityLevel = priorityLevel;
            TypesToHandleOn = typesToHandleOn;
        }

        public ExceptionHandlerForAttribute(params Type[] typesToHandleOn)
        {
            PriorityLevel = 0;
            TypesToHandleOn = typesToHandleOn;
        }

        public int PriorityLevel { get; }
        public Type[] TypesToHandleOn { get; }
    }
}
