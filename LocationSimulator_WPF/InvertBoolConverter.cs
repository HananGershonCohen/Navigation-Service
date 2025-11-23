using System;
using System.Globalization;
using System.Windows.Data;

// ודא שאתה עוטף את המחלקה ב-namespace של הפרויקט:
namespace LocationSimulator_WPF
{
    // ודא שהמחלקה היא Public! (תיקון משלב קודם)
    public class InvertBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolean)
            {
                return !boolean;
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolean)
            {
                return !boolean;
            }
            return value;
        }
    }
}