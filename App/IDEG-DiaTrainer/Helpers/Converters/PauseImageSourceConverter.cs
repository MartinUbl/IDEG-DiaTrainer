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
    /// Converter of DateTime to image source - pause/resume the simulation
    /// </summary>
    public class PauseImageSourceConverter : IValueConverter
    {
        ImageSource src_pause = ImageSource.FromFile("pause.png");
        ImageSource src_play = ImageSource.FromFile("play.png");

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var dt = (bool)value;

            return dt ? src_play : src_pause;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
