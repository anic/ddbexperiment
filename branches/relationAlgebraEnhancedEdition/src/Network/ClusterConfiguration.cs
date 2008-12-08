using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DistDBMS.Network
{
    public class ClusterConfiguration
    {
        public class HostConfiguration : Dictionary<string, Dictionary<string, object>>
        {
            new public Dictionary<string, object> this[string name]
            {
                get
                {
                    if (!this.ContainsKey(name))
                        base[name] = new Dictionary<string, object>();
                    return base[name];
                }
                set
                {
                    base[name] = value;
                }
            }

        }
        HostConfiguration hosts = new HostConfiguration();

        public HostConfiguration Hosts { get { return hosts; } }

    }
}
