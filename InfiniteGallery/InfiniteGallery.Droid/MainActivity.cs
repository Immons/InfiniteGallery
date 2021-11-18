using Acr.UserDialogs;
using Android.App;
using Android.Content.PM;
using Android.Views;
using Android.OS;
using FFImageLoading.Forms.Platform;
using InfiniteGallery.Droid.Configuration.Ioc;
using Xamarin.Forms;

namespace InfiniteGallery.Droid
{
    [Activity(Label = "InfiniteGallery", Theme = "@style/MainTheme", MainLauncher = true,
        ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);

            UserDialogs.Init(() => this);
            CachedImageRenderer.Init(false);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);

            var container = Bootstrapper.Init();
            Forms.Init(this, savedInstanceState);
            Window.SetSoftInputMode(SoftInput.AdjustResize);
            LoadApplication(new App(container));
        }
    }
}