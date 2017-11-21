using NodaTime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace HisRoyalRedness.com
{
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

        #region Singleton
        static CommonInstant()
        {
            _instance = new Lazy<CommonInstant>(() => new CommonInstant());
        }

        public static CommonInstant Default => _instance.Value;
        static readonly Lazy<CommonInstant> _instance;
        #endregion Singleton

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
