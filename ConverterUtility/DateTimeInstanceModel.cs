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
        LocalDateTime LocalTime { get; }

        bool IsRemovable { get; }
    }

    public abstract class TimeInstanceModelBase : NotifyBase, ITimeInstanceModel, IDisposable
    {
        #region Constructors
        protected TimeInstanceModelBase()
            : this(null, DateTimeZone.Utc)
        { }

        protected TimeInstanceModelBase(CommonInstant instant)
            : this(instant, DateTimeZone.Utc)
        { }

        protected TimeInstanceModelBase(DateTimeZone timeZone)
            : this(null, timeZone)
        { }

        protected TimeInstanceModelBase(CommonInstant instant, DateTimeZone timeZone)
        {
            _instant = instant ?? CommonInstant.Default;
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

        #region ITimeInstanceModel implementation
        public CommonInstant Instant => _instant;

        public DateTimeZone TimeZone
        {
            get { return _timeZone; }
            set { SetProperty(ref _timeZone, value, _ => UpdateLocalTime(_instant.Instant)); }
        }
        DateTimeZone _timeZone = DateTimeZone.Utc;

        public virtual LocalDateTime LocalTime
        {
            get { return GetInstantInLocal; }
            set { SetInstant(value); }
        }        

        public virtual bool IsRemovable => false;
        #endregion ITimeInstanceModel implementation

        protected LocalDateTime GetInstantInLocal
            => _instant.Instant.InZone(TimeZone).LocalDateTime;

        protected void SetInstant(Instant instant)
            => _instant.Instant = instant;

        protected void SetInstant(LocalDateTime localTime)
            => SetInstant(localTime.InZoneLeniently(TimeZone).ToInstant());

        void Instant_PropertyChanged(object sender, PropertyChangedEventArgs e)
            => UpdateLocalTime(_instant.Instant);

        protected virtual void UpdateLocalTime(Instant instant)
            => NotifyPropertyChanged(nameof(LocalTime));

        public ReadOnlyCollection<DateTimeZone> BclTimeZones => _bclZones;
        public ReadOnlyCollection<DateTimeZone> TzdbTimeZones => _tzdbZones;

        static readonly ReadOnlyCollection<DateTimeZone> _bclZones = DateTimeZoneProviders.Bcl.GetZones().ToList().AsReadOnly();
        static readonly ReadOnlyCollection<DateTimeZone> _tzdbZones = DateTimeZoneProviders.Tzdb.GetZones().ToList().AsReadOnly();

        readonly CommonInstant _instant;
    }

    public class UtcInstanceModel : TimeInstanceModelBase
    {
        #region Constructors
        public UtcInstanceModel(CommonInstant instant = null)
            : base(instant, DateTimeZone.Utc)
        { }
        #endregion Constructors

        public Offset LocalTimeOffset => Offset.Zero;
    }

    public class TimeInstanceModel : TimeInstanceModelBase
    {
        #region Constructors
        public TimeInstanceModel(CommonInstant instant = null, Action<TimeInstanceModel> removeTimeInstance = null)
            : this(instant, DateTimeZoneProviders.Bcl.GetSystemDefault(), removeTimeInstance)
        { }

        public TimeInstanceModel(Action<TimeInstanceModel> removeTimeInstance)
            : this(null, DateTimeZoneProviders.Bcl.GetSystemDefault(), removeTimeInstance)
        { }

        public TimeInstanceModel(DateTimeZone timeZone, Action<TimeInstanceModel> removeTimeInstance = null)
            : this(null, timeZone, removeTimeInstance)
        { }

        public TimeInstanceModel(CommonInstant instant, DateTimeZone timeZone, Action<TimeInstanceModel> removeTimeInstance = null)
            : base(instant, timeZone)
        {
            _removable = removeTimeInstance != null;
            _removeTimeInstance = new RelayCommand(_ => removeTimeInstance(this));
        }
        #endregion Constructors

        public Offset LocalTimeOffset => _zonedDateTime.Offset;
        ZonedDateTime _zonedDateTime;

        public override bool IsRemovable => _removable;
        readonly bool _removable;

        protected override void UpdateLocalTime(Instant instant)
        {
            _zonedDateTime = instant.InZone(TimeZone);
            NotifyPropertyChanged(nameof(LocalTimeOffset), nameof(LocalTime));
        }

        public RelayCommand RemoveInstance => _removeTimeInstance;
        readonly RelayCommand _removeTimeInstance;
    }

    public class EpochInstanceModel : TimeInstanceModelBase
    {
        #region Constructors
        public EpochInstanceModel(CommonInstant instant = null)
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

    public class PlaceholderInstanceModel : TimeInstanceModelBase
    {
        #region Constructors
        public PlaceholderInstanceModel(Action addNew)
        {
            _addNewTimeInstance = new NotifyBase<object>.RelayCommand(_ => addNew());
        }
        #endregion Constructors

        public RelayCommand AddNewInstance => _addNewTimeInstance;
        readonly RelayCommand _addNewTimeInstance;
    }
}
