using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using IDEG_DiaTrainer.Components.Pumps;
using IDEG_DiaTrainer.Controllers;
using IDEG_DiaTrainer.Helpers;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;

namespace IDEG_DiaTrainer.Pages.Popups
{
	public partial class ExercisePopup : ContentPage
	{
		private ExerciseManager exerciseManager;

		public ExercisePopup(ExerciseManager manager)
		{
			exerciseManager = manager;

			InitializeComponent();

			var recs = exerciseManager.GetExercise();
			foreach (var r in recs)
				ExerciseList.Children.Add(new Helpers.ExerciseBlock(r));
		}
	}
}
