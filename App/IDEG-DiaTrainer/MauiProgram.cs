using Microsoft.Maui;
using Microsoft.Maui.Hosting;
using Microsoft.Maui.Controls.Compatibility;
using Microsoft.Maui.Controls.Hosting;
using Microsoft.Maui.LifecycleEvents;
using Microsoft.UI.Windowing;
using Microsoft.UI;
using System;
using WinRT.Interop;
using System.Threading.Tasks;

namespace IDEG_DiaTrainer
{
	public static class MauiProgram
	{
		private static bool IsFirstWindowOpened = false;

		public static MauiApp CreateMauiApp()
		{
			var builder = MauiApp.CreateBuilder();
			builder
				.UseMauiApp<App>()
				.ConfigureMauiHandlers(handlers =>
				{
					// NOTE: this should probably be solved by SkiaSharp library itself
					//       the implementation may be buggy at this time, so let us register the handler here manually
					handlers.AddHandler(typeof(SkiaSharp.Views.Maui.Controls.SKCanvasView), typeof(SkiaSharp.Views.Maui.Handlers.SKCanvasViewHandler));
				})
				.ConfigureFonts(fonts =>
				{
					fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				})
				.ConfigureLifecycleEvents(lifecycle => {
#if WINDOWS
					lifecycle.AddWindows (windows => {
						_ = windows.OnWindowCreated (async (window) => {
							if (!IsFirstWindowOpened)
							{
								//window.ExtendsContentIntoTitleBar = false;
								window.ExtendsContentIntoTitleBar = true;

								await Task.Delay(100);
								window.SetTitleBar(null);

								IsFirstWindowOpened = true;
								IntPtr nativeWindowHandle = WindowNative.GetWindowHandle(window);
								WindowId win32WindowsId = Win32Interop.GetWindowIdFromWindow(nativeWindowHandle);

								AppWindow appWindow = AppWindow.GetFromWindowId(win32WindowsId);

								// "maximize" window; this is a workaround, as Maui/WinUI does not yet support maximized mode without PInvoke/Win32

								if (appWindow.Presenter is OverlappedPresenter p)
									p.Maximize();
								else
								{
									appWindow.Move(new Windows.Graphics.PointInt32 { X = DisplayArea.Primary.WorkArea.X, Y = DisplayArea.Primary.WorkArea.Y });
									appWindow.Resize(new Windows.Graphics.SizeInt32 { Width = DisplayArea.Primary.WorkArea.Width, Height = DisplayArea.Primary.WorkArea.Height });
								}

								// NOTE: allow this for kiosk mode (e.g.; faculty open days, etc.)
								//appWindow.SetPresenter(FullScreenPresenter.Create());
							}
						});
					});
#endif
				});

			return builder.Build();
		}
	}
}