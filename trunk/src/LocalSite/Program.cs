using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SQLite;
using DistDBMS.LocalSite.DataAccess;
using System.IO;
using DistDBMS.Common.Dictionary;
using DistDBMS.Common.Table;
using DistDBMS.Network;
using System.Threading;

namespace DistDBMS.LocalSite
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args == null || args.Length == 0)
                return;

            NetworkInitiator initiator = new NetworkInitiator();
            ClusterConfiguration clusterConfig = initiator.GetConfiguration(args[0]);

            LocalSiteServer localSiteServer = new LocalSiteServer(clusterConfig, args[1]);
            localSiteServer.Start();

            System.Console.WriteLine("LocalSite " + args[1] + " started!");
            while (true)
            {
                try { Thread.Sleep(500); }
                catch { }
            }
        }
    }
}
