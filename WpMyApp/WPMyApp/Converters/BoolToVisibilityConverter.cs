using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace WpMyApp.Converters
{
    public class BoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool invert = false;
            if (parameter != null && bool.TryParse(parameter.ToString(), out var p))
                invert = p;

            bool b = false;
            if (value is bool vb) b = vb;
            if (invert) b = !b;

            return b ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Visibility vis)
            {
                return vis == Visibility.Visible;
            }
            return false;
        }
    }
}
