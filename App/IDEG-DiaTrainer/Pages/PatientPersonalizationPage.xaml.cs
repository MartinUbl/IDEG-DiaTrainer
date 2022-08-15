using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using IDEG_DiaTrainer.Config;
using IDEG_DiaTrainer.Helpers;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;

namespace IDEG_DiaTrainer.Pages
{

	public partial class PatientPersonalizationPage : ContentPage
	{
        private string SelectedFileName = "";

		public PatientPersonalizationPage()
		{
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

        private void PersonalizeButton_Clicked(object sender, EventArgs e)
        {
            string BaseFileName = Path.GetFileName(SelectedFileName);
            File.Copy(SelectedFileName, Path.Combine(Microsoft.Maui.Storage.FileSystem.AppDataDirectory, BaseFileName), true);

            string cfg = ConfigMgr.ReadConfig("config-extracted", new System.Collections.Generic.Dictionary<string, string>() {
                { "UploadedFilename", BaseFileName }
            });
        }
    }

}