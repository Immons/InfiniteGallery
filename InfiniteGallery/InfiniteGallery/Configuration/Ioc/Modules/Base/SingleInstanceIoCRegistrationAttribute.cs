using System;

namespace InfiniteGallery.Configuration.Ioc.Modules.Base
{
	public class SingleInstanceIoCRegistrationAttribute : Attribute
	{
		public SingleInstanceIoCRegistrationAttribute(bool autoActivate = false)
		{
			AutoActivate = autoActivate;
		}

		public bool AutoActivate { get; }
	}
}
