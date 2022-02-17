using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using SkiaSharp;
using SkiaSharp.Views.Maui.Controls;

namespace IDEG_DiaTrainer.Pages
{
    public partial class SimulationPage : ContentPage
    {
        Controllers.SimulationController controller;
        Helpers.FoodManager foodManager;

        public static readonly float ImageScale = 0.5f;

        string StoredDrawing_Glucose = "<svg version=\"1.1\" width=\"300\" height=\"200\" xmlns=\"http://www.w3.org/2000/svg\"><text x=\"40\" y=\"130em\" font-size=\"120em\">Loading...</text></svg>";
        string StoredDrawing_Insulin = "<svg version=\"1.1\" width=\"300\" height=\"200\" xmlns=\"http://www.w3.org/2000/svg\"><text x=\"40\" y=\"130em\" font-size=\"120em\">Loading...</text></svg>";
        string StoredDrawing_Carbs = "<svg version=\"1.1\" width=\"300\" height=\"200\" xmlns=\"http://www.w3.org/2000/svg\"><text x=\"40\" y=\"130em\" font-size=\"120em\">Loading...</text></svg>";

        bool ResizeMsgSent = false;

        public SimulationPage(int patientId = -1)
        {
            InitializeComponent();

            controller = new Controllers.SimulationController();
            controller.Start(patientId);

            foodManager = new Helpers.FoodManager();
            foodManager.Load();

            MessagingCenter.Subscribe<Messages.ValueAvailableMessage>(this, Messages.ValueAvailableMessage.Name, OnValueAvailable);
            MessagingCenter.Subscribe<Messages.DrawingAvailableMessage>(this, Messages.DrawingAvailableMessage.Name, OnDrawingAvailable);
        }

        private void OnValueAvailable(Messages.ValueAvailableMessage msg)
        {
            if (msg.SignalId == scgms.SignalGuids.InterstitiaryGlucose)
            {
                Device.BeginInvokeOnMainThread(() => {
                    GlucoseLabel.Text = msg.Value.ToString("F1");
                });
            }
            else if (msg.SignalId == scgms.SignalGuids.IOB)
            {
                Device.BeginInvokeOnMainThread(() => {
                    IOBLabel.Text = msg.Value.ToString("F1");
                });
            }
            else if (msg.SignalId == scgms.SignalGuids.COB)
            {
                Device.BeginInvokeOnMainThread(() => {
                    COBLabel.Text = msg.Value.ToString("F1");
                });
            }
        }

        private void OnDrawingAvailable(Messages.DrawingAvailableMessage msg)
        {
            if (!ResizeMsgSent)
            {
                double dens = 1.0 / (double)ImageScale;

                controller.SetDrawingSize(scgms.DrawingFilterInspection.DrawingType.Profile_Glucose, (int)(SVGView_Glucose.Width * dens), (int)(SVGView_Glucose.Height * dens));
                controller.SetDrawingSize(scgms.DrawingFilterInspection.DrawingType.Profile_Insulin, (int)(SVGView_Insulin.Width * dens), (int)(SVGView_Insulin.Height * dens));
                controller.SetDrawingSize(scgms.DrawingFilterInspection.DrawingType.Profile_Carbs, (int)(SVGView_Carbs.Width * dens), (int)(SVGView_Carbs.Height * dens));

                ResizeMsgSent = true;
            }
            StoredDrawing_Glucose = controller.GetDrawing(scgms.DrawingFilterInspection.DrawingType.Profile_Glucose);
            StoredDrawing_Insulin = controller.GetDrawing(scgms.DrawingFilterInspection.DrawingType.Profile_Insulin);
            StoredDrawing_Carbs = controller.GetDrawing(scgms.DrawingFilterInspection.DrawingType.Profile_Carbs);

            SVGView_Glucose.InvalidateSurface();
            SVGView_Insulin.InvalidateSurface();
            SVGView_Carbs.InvalidateSurface();
        }

        private void OnGraphPaint_Glucose(object sender, SkiaSharp.Views.Maui.SKPaintSurfaceEventArgs e)
        {
            OnGraphPaint_Generic(e, StoredDrawing_Glucose);
        }

        private void OnGraphPaint_Insulin(object sender, SkiaSharp.Views.Maui.SKPaintSurfaceEventArgs e)
        {
            OnGraphPaint_Generic(e, StoredDrawing_Insulin);
        }

        private void OnGraphPaint_Carbs(object sender, SkiaSharp.Views.Maui.SKPaintSurfaceEventArgs e)
        {
            OnGraphPaint_Generic(e, StoredDrawing_Carbs);
        }

        private void OnGraphPaint_Generic(SkiaSharp.Views.Maui.SKPaintSurfaceEventArgs e, string drawing)
        {
            var img = new SkiaSharp.Extended.Svg.SKSvg();
            using (var stream = new MemoryStream())
            {
                var writer = new StreamWriter(stream);
                writer.Write(drawing);
                writer.Flush();
                stream.Position = 0;
                img.Load(stream);
            }

            var surface = e.Surface;

            var canvas = surface.Canvas;

            canvas.Clear(SKColors.White);

            if (img == null)
                return;

            var matrix = SKMatrix.CreateScale(ImageScale, ImageScale);
            canvas.DrawPicture(img.Picture, ref matrix);
        }

        private async void MealButton_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new Popups.MealPopup(foodManager));

            // TODO: maybe use this instead, after it gets fixed in .NET 6.0
            /*
            var newWindow = new Window
            {
                Page = new Popups.MealPopup(foodManager)
            };
            Application.Current.OpenWindow(newWindow);
            */
        }
    }
}