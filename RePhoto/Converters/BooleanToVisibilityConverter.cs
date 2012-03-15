using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace RePhoto.Converters {
    public class BooleanToVisibilityConverter : IValueConverter {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            return System.Convert.ToBoolean(value) ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            return value.Equals(Visibility.Visible);
        }

        #endregion
    }
}