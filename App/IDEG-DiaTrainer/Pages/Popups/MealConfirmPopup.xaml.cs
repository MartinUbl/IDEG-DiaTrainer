using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using IDEG_DiaTrainer.Messages;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;

namespace IDEG_DiaTrainer.Pages.Popups
{
    /// <summary>
    /// Popup page for confirming meal selection and adjusting portion
    /// </summary>
    public partial class MealConfirmPopup : ContentPage, Helpers.IFoodBlockParent
    {
        // food record of seleted meal
        private Helpers.FoodRecord Food { get; set; }

        // encapsulated food block
        private Helpers.FoodBlock MealBlock;

        // stores multiplier after each change
        private double StoredMultiplier = 1.0;

        public MealConfirmPopup(Helpers.FoodRecord food)
        {
            Food = food;
            BindingContext = Food;

            InitializeComponent();

            // insert meal block into layout
            MealBlock = new Helpers.FoodBlock(food, this, true);
            TargetLayout.Children.Insert(0, MealBlock);
        }

        private async void ConfirmButton_Clicked(object sender, EventArgs e)
        {
            // this is here basically to just validate the inputs
            double result = 0;
            if (Double.TryParse(AmountEntry.Text, out result) && result > 0 && result < 10000)
            {
                // broadcast meal selection
                MessagingCenter.Send<InjectCarbsMessage>(new InjectCarbsMessage { 
                    CarbAmount = Food.Carbohydrates.HasValue ? Food.Carbohydrates.Value * StoredMultiplier : 0,
                    When = null,
                    IsRescue = false
                }, InjectCarbsMessage.Name);

                // this causes the previous page to be popped as well (so we return to simulation)
                // TODO: make this a bit more generic
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
            // parse the amount, set multiplier and propagate it 
            double result = 0;
            if (Double.TryParse(e.NewTextValue, out result) && MealBlock != null)
            {
                StoredMultiplier = result / Food.BaseAmount.Value;
                MealBlock.ChangeMultiplier(StoredMultiplier);
            }
        }

        private void ModifyPortionButton_Clicked(object sender, EventArgs e)
        {
            // propagate the change to amountentry
            double res = 0;
            if (Double.TryParse(AmountEntry.Text, out res))
                AmountEntry.Text = (res + Double.Parse((string)(((Button)sender).CommandParameter))).ToString();
        }
    }
}
