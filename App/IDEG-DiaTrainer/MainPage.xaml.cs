using System;
using System.Diagnostics;
using Microsoft.Maui.Controls;

namespace IDEG_DiaTrainer
{
	public partial class MainPage : ContentPage
	{
		public MainPage()
		{
			InitializeComponent();
		}

		/*
		private void ExecuteCb(scgms.ScgmsEvent evt)
		{
			//DisplayAlert("ALERT", evt.eventCode.ToString(), "ok");
			Debug.WriteLine("EVENT: " + evt.eventCode.ToString());
			Debug.WriteLine("CODE: " + ((uint)evt.eventCode));
			if (evt.eventCode == scgms.EventCode.Error)
				Debug.WriteLine(evt.infoString);
			else if (evt.eventCode == scgms.EventCode.Level)
				Debug.WriteLine("Level = " + evt.level);
		}
		*/

		public void OnScenarioClicked(object sender, EventArgs args)
		{
			DisplayAlert("Not implemented", "We haven't implemented this feature yet", "I understand");

			/*
			scgms.Execution execution = new scgms.Execution();

			execution.RegisterCallback(ExecuteCb);
			execution.Start("config-s2013");

			scgms.ScgmsEvent evt = new scgms.ScgmsEvent();
			evt.eventCode = scgms.EventCode.Level;
			evt.signalId = scgms.SignalGuids.Synchronization;
			evt.deviceTime = scgms.Utils.DateTimeToRatTime(DateTime.UtcNow);
			evt.level = 0;
			evt.segmentId = 1;
			execution.InjectEvent(evt);
			*/
		}

		public async void OnFreeRunningClicked(object sender, EventArgs args)
		{
			await Navigation.PushAsync(new Pages.PatientSelectionPage());
		}
	}
}
