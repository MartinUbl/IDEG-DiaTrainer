using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDEG_DiaTrainer.Helpers
{
    /*
    public class OutputConverter : IValueConverter
    {
        public double ValueMultiplier { get; set; } = 1.0;

        public string Units { get; set; } = "g";

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var invalue = (double?)value;
            return invalue.HasValue ? invalue.Value * ValueMultiplier + " " + Units : "N/A";
        }

        public object ConvertBack(object value, Type targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
    */

    public class OutputConverter : IMultiValueConverter
    {
        public object Convert(object[] value, Type targetType, object parameter, CultureInfo culture)
        {
            var invalue = (double?)value[0];
            var mul = (double)(value[1] != null ? value[1] : 1.0);
            var units = (string)(value[2] != null ? value[2] : "?");
            return invalue.HasValue ? invalue.Value * mul + " " + units : "N/A";
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    public interface IFoodBlockParent
    {
        void FoodBlockTappedCallback(FoodBlock block);
    }

    public partial class FoodBlock : GridLayout
    {
        public FoodManager.FoodRecord Record { get; set; }

        public IFoodBlockParent BlockParent { get; set; }

        public double BaseMultiplier { get; set; } = 1.0;

        private Boolean FullBlock { get; set; } = false;

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
            }
        }

        public FoodBlock()
        {
            //
        }

        public FoodBlock(FoodManager.FoodRecord record, IFoodBlockParent parent, bool full)
        {
            Record = record;
            BindingContext = this;
            BlockParent = parent;
            FullBlock = full;

            InitializeComponent();

            DetailsGrid.IsVisible = FullBlock;
            ChangeMultiplier(1.0);

            Stream s = ResourceHelper.GetNamedStream("IDEG_DiaTrainer.Resources.Content.Food.img." + record.ImageFilename);

            FoodImage.Source = ImageSource.FromStream(() => s);
        }

        private void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            BlockParent.FoodBlockTappedCallback(this);
        }

        public void ChangeMultiplier(double mul)
        {
            //BindingContext = null;
            //OutGramsConv.ValueMultiplier = mul;
            //OutJouleConv.ValueMultiplier = mul;
            BaseMultiplier = mul;
            RecountVar.Text = String.Format("for {0} {1} of meal", Record.BaseAmount*mul, Record.Units);
            RecountVar.IsVisible = true;
            //BindingContext = Record;
        }

    }
}
