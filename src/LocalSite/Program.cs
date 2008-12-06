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
        class PackageProcessor
        {
            public void LocalSitePackageProcess(LocalSiteServerConnection conn, LocalSiteServerPacket packet)
            {
                System.Console.WriteLine("packet received");
                int a = 0;
            }

            public void P2PPackageProcess(LocalSiteServerConnection conn, P2PPacket packet)
            {
                int b = 0;
                System.Console.WriteLine("packet received");
            }
        }

        static void Main(string[] args)
        {
            if (args == null || args.Length == 0)
                return;

            NetworkInitiator initiator = new NetworkInitiator();
            ClusterConfiguration clusterConfig = initiator.GetConfiguration(args[0]);

            LocalSiteServer localSiteServer = new LocalSiteServer(clusterConfig, args[1]);
            PackageProcessor processor = new PackageProcessor();
            localSiteServer.LocalSitePacketProcessor = new LocalSiteServer.LocalSitePacketProcessorDelegate(processor.LocalSitePackageProcess);
            localSiteServer.P2PPacketProcessor = new LocalSiteServer.P2PPacketProcessorDelegate(processor.P2PPackageProcess);

            localSiteServer.Start();

            System.Console.WriteLine("LocalSite " + args[1] + " started!");
            //while (true)
            //{
            //    try { Thread.Sleep(500); }
            //    catch { }
            //}

            Console.ReadLine();
        }
    }
}
