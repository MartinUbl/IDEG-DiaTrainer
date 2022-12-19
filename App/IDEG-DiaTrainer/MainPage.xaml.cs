using System;
using System.Diagnostics;
using IDEG_DiaTrainer.Helpers;
using Microsoft.Maui.Controls;

namespace IDEG_DiaTrainer
{
	public partial class MainPage : ContentPage
	{
		public MainPage()
		{
			InitializeComponent();

            FoodManager.Instance.Load();
            ExerciseManager.Instance.Load();
		}

		public async void OnScenarioClicked(object sender, EventArgs args)
		{
            await Shell.Current.GoToAsync("scenarioselection");
        }

		public async void OnFreeRunningClicked(object sender, EventArgs args)
		{
            await Shell.Current.GoToAsync("patientselection");
        }

        public async void OnPersonalizedModeClicked(object sender, EventArgs args)
        {
            await Shell.Current.GoToAsync("patientpersonalization");
        }

        public async void OnCouldClicked(object sender, EventArgs args)
        {
            await Shell.Current.GoToAsync("couldselection");
        }
    }
}
