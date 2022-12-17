using IDEG_DiaTrainer.Enums;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
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
    public class EnabledColorConverter : IValueConverter
    {
        private static readonly Color PositiveColor = Color.FromArgb("512bdf");
        private static readonly Color NegativeColor = Color.FromArgb("888888");

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var cond = (Boolean)value;
            return cond ? PositiveColor : NegativeColor;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
