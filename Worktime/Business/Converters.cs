using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using WpfUtility;

namespace Worktime.Business
{
    /// <inheritdoc />
    /// <summary>
    ///     Converts a timespan to a string in the format hh:mm
    ///     Or trys to parse the string into a timespan (loss of seconds, days etc.)
    /// </summary>
    public class TimeSpanConverter : IValueConverter
    {
        /// <inheritdoc />
        /// <summary>
        ///     Converts a timespan to a string in the format hh:mm
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is TimeSpan ts)
            {
                return $@"{(int) ts.TotalHours:00}:{ts.Minutes:00}";
            }

            return new TimeSpan().ToString(@"hh\:mm");
        }

        /// <inheritdoc />
        /// <summary>
        ///     Trys to parse the string into a timespan (loss of seconds, days etc.)
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var result = new TimeSpan();
            if (value != null) TimeSpan.TryParse(value.ToString(), out result);
            return result;
        }
    }

    /// <summary>
    ///     Converts true to Visibility.Collapsed or false to Visibility.Visible
    /// </summary>
    public sealed class NegatedBooleanToVisibilityConverter : BooleanConverter<Visibility>
    {
        /// <summary>
        ///     Converts true to Visibility.Collapsed or false to Visibility.Visible
        /// </summary>
        public NegatedBooleanToVisibilityConverter() :
            base(Visibility.Collapsed, Visibility.Visible)
        {
        }
    }

    /// <inheritdoc />
    /// <summary>
    /// </summary>
    public sealed class IsEqualOrGreaterThanConverter : IValueConverter
    {
        /// <inheritdoc />
        /// <summary>
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || parameter == null) return false;

            if (decimal.TryParse(value.ToString(), out decimal valueParsed) &&
                decimal.TryParse(parameter.ToString(), out decimal parameterParsed))
            {
                return valueParsed >= parameterParsed;
            }

            return false;
        }

        /// <inheritdoc />
        /// <summary>
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <inheritdoc />
    /// <summary>
    /// </summary>
    public class DateTimeToTimeSpanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DateTime dt)
            {
                return new TimeSpan(dt.Hour, dt.Minute, dt.Second);
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is TimeSpan ts)
            {
                var now = DateTime.Now;
                return new DateTime(now.Year, now.Month, now.Day, ts.Hours, ts.Minutes, ts.Seconds);
            }

            return null;
        }
    }

    /// <inheritdoc />
    /// <summary>
    /// </summary>
    public class DateTimeToDateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DateTime dt)
            {
                return new DateTime(dt.Year, dt.Month, dt.Day);
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DateTime dt)
            {
                return dt;
            }
            return null;
        }
    }
}
