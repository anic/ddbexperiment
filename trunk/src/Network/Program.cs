using System;
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

  

 * 每类Packet都有对应的派生的TextPacket(纯字符串)和TextObjectPacket(字符串+二进制对象)。限制：字符串64k以内，二进制对象1G以内
 * 数据包限制：1024*1024*1024 ，大约1G   
 * 
  
  
*/

namespace DistDBMS
{
    class Program
    {

        class LocalSiteState
        {
            public int TestValue;
            public int TempValue;
        }
        static private void LocalSiteConnectionStart(GenericServerConnection conn)
        {
            Debug.WriteLine("New LocalSiteServer Connection");
            conn.State = new LocalSiteState();
        }

        static private void LocalSiteP2PPacketProcessor(LocalSiteServerConnection conn, P2PPacket packet)
        {
            if(packet is P2PTextPacket)
            {
                P2PTextPacket textPacket = (packet as P2PTextPacket);
                Debug.WriteLine(conn.Server.Name + " P2P " + textPacket.Text);

                string[] args = textPacket.Text.Split(":".ToCharArray());
                if (args[0] == "Test")
                {
                    if (args[1] == "Set")
                    {
                        (conn.State as LocalSiteState).TempValue = int.Parse(args[2]);
                    }
                    else
                    {
                        System.Diagnostics.Debugger.Break();
                    }
                }

            }
        }

        static private void LocalSitePacketProcessor(LocalSiteServerConnection conn, LocalSiteServerPacket packet)
        {
            if (packet is LocalSiteServerTextPacket)
            {
                LocalSiteServerTextPacket textPacket = (packet as LocalSiteServerTextPacket);
                Debug.WriteLine(conn.Server.Name + " LCL " + textPacket.Text);

                string[] args = textPacket.Text.Split(":".ToCharArray());
                if (args[0] == "Test")
                {
                    if (args[1] == "Set")
                    {
                        (conn.State as LocalSiteState).TestValue = int.Parse(args[2]);
                        conn.SendServerClientTextPacket("");
                    }
                    else if (args[1] == "Send")
                    {
                        string text = "Test:Set:" + (conn.State as LocalSiteState).TestValue.ToString();
                        conn.SendP2PStepTextPacket(args[2], packet.StepIndex, text);
                        Debug.WriteLine(conn.Server.Name + " send P2P " + text);

                    }
                    else if (args[1] == "Sub")
                    {
                        (conn.State as LocalSiteState).TestValue -= (conn.State as LocalSiteState).TempValue;
                    }
                    else if (args[1] == "Return")
                    {
                        Debug.WriteLine(conn.Server.Name + " Return " + (conn.State as LocalSiteState).TestValue.ToString());
                        conn.SendServerClientTextPacket((conn.State as LocalSiteState).TestValue.ToString());
                    }
                    else
                    {
                        System.Diagnostics.Debugger.Break();
                    }
                }
            }
        }
        static private void ControlSitePacketProcessor(ControlSiteServerConnection conn, ServerClientPacket packet)
        {
            if (packet is ServerClientTextPacket)
            {
                ServerClientTextPacket textPacket = (ServerClientTextPacket)packet;
                string[] args = textPacket.Text.Split(":".ToCharArray());
                if (args[0] == "Test")
                {
                    
                    //测试乱序到达：先给L2发后续步骤，再向L1发前期步骤
                    conn.GetLocalSiteClient("L2").SendStepTextPacket(conn.SessionId, LocalSiteServerPacket.StepIndexNone, LocalSiteServerPacket.StepIndexNone, "Test:Set:20");
                    conn.GetLocalSiteClient("L2").Packets.WaitAndRead();
                    Debug.WriteLine("recv reply for L2");

                    conn.GetLocalSiteClient("L2").SendStepTextPacket(conn.SessionId, 0, 1, "Test:Sub");

                    conn.GetLocalSiteClient("L2").SendStepTextPacket(conn.SessionId, 1, 2, "Test:Return");


                    Thread.Sleep(2000);


                    conn.GetLocalSiteClient("L1").SendStepTextPacket(conn.SessionId, LocalSiteServerPacket.StepIndexNone, LocalSiteServerPacket.StepIndexNone, "Test:Set:11");
                    conn.GetLocalSiteClient("L1").Packets.WaitAndRead();
                    Debug.WriteLine("recv reply for L1");

                    conn.GetLocalSiteClient("L1").SendStepTextPacket(conn.SessionId, LocalSiteServerPacket.StepIndexNone, 0, "Test:Send:L2");
                    

                    
                    ServerClientTextPacket returnPacket = (ServerClientTextPacket)conn.GetLocalSiteClient("L2").Packets.WaitAndRead();
                    Debug.WriteLine("recv return for L2");

                    conn.SendServerClientTextObjectPacket(returnPacket.Text, "string object Hello From ControlServer");
                }
                else
                {
                    System.Diagnostics.Debugger.Break();
                }
            }

        }

        static void Main(string[] args)
        {
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
            localSiteServer1.ConnectionStart = LocalSiteConnectionStart;
            localSiteServer1.P2PPacketProcessor = LocalSiteP2PPacketProcessor;
            localSiteServer1.LocalSitePacketProcessor = LocalSitePacketProcessor;

            LocalSiteServer localSiteServer2 = new LocalSiteServer(clusterConfig, "L2");
            localSiteServer2.ConnectionStart = LocalSiteConnectionStart;
            localSiteServer2.P2PPacketProcessor = LocalSiteP2PPacketProcessor;
            localSiteServer2.LocalSitePacketProcessor = LocalSitePacketProcessor;

            ControlSiteServer controlSiteServer = new ControlSiteServer(clusterConfig, "C1");
            controlSiteServer.PacketProcessor = ControlSitePacketProcessor;

            localSiteServer1.Start();
            localSiteServer2.Start();
            controlSiteServer.Start();

            Thread.Sleep(500);

            ControlSiteClient controlSiteClient = new ControlSiteClient();
            controlSiteClient.Connect((string)clusterConfig.Hosts["C1"]["Host"], (int)clusterConfig.Hosts["C1"]["Port"]);
            while (true)
            {
                string s = Console.ReadLine();

                controlSiteClient.SendCommand("Test");
                ServerClientPacket csPacket = ServerClientPacket.NetworkPacketToServerClientPacket(controlSiteClient.Packets.WaitAndRead());
                if (csPacket is ServerClientTextObjectPacket)
                {
                    ServerClientTextObjectPacket packet = (csPacket as ServerClientTextObjectPacket);

                    Debug.WriteLine("Text:"+ packet.Text + ", Object:" + packet.Object.ToString());

                    Debug.WriteLine("Test Over.");
                }
            }

        }
    }
}
