using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using IDEG_DiaTrainer.Components.Pumps;
using IDEG_DiaTrainer.Controllers;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;

namespace IDEG_DiaTrainer.Pages
{
    public class CouldHaveResultsViewModel : INotifyPropertyChanged
    {
        private string _ResultText = "";
        public string ResultText
        {
            get { return _ResultText; }
            set { _ResultText = value; OnPropertyChanged(); }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }

    public partial class CouldHaveResultsPage : ContentPage
	{
        CouldHaveResultsViewModel viewModel = new CouldHaveResultsViewModel();

        public static double TimeDelta = 0;
        public static double ValueDelta = 0;

        public CouldHaveResultsPage()
		{
            viewModel.ResultText = "You've done ";

            if (TimeDelta < 0 && ValueDelta < 0)
                viewModel.ResultText += "better! The average glycemia was lower by " + (-ValueDelta).ToString("F2", CultureInfo.InvariantCulture) + " mmol/L and you spent " + (int)(-TimeDelta) + " less minutes in the risk zone.";
            else if (TimeDelta < 0 ^ ValueDelta < 0)
            {
                viewModel.ResultText += "somewhat better.";
                if (TimeDelta < 0)
                    viewModel.ResultText += " You've reduced the time in risk zone by " + (int)(-TimeDelta) + " minutes, but the average glycemia was higher by " + ValueDelta.ToString("F2", CultureInfo.InvariantCulture) + " mmol/L.";
                else
                    viewModel.ResultText += " You've reduced the average glycemia by " + (-ValueDelta).ToString("F2", CultureInfo.InvariantCulture) + " mmol/L, but spent " + (int)(TimeDelta) + " more minutes in the risk zone.";
            }
            else
                viewModel.ResultText += "worse... the average glycemia was higher by " + ValueDelta.ToString("F2", CultureInfo.InvariantCulture) + " mmol/L and you spent " + (int)(TimeDelta) + " more minutes in the risk zone.";

            BindingContext = viewModel;

			InitializeComponent();
		}

        private async void OKButton_Clicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("../../../..");
        }
    }
}
