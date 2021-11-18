using Autofac;
using InfiniteGallery.Configuration;
using InfiniteGallery.ViewModels;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace InfiniteGallery
{
    public partial class App : BaseFormsApp
    {
        public App(IContainer container) : base(container)
        {
            InitializeComponent();
            ResolveAppStart<MainViewModel, string>();
        }
    }
}