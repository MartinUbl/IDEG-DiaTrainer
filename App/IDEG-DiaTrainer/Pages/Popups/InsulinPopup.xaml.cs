using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using IDEG_DiaTrainer.Components.Pumps;
using IDEG_DiaTrainer.Controllers;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;

namespace IDEG_DiaTrainer.Pages.Popups
{
	public partial class InsulinPopup : ContentPage
	{
		private PumpController pump;

		public InsulinPopup(PumpController controller)
		{
			pump = controller;
			InitializeComponent();

			DefaultPump ui = new DefaultPump(pump);
			ui.VerticalOptions = LayoutOptions.Center;
			ui.HorizontalOptions = LayoutOptions.Center;
			PumpUITarget.Add(ui);
		}

		private async void BatteryChargeButton_Clicked(object sender, EventArgs e)
		{
			bool confirm = await DisplayAlert("Charge battery?", "This will suspend basal insulin delivery for an hour. Are you sure you want to do this now?", "OK", "Later");
			if (confirm)
			{
                await DisplayAlert("Battery charged", "The battery was fully charged", "I understand");
				pump.BatteryCharge = 1;
            }
        }

		private async void InsulinFillButton_Clicked(object sender, EventArgs e)
		{
            bool confirm = await DisplayAlert("Change reservoir?", "This will suspend basal insulin delivery for 5 minutes, and will add $42 to overal costs. Are you sure you want to do this now?", "OK", "Later");
            if (confirm)
            {
                await DisplayAlert("Reservoir changed", "The insulin reservoir was replaced.", "I understand");
				pump.RemainingInsulin = PumpController.InsulinReservoirSize;
            }
        }

		private async void InfusionChangeButton_Clicked(object sender, EventArgs e)
		{
            bool confirm = await DisplayAlert("Change infusion set?", "This will suspend basal insulin delivery for 15 minutes, and will add $8 to overal costs. Are you sure you want to do this now?", "OK", "Later");
            if (confirm)
            {
                await DisplayAlert("Infusion set changed", "The insulin infusion set was replaced.", "I understand");
				// TODO: mechanisms for infusion set change?
            }
        }
    }
}