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
    public class OutputConverter : IMultiValueConverter
    {
        public object Convert(object[] value, Type targetType, object parameter, CultureInfo culture)
        {
            var invalue = (double?)value[0];
            var mul = (double)(value[1] != null ? value[1] : 1.0);
            var units = (string)(value[2] != null ? value[2] : "?");
            return invalue.HasValue ? (int)(invalue.Value * mul) + " " + units : "N/A";
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

    public class FoodBlockViewModel : INotifyPropertyChanged
    {
        private FoodManager.FoodRecord _Record = null;
        public FoodManager.FoodRecord Record
        {
            get
            {
                return _Record;
            }
            set
            {
                _Record = value;
                OnPropertyChanged("Record");
            }
        }

        private double _BaseMultiplier = 1.0;
        public double BaseMultiplier
        {
            get
            {
                return _BaseMultiplier;
            }
            set
            {
                _BaseMultiplier = value;
                OnPropertyChanged("BaseMultiplier");
            }
        }

        public string EnergyUnits { get; set; } = "kcal";
        public string CarbUnits { get; set; } = "g";

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }

    public partial class FoodBlock : Grid, INotifyPropertyChanged
    {
        public FoodBlockViewModel Food { get; set; }

        public IFoodBlockParent BlockParent { get; set; }

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
                OnPropertyChanged();
            }
        }

        public FoodBlock()
        {
            //
        }

        public FoodBlock(FoodManager.FoodRecord record, IFoodBlockParent parent, bool full)
        {
            Food = new FoodBlockViewModel { Record = record, BaseMultiplier = 1.0 };
            BindingContext = Food;
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
            Food.BaseMultiplier = mul;
            RecountVar.Text = String.Format("for {0} {1} of meal", (int)Math.Round(Food.Record.BaseAmount.Value * mul), Food.Record.Units);
            RecountVar.IsVisible = true;
        }
    }
}
