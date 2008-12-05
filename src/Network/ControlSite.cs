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

        public Guid SessionId { get { return sessionId; } }
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

            (genericServer as ControlSiteServer).PacketProcessor(this, csPacket);
        }
    }

    public class ControlSiteServer : GenericServer<ControlSiteServerConnection>
    {
        public ClusterConfiguration ClusterConfig;

        public delegate void PacketProcessorDelegate(ControlSiteServerConnection conn, ServerClientPacket packet);
        public PacketProcessorDelegate PacketProcessor;

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
