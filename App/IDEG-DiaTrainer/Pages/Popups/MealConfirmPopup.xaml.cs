using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using IDEG_DiaTrainer.Messages;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;

namespace IDEG_DiaTrainer.Pages.Popups
{
    public partial class MealConfirmPopup : ContentPage, Helpers.IFoodBlockParent
    {
        private Helpers.FoodManager.FoodRecord Food { get; set; }

        public double CurrentPortionSize { get; set; }

        private Helpers.FoodBlock MealBlock;

        public MealConfirmPopup(Helpers.FoodManager.FoodRecord food)
        {
            Food = food;
            BindingContext = Food;

            InitializeComponent();

            AmountEntry.Text = food.BaseAmount.ToString();

            MealBlock = new Helpers.FoodBlock(food, this, true);
            TargetLayout.Children.Insert(0, MealBlock);
        }

        private async void ConfirmButton_Clicked(object sender, EventArgs e)
        {
            double result = 0;
            if (Double.TryParse(AmountEntry.Text, out result) && result > 0 && result < 10000)
            {
                MessagingCenter.Send<InjectCarbsMessage>(new InjectCarbsMessage { 
                    CarbAmount = Food.Carbohydrates.HasValue ? Food.Carbohydrates.Value : 0,
                    When = null,
                    IsRescue = false
                }, InjectCarbsMessage.Name);

                // this causes the previous page to be popped as well (so we return to simulation)
                Navigation.RemovePage(Navigation.NavigationStack[Navigation.NavigationStack.Count - 2]);
                await Navigation.PopAsync();
            }
            else
            {
                await DisplayAlert("No selection", "Please, enter a valid amount.", "OK");
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
                double mul = result / Food.BaseAmount.Value;
                MealBlock.ChangeMultiplier(mul);
            }
        }

        private void ModifyPortionButton_Clicked(object sender, EventArgs e)
        {
            double res = 0;
            if (Double.TryParse(AmountEntry.Text, out res))
            {
                AmountEntry.Text = (res + Double.Parse((string)(((Button)sender).CommandParameter))).ToString();
            }
        }
    }
}
