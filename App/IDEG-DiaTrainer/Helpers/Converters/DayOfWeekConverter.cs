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
    /// Converter of DateTime to day of week string
    /// </summary>
    public class DayOfWeekConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var type = (DateTime)value;

            switch (type.DayOfWeek)
            {
                case DayOfWeek.Monday: return "Monday";
                case DayOfWeek.Tuesday: return "Tuesday";
                case DayOfWeek.Wednesday: return "Wednesday";
                case DayOfWeek.Thursday: return "Thursday";
                case DayOfWeek.Friday: return "Friday";
                case DayOfWeek.Saturday: return "Saturday";
                case DayOfWeek.Sunday: return "Sunday";
            }

            return "Unknown";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
