using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HisRoyalRedness.com
{
    public class ApplicationModel : NotifyBase
    {
        public ApplicationModel()
        {
            _tabModels = new ReadOnlyCollection<TabModelBase>(new[] 
            {
                new DateTimeTabModel()
            });
        }

        public IEnumerable<TabModelBase> TabModels => _tabModels;
        readonly ReadOnlyCollection<TabModelBase> _tabModels;
    }
}
