using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using IDEG_DiaTrainer.Helpers;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Essentials;

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
        // list of loaded patients to be displayed in output list
        private ObservableCollection<PatientRecord> PatientList;

        // viewmodel for patient selection
        private PatientSelectionViewModel SelectionViewModel = new PatientSelectionViewModel();

        public PatientSelectionPage()
        {
            // TODO: load patients from file
            PatientList = new ObservableCollection<PatientRecord>();
            for (int i = 0; i < 10; i++)
                PatientList.Add(new PatientRecord() { Name = "Patient " + (i + 1).ToString(), Id = i, Age = 35, Diabetes = Enums.DiabetesType.Type1 });

            SelectionViewModel.SelectedPatient = PatientList[0];

            BindingContext = SelectionViewModel;

            InitializeComponent();

            PatientListView.ItemsSource = PatientList;
            PatientListView.ItemSelected += (object sender, SelectedItemChangedEventArgs e) =>
            {
                DummyTextField.IsVisible = false;
                PatientDetailsField.IsVisible = true;
                SelectionViewModel.SelectedPatient = e.SelectedItem as PatientRecord;

                ContinueButton.IsEnabled = true;
            };
        }

        public /*async*/ void OnContinueButtonClicked(object sender, EventArgs args)
        {
            if (SelectionViewModel.SelectedPatient == null)
                return;

            //await Navigation.PushAsync(new Pages.SimulationPage(SelectionViewModel.SelectedPatient.Id));
            Application.Current.MainPage = new SimulationShell();
        }
    }
}
