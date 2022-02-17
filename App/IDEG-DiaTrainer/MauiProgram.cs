using Microsoft.Maui;
using Microsoft.Maui.Hosting;
using Microsoft.Maui.Controls.Compatibility;
using Microsoft.Maui.Controls.Hosting;

namespace IDEG_DiaTrainer
{
	public static class MauiProgram
	{
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
				});

			return builder.Build();
		}
	}
}