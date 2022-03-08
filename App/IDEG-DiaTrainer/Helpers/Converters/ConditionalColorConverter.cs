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
    /// Converter of color input to output based on condition outcome
    /// </summary>
    public class ConditionalColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var cond = (Boolean)value;
            var param = (String)parameter;

            var res = param.Split('|');
            if (res.Length != 2)
                return KnownColor.Default;

            return cond ? res[0] : res[1];
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
