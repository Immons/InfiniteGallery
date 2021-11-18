namespace InfiniteGallery.Configuration.Ioc.Contracts
{
    public interface IInjector<in T> where T : class
    {
        void Inject(T service);
    }
}