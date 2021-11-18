using System.Net;
using System.Threading.Tasks;
using Acr.UserDialogs;
using InfiniteGallery.Exceptions.Handlers.Base.Attributes;
using InfiniteGallery.Exceptions.Handlers.Base.Contracts;
using Refit;

namespace InfiniteGallery.Exceptions.Handlers.Base.Default
{
    [DefaultExceptionHandler]
    public class ApiExceptionHandler : ExceptionHandler<ApiException>
    {
        private readonly IUserDialogs _userDialogs;

        public ApiExceptionHandler(
	        IUserDialogs userDialogs)
        {
	        _userDialogs = userDialogs;
        }

        public override async Task Handle(IExceptionHandlingAction<ApiException> exceptionHandlingAction)
        {
            await ShowMessage(nameof(ApiExceptionHandler), exceptionHandlingAction.Exception.Message, exceptionHandlingAction.Exception);
            exceptionHandlingAction.HandlingShouldFinish = true;
        }


        protected virtual async Task ShowMessage(
	        string title,
	        string message,
	        ApiException exception,
	        bool showWithDisplayStackTraceQuestion = true)
        {
            if (exception.StatusCode == HttpStatusCode.Unauthorized)
            {
                await _userDialogs.AlertAsync($"Call to {exception.Uri} is unauthorized", "Unauthorized");
                return;
            }
#if DEBUG
	        await _userDialogs.AlertAsync(message, title);
#endif
        }
    }
}