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

        private Helpers.FoodBlock MealBlock;

        public MealConfirmPopup(Helpers.FoodManager.FoodRecord food)
        {
            Food = food;
            BindingContext = Food;

            InitializeComponent();

            MealBlock = new Helpers.FoodBlock(food, this, true);
            TargetLayout.Children.Insert(0, MealBlock);
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

        private void AmountEntry_TextChanged(object sender, TextChangedEventArgs e)
        {
            double result = 0;
            if (Double.TryParse(e.NewTextValue, out result) && MealBlock != null)
            {
                MealBlock.ChangeMultiplier(result / Food.BaseAmount.Value);
            }
        }

        private void ModifyPortionButton_Clicked(object sender, EventArgs e)
        {
            double result = 0;
            if (Double.TryParse(AmountEntry.Text, out result))
                AmountEntry.Text = (result + Double.Parse((string)(((Button)sender).CommandParameter))).ToString();
        }
    }
}
