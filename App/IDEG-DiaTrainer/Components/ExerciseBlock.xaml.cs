using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using IDEG_DiaTrainer.Messages;

namespace IDEG_DiaTrainer.Helpers
{
	public class ExerciseBlockViewModel : INotifyPropertyChanged
	{
		// storage parameter - ExerciseRecord element
		private ExerciseRecord _Record = null;
		// property parameter - stores ExerciseRecord element
		public ExerciseRecord Record
		{
			get { return _Record; }
			set { _Record = value; OnPropertyChanged(); }
		}

		public event PropertyChangedEventHandler PropertyChanged;

		public void OnPropertyChanged([CallerMemberName] string name = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
		}
	}

	public partial class ExerciseBlock : Grid
	{
		private ExerciseBlockViewModel viewModel = new ExerciseBlockViewModel();

		public ExerciseBlock(ExerciseRecord record)
		{
			BindingContext = viewModel;
			viewModel.Record = record;

			InitializeComponent();

			// open image resource and try to create a stream from it
			Stream s = ResourceHelper.GetNamedStream("IDEG_DiaTrainer.Resources.Content.Exercise.img." + record.ImageFilename);
			if (s != null)
				ExerciseImage.Source = ImageSource.FromStream(() => s);
		}

        private void ConfirmButton_Clicked(object sender, EventArgs e)
        {
			int durationMins = -1;
            try
            {
				durationMins = int.Parse(DurationEntry.Text);
            }
			catch
            {
				//
            }

			if (durationMins <= 5)
            {
				//
				return;
            }

			MessagingCenter.Send<InjectExerciseMessage>(new InjectExerciseMessage
			{
				Intensity = viewModel.Record.Intensity.Value,
				When = null,
				CancelAfterSeconds = durationMins*60
			}, InjectExerciseMessage.Name);

			WindowManager.Instance.CloseWindow(WindowTypes.Exercise);
		}
    }

}
