using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.Versioning;
using IDEG_DiaTrainer.Controllers;
using IDEG_DiaTrainer.Helpers;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using SkiaSharp;
using SkiaSharp.Views.Maui.Controls;

namespace IDEG_DiaTrainer.Pages
{
    /// <summary>
    /// ViewModel for simulation page
    /// </summary>
    public class SimulationViewModel : INotifyPropertyChanged
    {
        // storage parameter - current glucose level
        private double _CurrentGlucose;
        // property parameter - current glucose level
        public double CurrentGlucose
        {
            get { return _CurrentGlucose; }
            set { _CurrentGlucose = value; OnPropertyChanged(); }
        }

        // storage parameter - current carbohydrates on board
        private double _CurrentCOB;
        // property parameter - current carbohydrates on board
        public double CurrentCOB
        {
            get { return _CurrentCOB; }
            set { _CurrentCOB = value; OnPropertyChanged(); }
        }

        // storage parameter - current insulin on board
        private double _CurrentIOB;
        // property parameter - current insulin on board
        public double CurrentIOB
        {
            get { return _CurrentIOB; }
            set { _CurrentIOB = value; OnPropertyChanged(); }
        }

        // storage parameter - current datetime
        private DateTime _CurrentDateTime = DateTime.UtcNow;
        // property parameter - current insulin on board
        public DateTime CurrentDateTime
        {
            get { return _CurrentDateTime; }
            set { _CurrentDateTime = value; OnPropertyChanged(); }
        }

        // storage parameter - is simulation paused?
        private Boolean _IsPaused = false;
        // property parameter - is simulation paused?
        public Boolean IsPaused
        {
            get { return _IsPaused; }
            set { _IsPaused = value; OnPropertyChanged(); }
        }

        private PumpController _Pump = new PumpController();
        public PumpController Pump
        {
            get { return _Pump; }
            set { _Pump = value; OnPropertyChanged(); }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }

    /// <summary>
    /// Simulation page - main page of the whole app
    /// </summary>
    public partial class SimulationPage : ContentPage
    {
        // image scale of SVGs; this may be different between platforms
        private static readonly float ImageScale = 0.5f;

        // controller of the simulation - holds scgms execution environment
        private Controllers.SimulationController controller;

        // food manager - loads and manages known food
        private Helpers.FoodManager foodManager;

        // exercise manager - loads and manages known exercise
        private Helpers.ExerciseManager exerciseManager;

        // viewmodel instance
        private SimulationViewModel simulationViewModel = new SimulationViewModel();

        // stored drawing - glucose graph
        private string StoredDrawing_Glucose = "<svg version=\"1.1\" width=\"300\" height=\"200\" xmlns=\"http://www.w3.org/2000/svg\"><text x=\"40\" y=\"130em\" font-size=\"120em\">Loading...</text></svg>";
        // stored drawing - insulin graph
        private string StoredDrawing_Insulin = "<svg version=\"1.1\" width=\"300\" height=\"200\" xmlns=\"http://www.w3.org/2000/svg\"><text x=\"40\" y=\"130em\" font-size=\"120em\">Loading...</text></svg>";
        // stored drawing - carbs graph
        private string StoredDrawing_Carbs = "<svg version=\"1.1\" width=\"300\" height=\"200\" xmlns=\"http://www.w3.org/2000/svg\"><text x=\"40\" y=\"130em\" font-size=\"120em\">Loading...</text></svg>";

        // indicator of resize message being sent
        private bool ResizeMsgSent = false;

        public SimulationPage()
        {
            BindingContext = simulationViewModel;

            InitializeComponent();

            // initialize controller
            controller = new Controllers.SimulationController();
            controller.Start(-1); // TODO

            // initialize food manager
            foodManager = new Helpers.FoodManager();
            foodManager.Load();

            // initialize exercise manager
            exerciseManager = new Helpers.ExerciseManager();
            exerciseManager.Load();

            // subscribe to messages from controller
            MessagingCenter.Subscribe<Messages.ValueAvailableMessage>(this, Messages.ValueAvailableMessage.Name, OnValueAvailable);
            MessagingCenter.Subscribe<Messages.DrawingAvailableMessage>(this, Messages.DrawingAvailableMessage.Name, OnDrawingAvailable);
        }

