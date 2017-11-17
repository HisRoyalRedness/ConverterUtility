using NodaTime;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace HisRoyalRedness.com
{
    public interface ITimeInstanceModel
    {
        CommonInstant Instant { get; }
        DateTimeZone TimeZone { get; set; }
    }

    public abstract class TimeInstanceModelBase : NotifyBase, ITimeInstanceModel, IDisposable
    {
        #region Constructors
        protected TimeInstanceModelBase(CommonInstant instant, DateTimeZone timeZone)
        {
            _instant = instant;
            _timeZone = timeZone;
            _instant.PropertyChanged += Instant_PropertyChanged;
            UpdateLocalTime(instant?.Instant ?? new NodaTime.Instant());
        }
        #endregion Constructors

        #region IDisposable implementation
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    Instant.PropertyChanged -= Instant_PropertyChanged;
                }
                _disposed = true;
            }
        }

        public void Dispose() => Dispose(true);

        bool _disposed = false;
        #endregion IDisposable implementation

        public CommonInstant Instant => _instant;

        public DateTimeZone TimeZone
        {
            get { return _timeZone; }
            set { SetProperty(ref _timeZone, value, _ => UpdateLocalTime(_instant.Instant)); }
        }
        DateTimeZone _timeZone = DateTimeZone.Utc;

        void Instant_PropertyChanged(object sender, PropertyChangedEventArgs e)
            => UpdateLocalTime(_instant.Instant);

        protected abstract void UpdateLocalTime(Instant instant);

        public ReadOnlyCollection<DateTimeZone> BclTimeZones => _bclZones;
        public ReadOnlyCollection<DateTimeZone> TzdbTimeZones => _tzdbZones;

        static readonly ReadOnlyCollection<DateTimeZone> _bclZones = DateTimeZoneProviders.Bcl.GetZones().ToList().AsReadOnly();
        static readonly ReadOnlyCollection<DateTimeZone> _tzdbZones = DateTimeZoneProviders.Tzdb.GetZones().ToList().AsReadOnly();

        readonly CommonInstant _instant;
    }

    public class UtcInstanceModel : TimeInstanceModelBase
    {
        #region Constructors
        public UtcInstanceModel(CommonInstant instant)
            : base(instant, DateTimeZone.Utc)
        { }
        #endregion Constructors

        public LocalDateTime LocalTime => Instant.Instant.InUtc().LocalDateTime;
        public Offset LocalTimeOffset => Offset.Zero;
        protected override void UpdateLocalTime(Instant instant) => NotifyPropertyChanged(nameof(LocalTime));
    }

    public class TimeInstanceModel : TimeInstanceModelBase
    {
        #region Constructors
        public TimeInstanceModel(CommonInstant instant)
            : this(instant, DateTimeZoneProviders.Bcl.GetSystemDefault())
        { }

        public TimeInstanceModel(CommonInstant instant, DateTimeZone timeZone)
            : base(instant, timeZone)
        { }
        #endregion Constructors

        public LocalDateTime LocalTime
        {
            get { return _zonedDateTime.LocalDateTime; }
            set { SetProperty(ref _localTime, value); }
        }
        LocalDateTime _localTime;

        public Offset LocalTimeOffset => _zonedDateTime.Offset;

        protected override void UpdateLocalTime(Instant instant)
        {
            _zonedDateTime = instant.InZone(TimeZone);
            NotifyPropertyChanged(nameof(LocalTime), nameof(LocalTimeOffset));
        }

        ZonedDateTime _zonedDateTime;
    }

    public class EpochInstanceModel : TimeInstanceModelBase
    {
        #region Constructors
        public EpochInstanceModel(CommonInstant instant)
            : base(instant, DateTimeZone.Utc)
        { }
        #endregion Constructors

        public Int64 EpochSeconds
        {
            get { return Instant.Instant.ToUnixTimeSeconds(); }
            set { Instant.Instant = NodaTime.Instant.FromUnixTimeSeconds(value); }
        }

        protected override void UpdateLocalTime(Instant instant)
        {
            NotifyPropertyChanged(nameof(EpochSeconds));
        }
    }
}
