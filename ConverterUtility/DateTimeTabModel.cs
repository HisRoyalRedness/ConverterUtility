using NodaTime;
using NodaTime.TimeZones;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace HisRoyalRedness.com
{
    public class DateTimeTabModel : TabModelBase
    {
        public DateTimeTabModel() : base("Date/Time")
        {
            DateTimeInstances.Add(new DateTimeInstanceModel(Instant));
            DateTimeInstances.Add(new DateTimeInstanceModel(Instant, DateTimeZone.Utc));

            _tickTimer = new DispatcherTimer();
            _tickTimer.Interval = TimeSpan.FromMilliseconds(200);
            _tickTimer.Tick += (o, e) => Instant.Refresh();
        }

        public ObservableCollection<DateTimeInstanceModel> DateTimeInstances { get; } = new ObservableCollection<DateTimeInstanceModel>();


        public CommonInstant Instant
        {
            get { return _instant   ; }
            set { SetProperty(ref _instant, value); }
        }
        CommonInstant _instant = new CommonInstant();


        public bool UseCurrentTime
        {
            get { return _useCurrentTime; }
            set { SetProperty(ref _useCurrentTime, value, uct => _tickTimer.IsEnabled = uct); }
        }
        bool _useCurrentTime = false;

        readonly DispatcherTimer _tickTimer;
    }

    public class CommonInstant : NotifyBase
    {
        public Instant Instant
        {
            get { return _instant; }
            set { SetProperty(ref _instant, value); }
        }
        Instant _instant = SystemClock.Instance.GetCurrentInstant();

        public void Refresh()
        {
            Instant = SystemClock.Instance.GetCurrentInstant();
        }
    }
}
