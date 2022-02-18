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
    /// Output converter for multiplied value with unit
    /// </summary>
    public class MealParameterConverter : IMultiValueConverter
    {
        public object Convert(object[] value, Type targetType, object parameter, CultureInfo culture)
        {
            var invalue = (double?)value[0];
            var mul = (double)(value[1] != null ? value[1] : 1.0);
            var units = (string)(parameter != null ? parameter : "?");

            return invalue.HasValue ? (int)(invalue.Value * mul) + " " + units : "N/A";
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
