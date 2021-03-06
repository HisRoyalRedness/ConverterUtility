﻿using NodaTime;
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
            var g1 = DateTime.MaxValue;
            var g2 = DateTime.MinValue;
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

    public class LocalTimeConverter : PreferenceConverterBase<LocalDateTime>
    {
        protected override string Convert(LocalDateTime value, object parameter, CultureInfo culture)
        {
            return value.ToString(Preference.DateTimeFormat, culture.DateTimeFormat);
        }

        protected override LocalDateTime ConvertBack(string value, object parameter, CultureInfo culture)
        {
            return SystemClock.Instance.GetCurrentInstant().InUtc().LocalDateTime;
        }
    }

    public class LocalTimeToDateTimeConverter : PreferenceConverterBase<LocalDateTime, DateTime?, object>
    {
        protected override DateTime? Convert(LocalDateTime value, object parameter, CultureInfo culture)
            => value.ToDateTimeUnspecified();

        protected override LocalDateTime ConvertBack(DateTime? value, object parameter, CultureInfo culture)
            => value.HasValue
                ? LocalDateTime.FromDateTime(value.Value)
                : new LocalDateTime();
    }

    public class OffsetConverter : PreferenceConverterBase<Offset>
    {
        protected override string Convert(Offset value, object parameter, CultureInfo culture)
        {
            return value.ToString(Preference.OffsetFormat, culture.DateTimeFormat);
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
