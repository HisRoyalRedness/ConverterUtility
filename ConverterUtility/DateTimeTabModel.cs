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
            InstanceModels.Add(new UtcInstanceModel(Instant));
            InstanceModels.Add(new TimeInstanceModel(Instant));
            InstanceModels.Add(new TimeInstanceModel(Instant, DateTimeZoneProviders.Tzdb.GetZoneOrNull("Africa/Johannesburg")));
            InstanceModels.Add(new EpochInstanceModel(Instant));

            var tzdbZones = DateTimeZoneProviders.Tzdb.GetZones().ToList();
            var bclZones = DateTimeZoneProviders.Bcl.GetZones().ToList();
            Console.WriteLine();
        }

        public ObservableCollection<ITimeInstanceModel> InstanceModels { get; } = new ObservableCollection<ITimeInstanceModel>();        

        public CommonInstant Instant
        {
            get { return _instant   ; }
            set { SetProperty(ref _instant, value); }
        }
        CommonInstant _instant = new CommonInstant();
    }

    public class CommonInstant : NotifyBase
    {
        public CommonInstant()
        {
            _tickTimer = new DispatcherTimer()
            {
                Interval = TimeSpan.FromMilliseconds(200)
            };
            _tickTimer.Tick += (o, e) => Update();
        }

        public Instant Instant
        {
            get { return _instant; }
            set { SetProperty(ref _instant, value); }
        }
        Instant _instant = SystemClock.Instance.GetCurrentInstant();

        public bool AutoUpdate
        {
            get { return _autoUpdate; }
            set { SetProperty(ref _autoUpdate, value, au => _tickTimer.IsEnabled = au); }
        }
        bool _autoUpdate = false;

        public void Update()
        {
            Instant = SystemClock.Instance.GetCurrentInstant();
        }

        readonly DispatcherTimer _tickTimer;
    }

    internal static class TabModelExtensions
    {
        internal static IEnumerable<DateTimeZone> GetZones(this IDateTimeZoneProvider provider)
            => provider.Ids.Select(id => provider.GetZoneOrNull(id)).Where(z => z != null);
    }
}
