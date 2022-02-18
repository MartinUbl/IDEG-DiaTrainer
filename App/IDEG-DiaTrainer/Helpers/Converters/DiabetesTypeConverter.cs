using IDEG_DiaTrainer.Enums;
using Microsoft.Maui.Controls;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDEG_DiaTrainer.Helpers.Converters
{
    /// <summary>
    /// Converter of DiabetesType enum to string
    /// </summary>
    public class DiabetesTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var type = (DiabetesType)value;
            switch (type)
            {
                case DiabetesType.Type1: return "Diabetes type 1 (T1D)";
                case DiabetesType.Type2: return "Diabetes type 2 (T2D)";
            }

            return "Unknown diabetes type";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
