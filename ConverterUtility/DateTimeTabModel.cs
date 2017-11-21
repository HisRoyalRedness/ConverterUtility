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
            InstanceModels.Add(new UtcInstanceModel());
            InstanceModels.Add(new EpochInstanceModel());
            InstanceModels.Add(new TimeInstanceModel());
            //InstanceModels.Add(new TimeInstanceModel(DateTimeZoneProviders.Tzdb.GetZoneOrNull("Africa/Johannesburg"), RemoveInstance));
            InstanceModels.Add(new PlaceholderInstanceModel(AddInstance));
        }

        void AddInstance() => InstanceModels.Insert(InstanceModels.Count - 1, new TimeInstanceModel(RemoveInstance));
        void RemoveInstance(TimeInstanceModel model) => InstanceModels.Remove(model);

        public ObservableCollection<ITimeInstanceModel> InstanceModels { get; } = new ObservableCollection<ITimeInstanceModel>();

        public CommonInstant Instant => _instant;
        readonly CommonInstant _instant = CommonInstant.Default;
    }
}
