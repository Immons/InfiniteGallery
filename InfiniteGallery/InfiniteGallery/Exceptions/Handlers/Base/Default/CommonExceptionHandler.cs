using System;
using System.Threading.Tasks;
using Acr.UserDialogs;
using InfiniteGallery.Exceptions.Handlers.Base.Attributes;
using InfiniteGallery.Exceptions.Handlers.Base.Contracts;

namespace InfiniteGallery.Exceptions.Handlers.Base.Default
{
    [DefaultExceptionHandler]
    public class CommonExceptionHandler : ExceptionHandler<Exception>
    {
        private readonly IUserDialogs _dialogService;
        private bool _messageBeingDisplayed;

        public CommonExceptionHandler(
            IUserDialogs dialogService)
        {
            _dialogService = dialogService;
        }

        public override async Task Handle(IExceptionHandlingAction<Exception> exceptionHandlingAction)
        {
	        if (_messageBeingDisplayed)
		        return;

	        _messageBeingDisplayed = true;
            await ShowMessage(nameof(CommonExceptionHandler), exceptionHandlingAction.Exception.Message, exceptionHandlingAction.Exception);
            _messageBeingDisplayed = false;
            exceptionHandlingAction.HandlingShouldFinish = true;
        }

        protected virtual async Task ShowMessage(
            string title,
            string message,
            Exception exception,
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
             await _dialogService.AlertAsync("Some problem occured, try again", "Error");
#endif
        }
    }
}