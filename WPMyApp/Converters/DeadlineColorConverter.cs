using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace WpMyApp.Converters
{
    public class DeadlineColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int daysUntilDue)
            {
                // Просрочено
                if (daysUntilDue < 0)
                    return new SolidColorBrush(Colors.Red);

                // Сегодня
                if (daysUntilDue == 0)
                    return new SolidColorBrush(Colors.OrangeRed);

                // Завтра
                if (daysUntilDue == 1)
                    return new SolidColorBrush(Colors.Orange);

                // В течение недели
                if (daysUntilDue <= 7)
                    return new SolidColorBrush(Colors.Goldenrod);

                // Есть время
                return new SolidColorBrush(Colors.Green);
            }

            // Значение по умолчанию
            return new SolidColorBrush(Colors.Gray);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}