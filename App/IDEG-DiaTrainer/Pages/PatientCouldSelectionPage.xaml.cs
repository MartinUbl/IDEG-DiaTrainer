using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using IDEG_DiaTrainer.Helpers;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Controls;

namespace IDEG_DiaTrainer.Pages
{
    /// <summary>
    /// ViewModel for patient selection to allow live update on patient selection
    /// </summary>
    public class PatientCouldSelectionViewModel : INotifyPropertyChanged
    {
        // storage parameter for selected patient record
        private PatientRecord _SelectedPatient;
        // property parameter for selected patient record
        public PatientRecord SelectedPatient
        {
            get { return _SelectedPatient; }
            set { _SelectedPatient = value; OnPropertyChanged(); }
        }

        private string _ModelSelection = "cBases";
        public string ModelSelection
        {
            get { return _ModelSelection; }
            set { _ModelSelection = value; OnPropertyChanged(); }
        }

        // list of loaded patients to be displayed in output list
        private ObservableCollection<PatientRecord> _PatientList;
        public ObservableCollection<PatientRecord> PatientList
        {
            get { return _PatientList; }
            set { _PatientList = value; OnPropertyChanged(); }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }

    /// <summary>
    /// Page for patient selection
    /// </summary>
    public partial class PatientCouldSelectionPage : ContentPage
    {
        // viewmodel for patient selection
        private PatientCouldSelectionViewModel SelectionViewModel = new PatientCouldSelectionViewModel();

        public PatientCouldSelectionPage()
        {
            SimulationPage.SelectedCustomPatient = true;
            SimulationPage.SelectedModelType = Enums.ModelType.Bases;
            SelectionViewModel.PatientList = new ObservableCollection<PatientRecord>(Config.PatientMgr.GetCustomCohort(Enums.ModelType.Bases));

            if (SelectionViewModel.PatientList.Count > 0)
                SelectionViewModel.SelectedPatient = SelectionViewModel.PatientList[0];

            BindingContext = SelectionViewModel;

            SelectionViewModel.PropertyChanged += SelectionViewModel_PropertyChanged;

            InitializeComponent();

            PatientListView.ItemSelected += (object sender, SelectedItemChangedEventArgs e) =>
            {
                DummyTextField.IsVisible = false;
                PatientDetailsField.IsVisible = true;
                SelectionViewModel.SelectedPatient = e.SelectedItem as PatientRecord;

                ContinueButton.IsEnabled = true;
            };
        }

        private void SelectionViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "ModelSelection")
            {
                if (SelectionViewModel.ModelSelection == "cGCT")
                {
                    SelectionViewModel.PatientList = new ObservableCollection<PatientRecord>(Config.PatientMgr.GetCustomCohort(Enums.ModelType.GCTv2));
                    SimulationPage.SelectedModelType = Enums.ModelType.GCTv2;
                    SimulationPage.SelectedCustomPatient = true;
                }
                else if (SelectionViewModel.ModelSelection == "cBases")
                {
                    SelectionViewModel.PatientList = new ObservableCollection<PatientRecord>(Config.PatientMgr.GetCustomCohort(Enums.ModelType.Bases));
                    SimulationPage.SelectedModelType = Enums.ModelType.Bases;
                    SimulationPage.SelectedCustomPatient = true;
                }

                if (SelectionViewModel.PatientList.Count > 0)
                    SelectionViewModel.SelectedPatient = SelectionViewModel.PatientList[0];
            }
        }

        Controllers.SimulationController controller;

        public void OnContinueButtonClicked(object sender, EventArgs args)
        {
            if (SelectionViewModel.SelectedPatient == null)
                return;

            // this hurts, but works
            SimulationPage.SelectedControlType = Enums.ControlType.CouldHaveIDoneBetter;
            SimulationPage.PatientId = SelectionViewModel.SelectedPatient.Id;

            MessagingCenter.Subscribe<Messages.ValueAvailableMessage>(this, Messages.ValueAvailableMessage.Name, OnValueAvailable);
            MessagingCenter.Subscribe<Messages.SimulationShutdownMessage>(this, Messages.SimulationShutdownMessage.Name, OnShutDown);
            controller = new Controllers.SimulationController();
            controller.Start(SimulationPage.SelectedModelType, Enums.ControlType.RiskIdentify, SelectionViewModel.SelectedPatient.Id, true);

            // TODO: disable controls?
        }

        private DateTime? SegmentStart = null, SegmentStop = null;

        private class RiskEpisode
        {
            public bool isHyper = false;
            public DateTime when;
            public double duration;
            public double avg;
        }

        private List<RiskEpisode> RiskEpisodes = new List<RiskEpisode>();

        private void OnValueAvailable(Messages.ValueAvailableMessage msg)
        {
            if (!SegmentStart.HasValue)
                SegmentStart = msg.DeviceTime;
            SegmentStop = msg.DeviceTime;

            if (msg.SignalId == scgms.SignalGuids.RiskHyper)
            {
                RiskEpisodes.Add(new RiskEpisode {
                    isHyper = true,
                    when = msg.DeviceTime,
                    duration = msg.Value,
                    avg = 0,
                });
            }
            else if (msg.SignalId == scgms.SignalGuids.RiskHypo)
            {
                RiskEpisodes.Add(new RiskEpisode
                {
                    isHyper = false,
                    when = msg.DeviceTime,
                    duration = msg.Value,
                    avg = 0,
                });
            }
            else if (msg.SignalId == scgms.SignalGuids.RiskAvgValue)
            {
                foreach (var ep in RiskEpisodes)
                {
                    if (ep.when == msg.DeviceTime)
                        ep.avg = msg.Value;
                }
            }
        }

        private void OnShutDown(Messages.SimulationShutdownMessage msg)
        {
            MessagingCenter.Unsubscribe<Messages.ValueAvailableMessage>(this, Messages.ValueAvailableMessage.Name);
            MessagingCenter.Unsubscribe<Messages.SimulationShutdownMessage>(this, Messages.SimulationShutdownMessage.Name);
            controller = null;

            foreach (var eps in RiskEpisodes)
            {
                Debug.WriteLine("Identified " + (eps.isHyper ? "hyper" : "hypo") + " at " + eps.when.ToString() + " lasting " + eps.duration + " minutes, avg glycemia = " + eps.avg + " mmol/L");
            }

            Random random = new Random();
            int idx = random.Next(0, RiskEpisodes.Count);

            SimulationPage.StopTime = RiskEpisodes[idx].when.AddMinutes(-15);
            SimulationPage.TerminateTime = RiskEpisodes[idx].when.AddMinutes(RiskEpisodes[idx].duration + 60);

            SimulationPage.MeasureStartTime = RiskEpisodes[idx].when;
            SimulationPage.MeasureStopTime = RiskEpisodes[idx].when.AddMinutes(RiskEpisodes[idx].duration);
            SimulationPage.MeasureAvgGlycemia = RiskEpisodes[idx].avg;

            Dispatcher.Dispatch(async () => {

                await DisplayAlert("All set",
                    "The application went through the data you supplied and found a " + (RiskEpisodes[idx].isHyper ? "hyperglycemic" : "hypoglycemic") + " episode from " + RiskEpisodes[idx].when.ToString() +
                    " that you previously failed to handle well. Your task is to handle it correctly this time. The simulation starts 15 minutes prior the episode and ends an hour after the end.", "I understand, let's do it");

                await Shell.Current.GoToAsync("simulation", true);
            });
        }
    }
}
