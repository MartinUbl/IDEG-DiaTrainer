using Foundation;
using Microsoft.Maui;
using Microsoft.Maui.Hosting;

namespace IDEG_DiaTrainer.Platforms.MacCatalyst
{
    [Register("AppDelegate")]
    public class AppDelegate : MauiUIApplicationDelegate
    {
        protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
    }
}