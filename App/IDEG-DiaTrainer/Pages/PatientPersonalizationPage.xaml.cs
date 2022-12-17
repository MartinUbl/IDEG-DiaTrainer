using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using IDEG_DiaTrainer.Config;
using IDEG_DiaTrainer.Helpers;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;

namespace IDEG_DiaTrainer.Pages
{
    /// <summary>
    /// ViewModel for patient personalization to allow live update on view
    /// </summary>
    public class PatientPersonalizationViewModel : INotifyPropertyChanged
    {
        private bool _IsEnteringPhase = true;
        public bool IsEnteringPhase
        {
            get { return _IsEnteringPhase; }
            set { _IsEnteringPhase = value; OnPropertyChanged(); }
        }

        private bool _IsOptimizePhase = false;
        public bool IsOptimizePhase
        {
            get { return _IsOptimizePhase; }
            set { _IsOptimizePhase = value; OnPropertyChanged(); }
        }

        private bool _IsPreliminaryAnalysis = true;
        public bool IsPreliminaryAnalysis
        {
            get { return _IsPreliminaryAnalysis; }
            set { _IsPreliminaryAnalysis = value; OnPropertyChanged(); }
        }

        private bool _IsConfirmationPhase = false;
        public bool IsConfirmationPhase
        {
            get { return _IsConfirmationPhase; }
            set { _IsConfirmationPhase = value; OnPropertyChanged(); }
        }

        private int _PctDone = 0;
        public int PctDone
        {
            get { return _PctDone; }
            set { _PctDone = value; OnPropertyChanged(); }
        }

        private double _MetricVal = 36;
        public double MetricVal
        {
            get { return _MetricVal; }
            set { _MetricVal = value; OnPropertyChanged(); }
        }

        private string _ProfileName = "";
        public string ProfileName
        {
            get { return _ProfileName; }
            set { _ProfileName = value; OnPropertyChanged(); }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }

    public partial class PatientPersonalizationPage : ContentPage
	{
        private string SelectedFileName = "";

        private PatientPersonalizationViewModel viewModel = new PatientPersonalizationViewModel();

        public PatientPersonalizationPage()
		{
            BindingContext = viewModel;

			InitializeComponent();
		}

        private async void SelectDataButton_Clicked(object sender, EventArgs e)
        {
            try
            {
                PickOptions options = new PickOptions { PickerTitle = "Select a sensor or pump data dump file" };

                var result = await FilePicker.PickAsync(options);
                if (result != null)
                {
                    SelectedFileName = result.FullPath;
                    SelectedFileLabel.Text = $"Selected file: {result.FullPath}";
                }
            }
            catch (Exception)
            {
                // user cancelled
            }
        }

        private IntPtr _ProgressHandle = IntPtr.Zero;

        Enums.ModelType CurrentModelType = Enums.ModelType.Bases; // initial phase = analysis using bases model
        private string FoundParameters = "";

        private Task<string> PerformOptimization(Enums.ModelType modelType, uint genCnt, uint popCnt)
        {
            return Task<string>.Run(() =>
            {
                if (!Directory.Exists(Microsoft.Maui.Storage.FileSystem.AppDataDirectory))
                    Directory.CreateDirectory(Microsoft.Maui.Storage.FileSystem.AppDataDirectory);

                string BaseFileName = Path.GetFileName(SelectedFileName);
                File.Copy(SelectedFileName, Path.Combine(Microsoft.Maui.Storage.FileSystem.AppDataDirectory, BaseFileName), true);

                string configName = "";
                switch (modelType)
                {
                    case Enums.ModelType.GCTv2:
                        configName = "config-gct2-learn";
                        break;
                    case Enums.ModelType.Bases:
                        configName = "config-bases-learn";
                        break;
                }

                string cfg = ConfigMgr.ReadConfig(configName, new System.Collections.Generic.Dictionary<string, string>() {
                    { "UploadFilename", BaseFileName },
                    { "InitialParameters", FoundParameters }
                });

                var paramstr = scgms.ComInterop.OptimizeParameters(cfg, 12, "Parameters", genCnt, popCnt, _ProgressHandle);

                Debug.WriteLine("Resolved parameters: " + paramstr);

                return paramstr;
            });
        }

        private void HandleProgressTimer()
        {
            if (_ProgressHandle != IntPtr.Zero)
            {
                double pctDone = 0, metric = 100;
                if (scgms.ComInterop.DumpOptimizerProgress(_ProgressHandle, out pctDone, out metric))
                {
                    viewModel.PctDone = ((int)(pctDone * 100));
                    viewModel.MetricVal = metric;
                }
            }
        }

