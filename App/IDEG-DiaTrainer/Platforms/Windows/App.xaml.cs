﻿using Microsoft.Maui;
using Microsoft.Maui.Hosting;
using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using System;
using Windows.ApplicationModel;
using Windows.UI.ViewManagement;
using WinRT.Interop;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace IDEG_DiaTrainer.WinUI
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public partial class App : MauiWinUIApplication
    {
        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
        }

        protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();

        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
            base.OnLaunched(args);

            Microsoft.Maui.Essentials.Platform.OnLaunched(args);

            var currentWindow = Application.Windows[0].Handler.NativeView;
            IntPtr _windowHandle = WindowNative.GetWindowHandle(currentWindow);
            var windowId = Win32Interop.GetWindowIdFromWindow(_windowHandle);

            AppWindow appWindow = AppWindow.GetFromWindowId(windowId);

            // "maximize" window; this is a workaround, as Maui/WinUI does not yet support maximized mode without PInvoke/Win32
            appWindow.Move(new Windows.Graphics.PointInt32 { X = DisplayArea.Primary.WorkArea.X, Y = DisplayArea.Primary.WorkArea.Y });
            appWindow.Resize(new Windows.Graphics.SizeInt32 { Width = DisplayArea.Primary.WorkArea.Width, Height = DisplayArea.Primary.WorkArea.Height });

            // NOTE: allow this for kiosk mode (e.g.; faculty open days, etc.)
            //appWindow.SetPresenter(FullScreenPresenter.Create());
        }
    }
}
