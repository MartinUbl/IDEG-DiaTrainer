using System;
using System.Collections.ObjectModel;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Essentials;

namespace IDEG_DiaTrainer.Pages
{
    public class PatientDef
    {
        public int Id { get; set; }

        public String Name { get; set; }

        public int Age { get; set; }

        public String Diabetes { get; set; }
    }

    public partial class PatientSelectionPage : ContentPage
    {
        private ObservableCollection<PatientDef> PatientList;

        PatientDef SelectedPatient;

        public PatientSelectionPage()
        {
            // TODO: load patients from file
            PatientList = new ObservableCollection<PatientDef>();
            for (int i = 0; i < 10; i++)
                PatientList.Add(new PatientDef() { Name = "Patient " + (i + 1).ToString(), Id = i, Age = 35, Diabetes = Enums.DiabetesType.DiabetesType1 });

            SelectedPatient = PatientList[0];

            InitializeComponent();
            BindingContext = this;

            PatientListView.ItemsSource = PatientList;
            PatientListView.ItemSelected += (object sender, SelectedItemChangedEventArgs e) =>
            {
                DummyTextField.IsVisible = false;
                PatientDetailsField.IsVisible = true;
                SelectedPatient = e.SelectedItem as PatientDef;

                NameLabel.Text = SelectedPatient.Name;
                AgeLabel.Text = SelectedPatient.Age.ToString();
                DiabLabel.Text = SelectedPatient.Diabetes;

                ContinueButton.IsEnabled = true;

                //DisplayAlert("ItemSelected", e.SelectedItem.ToString(), "Ok");
            };
        }

        public async void OnContinueButtonClicked(object sender, EventArgs args)
        {
            if (SelectedPatient == null)
                return;

            await Navigation.PushAsync(new Pages.SimulationPage(SelectedPatient.Id));
        }
    }
}