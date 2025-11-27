using System;
using System.Globalization;
using System.Windows.Data;

namespace LocationSimulator_WPF
{
    public class InvertBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // summery:
            // if paramter called "value" ,  "is bool" - is a boolean value.
            // enter the value of "value " parameter, into the variable "boolean".

            if (value is bool boolean)
            {
                return !boolean;
                // return !valoue;  Does not compile!  Because "value" is of type object and not of type bool.
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