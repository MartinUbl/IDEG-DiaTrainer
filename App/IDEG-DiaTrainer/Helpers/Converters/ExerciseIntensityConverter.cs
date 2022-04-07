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
    /// Converter of double value to exercise intensity text
    /// </summary>
    public class ExerciseIntensityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var intensity = (double)value;
            if (intensity < 0.11)
                return "Very light";
            else if (intensity < 0.21)
                return "Light";
            else if (intensity < 0.31)
                return "Moderate";
            else if (intensity < 0.46)
                return "Intensive";
            else
                return "Very intensive";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
