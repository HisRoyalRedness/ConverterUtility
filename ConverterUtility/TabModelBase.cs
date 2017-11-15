using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HisRoyalRedness.com
{
    public abstract class TabModelBase : NotifyBase
    {
        protected TabModelBase(string header)
        {
            Header = header;
        }

        public string Header { get; private set; }        
    }
}
