using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace DistDBMS.Network
{






    public class ControlSiteServerConnection : GenericServerConnection
    {
        Dictionary<string, LocalSiteClient> localSiteClients = new Dictionary<string, LocalSiteClient>();
        Guid sessionId = Guid.NewGuid();

        public LocalSiteClient GetLocalSiteClient(string name)
        {
            lock (localSiteClients)
            {
                if (!localSiteClients.ContainsKey(name))
                {
                    LocalSiteClient client = new LocalSiteClient();
                    localSiteClients[name] = client;

                    ControlSiteServer server = (ControlSiteServer)genericServer;
                    client.Connect((string)server.ClusterConfig.Hosts[name]["Host"], (int)server.ClusterConfig.Hosts[name]["Port"]);
                }

                return localSiteClients[name];
            }
        }


        override public void Start()
        {
            base.Start();
        }

        void SendOKPacket()
        {
            ServerClientTextPacket packet = new ServerClientTextPacket();
            packet.Encapsulate();
            SendPacket(packet);
        }

        override public void OnPacketArrived(NetworkPacket networkPacket)
        {
            ServerClientPacket csPacket = ServerClientPacket.NetworkPacketToServerClientPacket(networkPacket);

            if (csPacket is ServerClientTextPacket)
            {
                ServerClientTextPacket packet = (ServerClientTextPacket)csPacket;
                string[] args = packet.Text.Split(":".ToCharArray());
                if (args[0] == "Test")
                {
                    LocalSiteServerTextPacket lsPacket = new LocalSiteServerTextPacket();
                    lsPacket.SessionId = sessionId;

                    lsPacket.StepIndex = LocalSiteServerPacket.StepIndexNone;
                    lsPacket.StepFromIndex = LocalSiteServerPacket.StepIndexNone;
                    lsPacket.Text = "Test:Set:10";
                    lsPacket.Encapsulate();
                    GetLocalSiteClient("L1").SendPacket(lsPacket);
                    GetLocalSiteClient("L1").Packets.WaitAndRead();
                    Debug.WriteLine("recv reply for L1");


                    //Thread.Sleep(2000);


                    lsPacket.StepIndex = 0;
                    lsPacket.StepFromIndex = LocalSiteServerPacket.StepIndexNone;
                    lsPacket.Text = "Test:Send:L2";
                    lsPacket.Encapsulate();
                    GetLocalSiteClient("L1").SendPacket(lsPacket);


                    
                    
                    lsPacket.StepIndex = LocalSiteServerPacket.StepIndexNone;
                    lsPacket.StepFromIndex = LocalSiteServerPacket.StepIndexNone;
                    lsPacket.Text = "Test:Set:20";
                    lsPacket.Encapsulate();
                    GetLocalSiteClient("L2").SendPacket(lsPacket);
                    GetLocalSiteClient("L2").Packets.WaitAndRead();
                    Debug.WriteLine("recv reply for L2");


                    lsPacket.StepIndex = 1;
                    lsPacket.StepFromIndex = 0;
                    lsPacket.Text = "Test:Sub";
                    lsPacket.Encapsulate();
                    GetLocalSiteClient("L2").SendPacket(lsPacket);


                    lsPacket.StepIndex = 2;
                    lsPacket.StepFromIndex = 1;
                    lsPacket.Text = "Test:Return";
                    lsPacket.Encapsulate();
                    GetLocalSiteClient("L2").SendPacket(lsPacket);
                    GetLocalSiteClient("L2").Packets.WaitAndRead();
                    Debug.WriteLine("recv return for L2");


                    ServerClientTextObjectPacket clientPacket = new ServerClientTextObjectPacket();
                    clientPacket.Object = "Hello From ControlServer";
                    clientPacket.Encapsulate();
                    SendPacket(clientPacket);
                }
                else
                {
                    System.Diagnostics.Debugger.Break();
                }
            }
        }
    }

    public class ControlSiteServer : GenericServer<ControlSiteServerConnection>
    {
        public ClusterConfiguration ClusterConfig;

        public string Name { get; set; }
        public ControlSiteServer(ClusterConfiguration clusterConfig, string name)
        {
            this.ClusterConfig = clusterConfig;
            Name = name;
        }
        public void Start()
        {
            base.Start((int)ClusterConfig.Hosts[Name]["Port"]);
        }
    }

    public class ControlSiteClient : GenericClientConnection
    {
        public PacketQueue Packets = new PacketQueue();

        override public void OnPacketArrived(NetworkPacket packet)
        {
            Packets.Append(ServerClientPacket.NetworkPacketToServerClientPacket(packet));
        }

        public void SendCommand(string s)
        {
            ServerClientTextPacket packet = new ServerClientTextPacket();
            packet.Text = s;
            packet.Encapsulate();
            SendPacket(packet);
        }
    }
}
