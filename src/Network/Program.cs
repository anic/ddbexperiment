﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime;
using DistDBMS.Network;
using System.IO;
using System.Threading;


/*
通信用的数据包：

  
  
                                                                            
                                                                     +------------->  
                                                                     |                [LocalSite] <---\
                                                                     |     +-------<    /|\            \ 
                                                                     |     |             |              \
                                                                     |     |             | P2PPacket     \
                 ServerClientPacket                  LocalSitePacket |     |             |               |
                 ------------------>               >-----------------+     |            \|/              | P2PPacket
       [Client]                      [ControlSite]                    \    |          [LocalSite]        |
                 <-----------------                <-------------------\---+            /|\              |
                  ServerClientPacket                 ServerClientPacket \   \            |               /
                                                                         \   \           | P2PPacket    /
                                                                          \   \          |             /
                                                                           \   +---->   \|/           /
                                                                            \          [LocalSite]<--"
                                                                             +------<

  

 * 每类Packet都有对应的派生的TextPacket(纯字符串)和TextObjectPacket(字符串+二进制对象)
 * 数据包限制：1024*1024*1024 ，大约1G
  
  
*/

namespace DistDBMS
{
    /*
    class B 
    {
        private int a = 0;
    }
    class D : B
    {
        public int a;
    }
    */

    class Program
    {

        static void Main(string[] args)
        {
            //D d = new D();

            ClusterConfiguration clusterConfig = new ClusterConfiguration();

            clusterConfig.Hosts["C1"]["Host"] = "127.0.0.1";
            clusterConfig.Hosts["C1"]["Port"] = 10000;

            clusterConfig.Hosts["L1"]["Host"] = "127.0.0.1";
            clusterConfig.Hosts["L1"]["Port"] = 20000;
            clusterConfig.Hosts["L1"]["P2PPort"] = 21000;

            clusterConfig.Hosts["L2"]["Host"] = "127.0.0.1";
            clusterConfig.Hosts["L2"]["Port"] = 30000;
            clusterConfig.Hosts["L2"]["P2PPort"] = 31000;

            LocalSiteServer localSiteServer1 = new LocalSiteServer(clusterConfig, "L1");
            LocalSiteServer localSiteServer2 = new LocalSiteServer(clusterConfig, "L2");
            ControlSiteServer controlSiteServer = new ControlSiteServer(clusterConfig, "C1");




            localSiteServer1.Start();
            localSiteServer2.Start();
            controlSiteServer.Start();

            Thread.Sleep(500);

            ControlSiteClient controlSiteClient = new ControlSiteClient();
            controlSiteClient.Connect((string)clusterConfig.Hosts["C1"]["Host"], (int)clusterConfig.Hosts["C1"]["Port"]);
            while (true)
            {
                string s = Console.ReadLine();

                /*
                controlSiteClient.SendCommand("Test:Set:L1:10");
                controlSiteClient.Packets.WaitAndRead();

                controlSiteClient.SendCommand("Test:Set:L2:20");
                controlSiteClient.Packets.WaitAndRead();

                controlSiteClient.SendCommand("Test:Sub:L1:L2");
                controlSiteClient.Packets.WaitAndRead();

                controlSiteClient.SendCommand("Test:Move:L1:L2");
                controlSiteClient.Packets.WaitAndRead();
                */
                controlSiteClient.SendCommand("Test");
                ServerClientPacket csPacket = ServerClientPacket.NetworkPacketToServerClientPacket(controlSiteClient.Packets.WaitAndRead());
                if (csPacket is ServerClientTextObjectPacket)
                {
                    Debug.WriteLine((csPacket as ServerClientTextObjectPacket).Object.ToString());
                }
                //+ "Test:Return:L2\n";
            }

            //NetworkPacket p = NetworkPacket.CreatePacketFromObject("asdf");
            //string s = (string)NetworkPacket.GetObjectFromPacket(p);
            //Dirty, threads running;
            //Environment.Exit(0);
        }
    }
}