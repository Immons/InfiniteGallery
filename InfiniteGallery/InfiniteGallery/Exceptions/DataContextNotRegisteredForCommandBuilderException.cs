using System;

namespace InfiniteGallery.Exceptions
{
	public class DataContextNotRegisteredForCommandBuilderException : Exception
	{
		public DataContextNotRegisteredForCommandBuilderException(Type commandBuilderType, Type dataContextType)
			: base($"You should set: {dataContextType.Name} through {commandBuilderType.Name}.RegisterDataContext method")
		{

		}
	}
}