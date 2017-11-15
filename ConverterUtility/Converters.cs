using NodaTime;
using NodaTime.TimeZones;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace HisRoyalRedness.com
{
    public class ConverterPreference : NotifyBase
    {
        public ConverterPreference()
        {
            DateTimeFormat = $"{DateFormat} {TimeFormat}";
        }

        #region Instancing
        public static ConverterPreference Instance => _instance.Value;
        static readonly Lazy<ConverterPreference> _instance = new Lazy<ConverterPreference>(() => new ConverterPreference());
        #endregion Instancing

        public string DateFormat { get; set; } = "ddd, dd MMM yyyy";
        public string TimeFormat { get; set; } = "HH:mm:ss";
        public string DateTimeFormat { get; set; }
        public string OffsetFormat { get; set; } = "m";
    }

    #region PreferenceConverterBase
    public abstract class PreferenceConverterBase<TSource> : PreferenceConverterBase<TSource, string, object>
    {
        protected PreferenceConverterBase(bool isOneWay = false)
            : base(isOneWay)
        { }
    }

    public abstract class PreferenceConverterBase<TSource, TParameter> : PreferenceConverterBase<TSource, string, TParameter>
    {
        protected PreferenceConverterBase(bool isOneWay = false)
            : base(isOneWay)
        { }
    }

    public abstract class PreferenceConverterBase<TSource, TTarget, TParameter> : IValueConverter
    {
        protected PreferenceConverterBase(bool isOneWay = false)
        {
            _isOneWay = isOneWay;
        }

        protected abstract TTarget Convert(TSource value, TParameter parameter, CultureInfo culture);
        protected virtual TSource ConvertBack(TTarget value, TParameter parameter, CultureInfo culture) 
        { throw new NotSupportedException(); }

        #region IValueConverter implementation
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var typedSource = (TSource)value;
            var typedParameter = (TParameter)parameter;
            return Convert(typedSource, typedParameter, culture);
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (_isOneWay)
                throw new NotSupportedException();
            var typedTarget = (TTarget)value;
            var typedParameter = (TParameter)parameter;
            return ConvertBack(typedTarget, typedParameter, culture);
        }
        #endregion IValueConverter implementation

        #region Preferences
        protected ConverterPreference Preference => _preference.Value;
        static readonly Lazy<ConverterPreference> _preference = new Lazy<ConverterPreference>(() => ConverterPreference.Instance);
        #endregion Preferences

        readonly bool _isOneWay;
    }
    #endregion PreferenceConverterBase

    public class DateTimeConverter : PreferenceConverterBase<ZonedDateTime>
    {
        protected override string Convert(ZonedDateTime value, object parameter, CultureInfo culture)
        {
            return value.ToString(Preference.DateTimeFormat, culture.DateTimeFormat);
        }
    }

    public class DateTimeZoneConverter : PreferenceConverterBase<DateTimeZone>
    {
        protected override string Convert(DateTimeZone value, object parameter, CultureInfo culture)
        {
            var bclTimeZone = value as BclDateTimeZone;
            return bclTimeZone == null ? value.ToString() : bclTimeZone.DisplayName;
        }
    }
}
