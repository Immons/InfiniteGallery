using System;
using System.Threading.Tasks;
using Acr.UserDialogs;
using InfiniteGallery.Exceptions.Handlers.Base.Attributes;
using InfiniteGallery.Exceptions.Handlers.Base.Contracts;

namespace InfiniteGallery.Exceptions.Handlers.Base.Default
{
	[DefaultExceptionHandler]
	public class NotImplementedExceptionHandler : ExceptionHandler<NotImplementedException>
	{
		private readonly IUserDialogs _userDialogs;

        public NotImplementedExceptionHandler(
			IUserDialogs userDialogs)
		{
			_userDialogs = userDialogs;
        }

		public override async Task Handle(IExceptionHandlingAction<NotImplementedException> exceptionHandlingAction)
		{
			await _userDialogs.AlertAsync("This feature is not implemented yet", "Not implemented");
			exceptionHandlingAction.HandlingShouldFinish = true;
		}
	}
}