using System;

namespace InfiniteGallery.Configuration.Ioc.Modules.Base
{
	public class SingleInstanceType
	{
		public SingleInstanceType(Type type, bool autoActivate)
		{
			Type = type;
			AutoActivate = autoActivate;
		}

		public Type Type { get; }
		public bool AutoActivate { get; }
	}
}
