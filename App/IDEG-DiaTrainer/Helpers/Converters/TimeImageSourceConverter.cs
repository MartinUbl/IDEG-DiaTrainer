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
    /// Converter of DateTime to image source - phase of the day
    /// </summary>
    public class TimeImageSourceConverter : IValueConverter
    {
        ImageSource src_night = ImageSource.FromFile("crescentmoon.png");
        ImageSource src_morning = ImageSource.FromFile("sunrise.png");
        ImageSource src_day = ImageSource.FromFile("cloudyday.png");
        ImageSource src_evening = ImageSource.FromFile("sunset.png");

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var dt = (DateTime)value;

            if (dt.Hour < 6 || dt.Hour >= 20)
                return src_night;
            else if (dt.Hour < 10)
                return src_morning;
            else if (dt.Hour < 17)
                return src_day;
            else
                return src_evening;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
