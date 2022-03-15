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
	}
}