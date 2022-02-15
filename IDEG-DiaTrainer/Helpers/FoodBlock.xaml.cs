using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDEG_DiaTrainer.Helpers
{
    public class OutputConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var invalue = (double?)value;
            return invalue.HasValue ? invalue.Value + " g" : "N/A";
        }

        public object ConvertBack(object value, Type targetTypes, object parameter, CultureInfo culture)
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

        public FoodBlock(FoodManager.FoodRecord record, IFoodBlockParent parent)
        {
            Record = record;
            BindingContext = Record;
            BlockParent = parent;

            InitializeComponent();

            Stream s = ResourceHelper.GetNamedStream("IDEG_DiaTrainer.Resources.Content.Food.img." + record.ImageFilename);

            FoodImage.Source = ImageSource.FromStream(() => s);
        }

        private void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            BlockParent.FoodBlockTappedCallback(this);
        }
    }
}