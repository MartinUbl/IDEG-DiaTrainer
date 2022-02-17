using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;

namespace IDEG_DiaTrainer.Pages.Popups
{
    public partial class MealPopup : ContentPage, Helpers.IFoodBlockParent
    {
        private Helpers.FoodManager foodManager;

        private Helpers.FoodBlock SelectedBlock = null;

        public MealPopup(Helpers.FoodManager food)
        {
            InitializeComponent();

            foodManager = food;

            var recs = foodManager.GetFood();
            foreach (var r in recs)
                FoodList.Children.Add(new Helpers.FoodBlock(r, this, false));
        }

        private async void ConfirmButton_Clicked(object sender, EventArgs e)
        {
            if (SelectedBlock != null)
            {
                await Navigation.PushAsync(new MealConfirmPopup(SelectedBlock.Food.Record));
            }
            else
            {
                await DisplayAlert("No selection", "Please, select a meal to be eaten by clicking on it.", "OK");
            }
        }

        public void FoodBlockTappedCallback(Helpers.FoodBlock block)
        {
            if (SelectedBlock != null)
                SelectedBlock.IsSelected = false;

            SelectedBlock = block;

            if (SelectedBlock != null)
                SelectedBlock.IsSelected = true;
        }
    }
}