        private async void PersonalizeButton_Clicked(object sender, EventArgs e)
        {
            if (SelectedFileName.Length == 0)
            {
                await DisplayAlert("Error", "Please, select the source file.", "OK");
                return;
            }

            if (viewModel.ProfileName.Length < 4 || viewModel.ProfileName.Length > 32 || viewModel.ProfileName.Contains(";"))
            {
                await DisplayAlert("Error", "Please, enter a valid profile name. The profile name should contain between 4-32 characters and should not contain any special characters", "OK");
                return;
            }

            if (PatientMgr.HasCustomPatient(viewModel.ProfileName))
            {
                bool res = await DisplayAlert("Name conflict", "You already stored a profile named " + viewModel.ProfileName + ". Should we overwrite it?", "Yes, overwrite", "No, I will choose another name");
                if (!res)
                    return;

                PatientMgr.RemoveCustomPatient(viewModel.ProfileName);
            }

            _ProgressHandle = scgms.ComInterop.OptimizerCreateProgress();

            bool repeat = false;
            bool success = true;
            viewModel.IsPreliminaryAnalysis = true;
            CurrentModelType = Enums.ModelType.Bases; // initial phase = analysis using bases model
            FoundParameters = PatientMgr.GetDefaultParameters(CurrentModelType);
            uint genCount = 500;
            uint popCount = 50;

            viewModel.IsEnteringPhase = false;
            viewModel.IsOptimizePhase = true;

            do
            {
                viewModel.PctDone = 0;

                System.Timers.Timer timer = new(interval: 1000);
                timer.Elapsed += (sender, e) => HandleProgressTimer();
                timer.Start();

                string result = await PerformOptimization(CurrentModelType, genCount, popCount);

                timer.Stop();
                timer.Dispose();

                if (result != null && result.Length > 0)
                {
                    FoundParameters = result;

                    if (viewModel.IsPreliminaryAnalysis)
                    {
                        bool continueAnalysis = viewModel.MetricVal < 1.8 ?
                            await DisplayAlert("Analysis complete", "Analysis completed and has found, that your data could be used. Should the application proceed with personalization?", "Yes", "Cancel")
                            : await DisplayAlert("Analysis complete", "Analysis completed and has found, that your data are probably not in a great condition for optimalization. Should the application proceed with personalization anyway?", "Yes", "Cancel");

                        if (continueAnalysis)
                        {
                            genCount = 1000;
                            popCount = 100;
                            viewModel.IsPreliminaryAnalysis = false;
                            repeat = true;
                        }
                        else
                        {
                            success = false;
                            break;
                        }
                    }
                    else
                    {
                        if (viewModel.MetricVal < 1.0)
                        {
                            bool res = await DisplayAlert("Personalization complete", "Personalization completed and there seems to be a good fit! Should we accept it as-is, or should we refine the model further?", "Yes, accept", "No, refine the model");

                            if (!res)
                            {
                                genCount = 1000;
                                popCount = 100;
                                repeat = true;
                            }
                            else
                            {
                                //
                                repeat = false;
                            }
                        }
                        else if (CurrentModelType == Enums.ModelType.Bases) // still the fast model but poor fit
                        {
                            bool res = await DisplayAlert("Personalization performs poorly", "Personalization completed and the fast model is unable to fit properly. Should we try the slower model?", "Try the slow model", "Cancel");

                            if (res)
                            {
                                CurrentModelType = Enums.ModelType.GCTv2;
                                FoundParameters = PatientMgr.GetDefaultParameters(CurrentModelType);
                                genCount = 200;
                                popCount = 50;
                            }
                            else
                            {
                                success = false;
                                break;
                            }
                        }
                        else
                        {
                            bool res = await DisplayAlert("Personalization performs poorly", "Personalization completed and both the models are unable to fit properly. Should we try again?", "Try the slow model again", "Cancel");

                            if (res)
                            {
                                genCount = 200;
                                popCount = 50;
                                repeat = true;
                            }
                            else
                            {
                                success = false;
                                break;
                            }
                        }
                    }
                }
                else
                {
                    viewModel.IsOptimizePhase = false;
                    viewModel.IsEnteringPhase = true;

                    await DisplayAlert("Error", "It seems that there was a problem with data you've uploaded. Please, select another data and try again.", "OK");
                    return;
                }
            }
            while (repeat);

            scgms.ComInterop.OptimizerDisposeProgress(_ProgressHandle);
            _ProgressHandle = IntPtr.Zero;

            if (success)
            {
                viewModel.IsOptimizePhase = false;
                viewModel.IsConfirmationPhase = true;
            }
            else
            {
                viewModel.IsEnteringPhase = true;
                viewModel.IsOptimizePhase = false;
            }
        }

        private async void SaveProfileButton_Clicked(object sender, EventArgs e)
        {
            int patId = PatientMgr.AddCustomPatient(CurrentModelType, viewModel.ProfileName, FoundParameters);
            PatientMgr.StoreCustomDataFileFor(patId, SelectedFileName);

            await DisplayAlert("Success", "The profile was saved! You can now select your profile in other parts of the application.", "OK");

            await Shell.Current.GoToAsync("..");
        }
    }

}