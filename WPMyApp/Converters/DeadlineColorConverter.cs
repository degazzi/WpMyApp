using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace WpMyApp.Converters
{
    public class DeadlineColorConverter : IValueConverter
    {
        public object Convert(object value, Type type, object param, CultureInfo culture)
        {
            if (value is int days)
            {
                if (days < 0) return Brushes.Red;
                if (days <= 3) return Brushes.Orange;
                return Brushes.LightGreen;
            }

            return Brushes.White;
        }

        public object ConvertBack(object value, Type t, object p, CultureInfo c) => null;
    }
}
