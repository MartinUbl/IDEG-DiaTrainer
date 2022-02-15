using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;

namespace IDEG_DiaTrainer.Pages.Popups
{
    public partial class MealConfirmPopup : ContentPage, Helpers.IFoodBlockParent
    {
        private Helpers.FoodManager.FoodRecord Food { get; set; }

        public MealConfirmPopup(Helpers.FoodManager.FoodRecord food)
        {
            Food = food;

            InitializeComponent();

            TargetLayout.Children.Insert(0, new Helpers.FoodBlock(food, this));
        }

        private async void ConfirmButton_Clicked(object sender, EventArgs e)
        {
            if (true)
            {
                //
            }
            else
            {
                await DisplayAlert("No selection", "Please, select a meal to be eaten by clicking on it.", "OK");
            }
        }

        public void FoodBlockTappedCallback(Helpers.FoodBlock block)
        {
            //
        }
    }
}
