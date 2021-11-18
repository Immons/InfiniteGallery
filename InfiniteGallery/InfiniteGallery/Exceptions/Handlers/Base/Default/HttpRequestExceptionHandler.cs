using System.Net.Http;
using System.Threading.Tasks;
using Acr.UserDialogs;
using InfiniteGallery.Exceptions.Handlers.Base.Attributes;
using InfiniteGallery.Exceptions.Handlers.Base.Contracts;
using Xamarin.Essentials;

namespace InfiniteGallery.Exceptions.Handlers.Base.Default
{
    [DefaultExceptionHandler]
    public class HttpRequestExceptionHandler : ExceptionHandler<HttpRequestException>
    {
	    private bool _messageBeingDisplayed = false;
        private readonly IUserDialogs _dialogService;

        public HttpRequestExceptionHandler(
            IUserDialogs dialogService)
        {
            _dialogService = dialogService;
        }

        public override async Task Handle(IExceptionHandlingAction<HttpRequestException> exceptionHandlingAction)
        {
	        if (_messageBeingDisplayed)
		        return;

	        _messageBeingDisplayed = true;
            await ShowMessage(nameof(HttpRequestException), exceptionHandlingAction.Exception.Message, exceptionHandlingAction.Exception);
            _messageBeingDisplayed = false;
            exceptionHandlingAction.HandlingShouldFinish = true;
        }

        protected virtual async Task ShowMessage(
	        string title,
	        string message,
	        HttpRequestException exception,
	        bool showWithDisplayStackTraceQuestion = true)
        {
#if !RELEASE
	        if (showWithDisplayStackTraceQuestion)
	        {
		        var result = await _dialogService.ConfirmAsync(message, title, "Show StackTrace", "Ok");
		        if (result)
			        await ShowMessage("StackTrace", exception.ToString(), exception, false);
	        }
	        else
	        {
		        await _dialogService.AlertAsync(exception.ToString(), title);
	        }

#else
	        var messageToShow = Connectivity.NetworkAccess == NetworkAccess.Internet ? "Some error occured when sending request, try again" : "No internet connection";
			await _dialogService.AlertAsync(messageToShow, "Error");
#endif
        }
    }
}