using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace IDEG_DiaTrainer.Helpers
{
    /// <summary>
    /// Interface for parent of a food block - implements a "clicked" callback if needed
    /// </summary>
    public interface IFoodBlockParent
    {
        /// <summary>
        /// Executed on "tapped" event on food block
        /// </summary>
        /// <param name="block"></param>
        void FoodBlockTappedCallback(FoodBlock block);
    }

    /// <summary>
    /// View model for foodblock - updates view bindings if needed
    /// </summary>
    public class FoodBlockViewModel : INotifyPropertyChanged
    {
        // storage parameter - FoodRecord element
        private FoodRecord _Record = null;
        // property parameter - stores FoodRecord element
        public FoodRecord Record
        {
            get { return _Record; }
            set { _Record = value; OnPropertyChanged(); }
        }

        // storage parameter - multiplier of output values
        private double _BaseMultiplier = 1.0;
        // property parameter - stores multiplier of output values (converter multiplies all values by this factor)
        public double BaseMultiplier
        {
            get { return _BaseMultiplier; }
            set { _BaseMultiplier = value; OnPropertyChanged(); }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }

    /// <summary>
    /// FoodBlock UI component - displays a food with title, and optionally with all parameters (such as carbohydrates, fat, ...)
    /// </summary>
    public partial class FoodBlock : Grid, INotifyPropertyChanged
    {
        // stored viewmodel of displayed food
        public FoodBlockViewModel Food { get; set; }

        // parent of this block - used to signalize tapped event
        public IFoodBlockParent BlockParent { get; set; }

        // is this block a full block? (contains food parameters, ...)
        private Boolean FullBlock { get; set; } = false;

        // is this block currently selected?
        private Boolean _IsSelected = false;
        public Boolean IsSelected
        {
            get
            {
                return _IsSelected;
            }
            set
            {
                _IsSelected = value;
                if (_IsSelected)
                    BackgroundColor = Color.FromRgba(192, 192, 224, 255);
                else
                    BackgroundColor = KnownColor.Transparent;
                OnPropertyChanged();
            }
        }

        public FoodBlock()
        {
            //
        }

        public FoodBlock(FoodRecord record, IFoodBlockParent parent, bool full)
        {
            // create viewmodel and bind to it
            Food = new FoodBlockViewModel { Record = record, BaseMultiplier = 1.0 };
            BindingContext = Food;
            BlockParent = parent;
            FullBlock = full;

            InitializeComponent();

            DetailsGrid.IsVisible = FullBlock;
            ChangeMultiplier(1.0);

            // open image resource and try to create a stream from it
            Stream s = ResourceHelper.GetNamedStream("IDEG_DiaTrainer.Resources.Content.Food.img." + record.ImageFilename);
            if (s != null)
                FoodImage.Source = ImageSource.FromStream(() => s);
        }

        private void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            BlockParent.FoodBlockTappedCallback(this);
        }

        /// <summary>
        /// Changes a multiplier of displayed meal
        /// </summary>
        /// <param name="mul">new multiplier</param>
        public void ChangeMultiplier(double mul)
        {
            Food.BaseMultiplier = mul;
            // TODO: binding instead of setting the text explicitly
            RecountVar.Text = String.Format("for {0} {1} of meal", (int)Math.Round(Food.Record.BaseAmount.Value * mul), Food.Record.Units);
            RecountVar.IsVisible = true;
        }
    }
}