        private void OnValueAvailable(Messages.ValueAvailableMessage msg)
        {
            // update viewmodel on new value

            if (msg.SignalId == scgms.SignalGuids.InterstitiaryGlucose)
                simulationViewModel.CurrentGlucose = msg.Value;
            else if (msg.SignalId == scgms.SignalGuids.IOB)
                simulationViewModel.CurrentIOB = msg.Value;
            else if (msg.SignalId == scgms.SignalGuids.COB)
                simulationViewModel.CurrentCOB = msg.Value;

            simulationViewModel.CurrentDateTime = msg.DeviceTime;

            Dispatcher.Dispatch(() => {
                TimelineDrawable.CurrentDateTime = simulationViewModel.CurrentDateTime;
                TimelineCanvas.Invalidate();
            });
        }

        private void InvalidateSurfaces()
        {
            SVGView_Glucose.InvalidateSurface();
            SVGView_Insulin.InvalidateSurface();
            SVGView_Carbs.InvalidateSurface();
        }

        private void OnDrawingAvailable(Messages.DrawingAvailableMessage msg)
        {
            if (!ResizeMsgSent)
            {
                // calculate the density
                double dens = 1.0 / ImageScale;

                // send resize messages
                controller.SetDrawingSize(scgms.DrawingFilterInspection.DrawingType.Profile_Glucose, (int)(SVGView_Glucose.Width * dens), (int)(SVGView_Glucose.Height * dens));
                controller.SetDrawingSize(scgms.DrawingFilterInspection.DrawingType.Profile_Insulin, (int)(SVGView_Insulin.Width * dens), (int)(SVGView_Insulin.Height * dens));
                controller.SetDrawingSize(scgms.DrawingFilterInspection.DrawingType.Profile_Carbs, (int)(SVGView_Carbs.Width * dens), (int)(SVGView_Carbs.Height * dens));

                ResizeMsgSent = true;
            }

            // render SVGs - retrieve them from scgms environment
            StoredDrawing_Glucose = controller.GetDrawing(scgms.DrawingFilterInspection.DrawingType.Profile_Glucose);
            StoredDrawing_Insulin = controller.GetDrawing(scgms.DrawingFilterInspection.DrawingType.Profile_Insulin);
            StoredDrawing_Carbs = controller.GetDrawing(scgms.DrawingFilterInspection.DrawingType.Profile_Carbs);

            // invalidate output
            InvalidateSurfaces();
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

        private void Pause()
        {
            if (simulationViewModel.IsPaused)
                return;

            simulationViewModel.IsPaused = true;
            controller.Pause();
        }

        private void Resume()
        {
            if (!simulationViewModel.IsPaused)
                return;

            simulationViewModel.IsPaused = false;
            controller.Resume();
        }

        private void PlayButton_Clicked(object sender, EventArgs e)
        {
            if (simulationViewModel.IsPaused)
                Resume();
            else
                Pause();
        }

        private void MealButton_Clicked(object sender, EventArgs e)
        {
            WindowManager.Instance.OpenWindow(WindowTypes.Meal, new Popups.MealPopup(foodManager), true);
        }

        private void InsulinButton_Clicked(object sender, EventArgs e)
        {
            WindowManager.Instance.OpenWindow(WindowTypes.Insulin, new Popups.InsulinPopup(simulationViewModel.Pump), true);
        }

        private void ExerciseButton_Clicked(object sender, EventArgs e)
        {
            WindowManager.Instance.OpenWindow(WindowTypes.Exercise, new Popups.ExercisePopup(exerciseManager), true);
        }
    }
}