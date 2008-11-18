using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace DistDBMS.Network
{
    public enum ControlSitePacketTags
    {
        None,
        OK,
        Command,
        Object,
    }

    public class ControlSitePacket : NetworkPacket
    {
        public static ControlSitePacket NetworkPacketToControlSitePacket(NetworkPacket networkPacket)
        {
            switch ((ControlSitePacketTags)networkPacket.Tag)
            {
                case ControlSitePacketTags.OK:
                    {
                        return networkPacket.ToPacket<ControlSiteOKPacket>();
                    }
                case ControlSitePacketTags.Command:
                    {
                        return networkPacket.ToPacket<ControlSiteCommandPacket>();
                    }
                default:
                    System.Diagnostics.Debugger.Break();
                    break;

            }
            return null;
        }


    }

    public class ControlSiteOKPacket : ControlSitePacket
    {
        public override bool Encapsulate()
        {
            Tag = (byte)ControlSitePacketTags.OK;
            if (!base.Encapsulate())
                return false;
            return true;
        }
    }


    public class ControlSiteCommandPacket : ControlSitePacket
    {
        public string Command;

        public override bool Encapsulate()
        {
            Tag = (byte)ControlSitePacketTags.Command;
            if (!base.Encapsulate())
                return false;
            return WriteString(Command);
        }

        protected override void Unencapsulate()
        {
            base.Unencapsulate();
            Command = ReadString();
        }
    }

    public class ControlSiteObjectPacket : ControlSitePacket
    {
        public object Object;

        public override bool Encapsulate()
        {
            Tag = (byte)ControlSitePacketTags.Object;
            if (!base.Encapsulate())
                return false;
            return WriteObject(Object);
        }

        protected override void Unencapsulate()
        {
            base.Unencapsulate();
            Object = ReadObject();
        }
    }









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
            LocalSiteOKPacket packet = new LocalSiteOKPacket();
            packet.Encapsulate();
            SendPacket(packet);
        }

        override public void OnPacketArrived(NetworkPacket networkPacket)
        {
            ControlSitePacket csPacket = ControlSitePacket.NetworkPacketToControlSitePacket(networkPacket);

            if(csPacket is ControlSiteCommandPacket)
            {
                ControlSiteCommandPacket packet = (ControlSiteCommandPacket)csPacket;
                string[] args = packet.Command.Split(":".ToCharArray());
                if (args[0] == "Test")
                {
                    LocalSiteCommandPacket lsPacket = new LocalSiteCommandPacket();
                    lsPacket.SessionId = sessionId;

                    lsPacket.StepIndex = LocalSitePacket.StepIndexNone;
                    lsPacket.StepFromIndex = LocalSitePacket.StepIndexNone;
                    lsPacket.Command = "Test:Set:10";
                    lsPacket.Encapsulate();
                    GetLocalSiteClient("L1").SendPacket(lsPacket);
                    GetLocalSiteClient("L1").Packets.WaitAndRead();
                    Debug.WriteLine("recv reply for L1");



                    lsPacket.StepIndex = 0;
                    lsPacket.StepFromIndex = LocalSitePacket.StepIndexNone;
                    lsPacket.Command = "Test:Send:L2";
                    lsPacket.Encapsulate();
                    GetLocalSiteClient("L1").SendPacket(lsPacket);


                    //Thread.Sleep(1000);
                    
                    lsPacket.StepIndex = LocalSitePacket.StepIndexNone;
                    lsPacket.StepFromIndex = LocalSitePacket.StepIndexNone;
                    lsPacket.Command = "Test:Set:20";
                    lsPacket.Encapsulate();
                    GetLocalSiteClient("L2").SendPacket(lsPacket);
                    GetLocalSiteClient("L2").Packets.WaitAndRead();
                    Debug.WriteLine("recv reply for L2");


                    lsPacket.StepIndex = 1;
                    lsPacket.StepFromIndex = 0;
                    lsPacket.Command = "Test:Sub";
                    lsPacket.Encapsulate();
                    GetLocalSiteClient("L2").SendPacket(lsPacket);


                    lsPacket.StepIndex = 2;
                    lsPacket.StepFromIndex = 1;
                    lsPacket.Command = "Test:Return";
                    lsPacket.Encapsulate();
                    GetLocalSiteClient("L2").SendPacket(lsPacket);
                    GetLocalSiteClient("L2").Packets.WaitAndRead();
                    Debug.WriteLine("recv return for L2");

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
            Packets.Append(ControlSitePacket.NetworkPacketToControlSitePacket(packet));
        }

        public void SendCommand(string s)
        {
            ControlSiteCommandPacket packet = new ControlSiteCommandPacket();
            packet.Command = s;
            packet.Encapsulate();
            SendPacket(packet);
        }
    }
}
