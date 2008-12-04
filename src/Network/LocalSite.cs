using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace DistDBMS.Network
{

    public enum P2PPacketTags
    {
        None,
        Text,
        TextObject,
    }

    public class SessionStepPacket : NetworkPacket
    {
        public const int StepIndexNone = -1;


        public int StepIndex;
        public Guid SessionId;

        public override bool Encapsulate()
        {
            if (!base.Encapsulate())
                return false;

            return  WriteInt(StepIndex) && WriteGuid(SessionId);
        }

        protected override void Unencapsulate()
        {
            base.Unencapsulate();
            StepIndex = ReadInt();
            SessionId = ReadGuid();
        }

        public string FromHostName { get; set; }
    }

    public class P2PPacket : SessionStepPacket
    {
        new public byte Tag;

        public override bool Encapsulate()
        {
            base.Tag = (byte)GenericP2PPacketTags.Application;
            if (!base.Encapsulate())
                return false;

            return WriteByte(Tag);
        }

        protected override void Unencapsulate()
        {
            base.Unencapsulate();
            Tag = ReadByte();
        }

        public static P2PPacket NetworkPacketToP2PPacket(NetworkPacket networkPacket, string hostName)
        {
            P2PPacket packet = networkPacket.ToPacket<P2PPacket>();

            switch ((P2PPacketTags)packet.Tag)
            {
                case P2PPacketTags.Text:
                    packet = packet.ToPacket<P2PTextPacket>();
                    break;
                case P2PPacketTags.TextObject:
                    packet = packet.ToPacket<P2PTextObjectPacket>();
                    break;
                default:
                    System.Diagnostics.Debugger.Break();
                    break;

            }
            packet.FromHostName = hostName;

            return packet;
        }
    }

    public class P2PTextPacket : P2PPacket
    {
        public string Command;

        public override bool Encapsulate()
        {
            base.Tag = (byte)P2PPacketTags.Text;

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

    public class P2PTextObjectPacket : P2PPacket
    {
        public object Object;

        public override bool Encapsulate()
        {
            base.Tag = (byte)P2PPacketTags.TextObject;
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

    public class P2PNetwork : GenericP2PNetwork<P2PPacket>
    {
        private LocalSiteServer localSiteServer { get; set;  }

        public void SetParameters(string name, LocalSiteServer lsServer)
        {
            LocalName = name;
            localSiteServer = lsServer;
        }

        public override void OnPacketArrived(PeerConnection conn, P2PPacket p2pPacket)
        {
            localSiteServer.HandleP2PPacket(conn, p2pPacket);
        }
    }














    public enum LocalSiteServerPacketTags
    {
        None,
        Text,
        TextObject,
    }


    public class LocalSiteServerPacket : SessionStepPacket
    {
        public int StepFromIndex;

        public override bool Encapsulate()
        {
            if (!base.Encapsulate())
                return false;

            return WriteInt(StepFromIndex);
        }

        protected override void Unencapsulate()
        {
            base.Unencapsulate();
            StepFromIndex = ReadInt();
        }

        public static LocalSiteServerPacket NetworkPacketToLocalSitePacket(NetworkPacket networkPacket)
        {
            LocalSiteServerPacket lsPacket = networkPacket.ToPacket<LocalSiteServerPacket>();

            switch ((LocalSiteServerPacketTags)lsPacket.Tag)
            {
                case LocalSiteServerPacketTags.Text:
                    {
                        return lsPacket.ToPacket<LocalSiteServerTextPacket>();
                    }
                case LocalSiteServerPacketTags.TextObject:
                    {
                        return lsPacket.ToPacket<LocalSiteServerTextObjectPacket>();
                    }
                default:
                    System.Diagnostics.Debugger.Break();
                    break;
                
            }
            return null;
        }

    }

    public class LocalSiteServerTextPacket : LocalSiteServerPacket
    {
        public string Text = "";

        public override bool Encapsulate()
        {
            Tag = (byte)LocalSiteServerPacketTags.Text;
            if (!base.Encapsulate())
                return false;

            return WriteString(Text);
        }

        protected override void Unencapsulate()
        {
            base.Unencapsulate();
            Text = ReadString();
        }
    }

    public class LocalSiteServerTextObjectPacket : LocalSiteServerPacket
    {
        public string Text = "";
        public object Object;

        public override bool Encapsulate()
        {
            base.Tag = (byte)LocalSiteServerPacketTags.TextObject;
            if (!base.Encapsulate())
                return false;
            return WriteObject(Object) && WriteString(Text);
        }

        protected override void Unencapsulate()
        {
            base.Unencapsulate();
            Text = ReadString();
            Object = ReadObject();
        }
    }

    public class LocalSiteServerConnection : GenericServerConnection
    {
        
        public Guid SessionId;
        LocalSiteServer localSiteServer;
        int TestValue = 0, TempValue = 0;

        override public void Start()
        {
            localSiteServer = (LocalSiteServer)genericServer;

            new Thread(new ThreadStart(ThreadProcessPackets)).Start();
            base.Start();
        }

        void EncapsulateAndSendP2PPacket(string p2pDest, int stepIndex, P2PPacket p2pPacket)
        {
            p2pPacket.StepIndex = stepIndex;
            p2pPacket.SessionId = SessionId;
            p2pPacket.Encapsulate();
            PeerConnection peerConn = localSiteServer.P2PNetwork.GetConnection(p2pDest);
            if (peerConn != null)
                peerConn.SendPacket(p2pPacket);
            else
                System.Diagnostics.Debugger.Break();
        }

        List<SessionStepPacket> stepWaitingPackets = new List<SessionStepPacket>();
        List<SessionStepPacket> stepDonePackets = new List<SessionStepPacket>();
        Dictionary<string, SessionStepPacket> incompletedPackets = new Dictionary<string, SessionStepPacket>();

        void ProcessSessionStepPacket(SessionStepPacket ssPacket)
        {
            if (ssPacket is LocalSiteServerTextPacket)
            {
                LocalSiteServerTextPacket packet = (LocalSiteServerTextPacket)ssPacket;

                Debug.WriteLine(localSiteServer.Name + " LCL " + packet.Text);

                string[] args = packet.Text.Split(":".ToCharArray());
                if (args[0] == "Test")
                {
                    P2PTextPacket p2pPacket = new P2PTextPacket();
                    if (args[1] == "Set")
                    {
                        TestValue = int.Parse(args[2]);
                        SendOKPacket();
                    }
                    else if (args[1] == "Send")
                    {

                        p2pPacket.Command = "Test:Set:" + TestValue.ToString();
                        EncapsulateAndSendP2PPacket(args[2], packet.StepIndex, p2pPacket);
                        Debug.WriteLine(localSiteServer.Name + " send P2P " + p2pPacket.Command);

                    }
                    else if (args[1] == "Sub")
                    {
                        TestValue = - TempValue;
                    }
                    else if (args[1] == "Return")
                    {
                        Debug.WriteLine(localSiteServer.Name + " Return " + TestValue.ToString());
                        SendOKPacket();
                    }
                    else
                    {
                        System.Diagnostics.Debugger.Break();
                    }
                }
            }
            else if (ssPacket is P2PTextPacket)
            {
                P2PTextPacket packet = (P2PTextPacket)ssPacket;

                Debug.WriteLine(localSiteServer.Name + " P2P " + packet.Command);

                string[] args = packet.Command.Split(":".ToCharArray());
                if (args[0] == "Test")
                {
                    if (args[1] == "Set")
                    {
                        TempValue = int.Parse(args[2]);
                    }
                    else
                    {
                        System.Diagnostics.Debugger.Break();
                    }
                }
            }
        }

        void ThreadProcessPackets()
        {
            while(true)
            {
                SessionStepPacket ssPacket = (SessionStepPacket)Packets.WaitAndRead(Timeout.Infinite);
                if (ssPacket != null)
                {
                    if (ssPacket.SessionId != SessionId)
                    {
                        if (ssPacket is LocalSiteServerPacket)
                        {
                            Guid oldSessionId = SessionId;
                            SessionId = ssPacket.SessionId;

                            stepWaitingPackets.Clear(); stepDonePackets.Clear();
                            for (int i = 0; i < 100; i++)
                            {
                                stepDonePackets.Add(null);
                                stepWaitingPackets.Add(null);
                            }
                            incompletedPackets.Clear();

                            //新建的会话，或者会话还没建立，但P2P数据已经到达
                            localSiteServer.UpdateSessionConnectionTable(this, oldSessionId, SessionId);

                            //继续执行
                        }
                        else
                        {
                            //过期的P2P数据包，忽略
                            continue;
                        }
                    }

                    //如果有未完成的包在等待后续包，则处理之前的未完成的包
                    if (incompletedPackets.ContainsKey(ssPacket.FromHostName))
                    {
                        incompletedPackets[ssPacket.FromHostName].AddFollowingPacket(ssPacket);
                        if (incompletedPackets[ssPacket.FromHostName].IsCompleted
                            ||
                            ssPacket.IsCompleted)
                        {
                            //开始处理已经完成的包
                            ssPacket = incompletedPackets[ssPacket.FromHostName];
                            incompletedPackets.Remove(ssPacket.FromHostName);
                        }
                        else
                        {
                            //当前包只是后续数据，不需要处理。
                            continue;
                        }
                    }
                    else
                    {
                        if (!ssPacket.IsCompleted)
                        {
                            //未完成的包，等待后续
                            incompletedPackets[ssPacket.FromHostName] = ssPacket;
                            continue;
                        }
                    }

                    
                    
                    bool modified = false;

                    if (ssPacket is P2PPacket || (ssPacket as LocalSiteServerPacket).StepFromIndex == SessionStepPacket.StepIndexNone)
                    {
                        ProcessSessionStepPacket(ssPacket);

                        if (ssPacket.StepIndex != SessionStepPacket.StepIndexNone)
                        {
                            stepDonePackets[ssPacket.StepIndex] = ssPacket;
                            modified = true;
                        }
                    }
                    else
                    {
                        stepWaitingPackets[ssPacket.StepIndex] = ssPacket;
                        modified = true;
                    }    

                    while (modified)
                    {
                        modified = false;
                        for (int i = 0; i < stepWaitingPackets.Count; i++)
                        {
                            if (stepWaitingPackets[i] != null && stepDonePackets[ (stepWaitingPackets[i] as LocalSiteServerPacket).StepFromIndex] != null)
                            {
                                ProcessSessionStepPacket(stepWaitingPackets[i]);
                                stepDonePackets[stepWaitingPackets[i].StepIndex] = stepWaitingPackets[i];
                                stepWaitingPackets[i] = null;
                                modified = true;
                            }
                        }
                    }
                }
            }
        }

        void SendOKPacket()
        {
            ServerClientTextPacket packet = new ServerClientTextPacket();
            packet.Encapsulate();
            SendPacket(packet);
        }

        override public void OnPacketArrived(NetworkPacket networkPacket)
        {
            LocalSiteServer localSiteServer = (LocalSiteServer)genericServer;
            LocalSiteServerPacket lsPacket = LocalSiteServerPacket.NetworkPacketToLocalSitePacket(networkPacket);

            lsPacket.FromHostName = localSiteServer.Name;
            Packets.Append(lsPacket);            
        }
    }

    public class LocalSiteServer : GenericServer<LocalSiteServerConnection>
    {
        Dictionary<Guid, LocalSiteServerConnection> SessionConnections = new Dictionary<Guid,LocalSiteServerConnection>();
        Dictionary<Guid, PacketQueue> TempSessionPackets = new Dictionary<Guid, PacketQueue>();

        public P2PNetwork P2PNetwork;
        public ClusterConfiguration ClusterConfig;

        public string Name { get; set; }

        public LocalSiteServer(ClusterConfiguration clusterConfig, string name)
        {
            this.ClusterConfig = clusterConfig;
            Name = name;
        }


        public void UpdateSessionConnectionTable(LocalSiteServerConnection conn, Guid oldSessionId, Guid newSessionId)
        {
            lock (SessionConnections)
            {
                if (oldSessionId != Guid.Empty && SessionConnections.ContainsKey(oldSessionId))
                {
                    SessionConnections.Remove(oldSessionId);
                }

                SessionConnections[newSessionId] = conn;
                if (TempSessionPackets.ContainsKey(newSessionId))
                {
                    TempSessionPackets[newSessionId].MoveTo(conn.Packets);
                    TempSessionPackets.Remove(newSessionId);
                }
            }
        }

        public void Start()
        {
            //启动 P2P 网络
            P2PNetwork = new P2PNetwork();
            P2PNetwork.SetParameters(Name, this);

            foreach (KeyValuePair<string, Dictionary<string, object>> kv in ClusterConfig.Hosts)
            {
                if(kv.Value.ContainsKey("P2PPort"))
                {
                    P2PNetwork.AddHost(kv.Key, (string)kv.Value["Host"], (int)kv.Value["P2PPort"]);
                }
            }

            P2PNetwork.Start((int)ClusterConfig.Hosts[Name]["P2PPort"]);

            base.Start((int)ClusterConfig.Hosts[Name]["Port"]);
        }


        public void HandleP2PPacket(PeerConnection conn, P2PPacket p2pPacket)
        {
            lock(SessionConnections)
            {

                if (SessionConnections.ContainsKey(p2pPacket.SessionId))
                {
                    SessionConnections[p2pPacket.SessionId].Packets.Append(P2PPacket.NetworkPacketToP2PPacket(p2pPacket, conn.Name));
                }
                else
                {
                    if(!TempSessionPackets.ContainsKey(p2pPacket.SessionId))
                        TempSessionPackets[p2pPacket.SessionId] = new PacketQueue();
                    TempSessionPackets[p2pPacket.SessionId].Append(P2PPacket.NetworkPacketToP2PPacket(p2pPacket, conn.Name));
                }
            }
        }
    }

    public class LocalSiteClient : GenericClientConnection
    {
        public PacketQueue Packets = new PacketQueue();

        override public void OnPacketArrived(NetworkPacket packet)
        {
            Packets.Append(ServerClientPacket.NetworkPacketToServerClientPacket(packet));
        }

    }

}

