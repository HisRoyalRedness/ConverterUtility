using NodaTime;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace HisRoyalRedness.com
{
    public class DateTimeInstanceModel : NotifyBase, IDisposable
    {
        #region Constructors
        public DateTimeInstanceModel(CommonInstant instant)
            : this(instant, DateTimeZoneProviders.Bcl.GetSystemDefault())
        { }

        public DateTimeInstanceModel(CommonInstant instant, DateTimeZone dateTimeZone)
        {
            _dateTimeZone = dateTimeZone;
            Instant = instant;
            Instant.PropertyChanged += Instant_PropertyChanged;
        }
        #endregion Constructors

        public DateTimeZone DateTimeZone
        {
            get { return _dateTimeZone; }
            set { SetProperty(ref _dateTimeZone, value, _ => NotifyPropertyChanged(nameof(ZonedDateTime))); }
        }
        DateTimeZone _dateTimeZone = DateTimeZone.Utc;

        public CommonInstant Instant { get; private set; }

        void Instant_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            NotifyPropertyChanged(nameof(ZonedDateTime));
        }

        public ZonedDateTime ZonedDateTime
        {
            get { return Instant.Instant.InZone(DateTimeZone); }
            set { Instant.Instant = value.ToInstant();  }
        }

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
    }
}
