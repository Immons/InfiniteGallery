namespace InfiniteGallery.Configuration
{
	public static class CoreBootstrapper
	{
	    private const string UnixStartTime = "01.01.1970";

        private static bool _initialized;

		public static void Init()
		{
			if (_initialized) return;

        }
    }
}