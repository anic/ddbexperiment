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


        /// <summary>
        /// 
        /// </summary>
        /// <param name="args">第一个参数是初始化的脚本,第二个参数是想运行的LocalSite名称（如果不填，则启动所有LocalSite）</param>
        static void Main(string[] args)
        {
            //第一个参数是初始化的脚本
            string scriptFile = "NetworkInitScript.txt";

            if (args.Length >= 1)
                scriptFile = args[0];

            //args.Length >= 1
            NetworkInitiator initiator = new NetworkInitiator();
            ClusterConfiguration clusterConfig = initiator.GetConfiguration(scriptFile);

            
            //args.Length >=2 
            List<string> localSite = new List<string>();
            if (args.Length >= 2)
                for (int i = 1; i < args.Length; i++)
                    localSite.Add(args[i]);
            else //没有参数，启动所有
                localSite.AddRange(initiator.LocalSiteNames);


            foreach (string site in localSite)
            {
                LocalSiteServer localSiteServer = new LocalSiteServer(clusterConfig, site);

                PackageProcessor processor = new PackageProcessor(site);
                localSiteServer.LocalSitePacketProcessor = new LocalSiteServer.LocalSitePacketProcessorDelegate(processor.LocalSitePackageProcess);
                localSiteServer.P2PPacketProcessor = new LocalSiteServer.P2PPacketProcessorDelegate(processor.P2PPackageProcess);

                localSiteServer.Start();

                System.Console.WriteLine("LocalSite " + site + " started!");
            }

            Console.ReadLine();
        }
    }
}
