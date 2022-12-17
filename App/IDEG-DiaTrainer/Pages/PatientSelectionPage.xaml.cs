using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using IDEG_DiaTrainer.Helpers;
using Microsoft.Maui.Controls;

namespace IDEG_DiaTrainer.Pages
{
    /// <summary>
    /// ViewModel for patient selection to allow live update on patient selection
    /// </summary>
    public class PatientSelectionViewModel : INotifyPropertyChanged
    {
        // storage parameter for selected patient record
        private PatientRecord _SelectedPatient;
        // property parameter for selected patient record
        public PatientRecord SelectedPatient
        {
            get { return _SelectedPatient; }
            set { _SelectedPatient = value; OnPropertyChanged(); }
        }

        private string _ModelSelection = "GCT";
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
    public partial class PatientSelectionPage : ContentPage
    {
        // viewmodel for patient selection
        private PatientSelectionViewModel SelectionViewModel = new PatientSelectionViewModel();

        public PatientSelectionPage()
        {
            SimulationPage.SelectedModelType = Enums.ModelType.GCTv2;
            SelectionViewModel.PatientList = new ObservableCollection<PatientRecord>(Config.PatientMgr.GetDefaultCohort(Enums.ModelType.GCTv2));

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
                if (SelectionViewModel.ModelSelection == "GCT")
                {
                    SelectionViewModel.PatientList = new ObservableCollection<PatientRecord>(Config.PatientMgr.GetDefaultCohort(Enums.ModelType.GCTv2));
                    SimulationPage.SelectedModelType = Enums.ModelType.GCTv2;
                    SimulationPage.SelectedCustomPatient = false;
                }
                else if (SelectionViewModel.ModelSelection == "Bases")
                {
                    SelectionViewModel.PatientList = new ObservableCollection<PatientRecord>(Config.PatientMgr.GetDefaultCohort(Enums.ModelType.Bases));
                    SimulationPage.SelectedModelType = Enums.ModelType.Bases;
                    SimulationPage.SelectedCustomPatient = false;
                }
                else if (SelectionViewModel.ModelSelection == "cGCT")
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

        public async void OnContinueButtonClicked(object sender, EventArgs args)
        {
            if (SelectionViewModel.SelectedPatient == null)
                return;

            // this hurts, but works
            SimulationPage.SelectedControlType = Enums.ControlType.FreeRunning;
            SimulationPage.PatientId = SelectionViewModel.SelectedPatient.Id;

            await Shell.Current.GoToAsync("simulation", true);
        }
    }
}
