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
        Data,
        Command,
        Object,
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
                case P2PPacketTags.Command:
                    packet = packet.ToPacket<P2PCommandPacket>();
                    break;
                case P2PPacketTags.Data:
                    packet = packet.ToPacket<P2PDataPacket>();
                    break;
                default:
                    System.Diagnostics.Debugger.Break();
                    break;

            }
            packet.FromHostName = hostName;

            return packet;
        }
    }

    public class P2PDataPacket : P2PPacket
    {
        public bool IsLast;
        public override bool Encapsulate()
        {
            base.Tag = (byte)P2PPacketTags.Data;

            if (!base.Encapsulate())
                return false;

            return WriteByte(IsLast ? (byte)1 : (byte)0);
        }

        protected override void Unencapsulate()
        {
            base.Unencapsulate();
            IsLast = (ReadByte() != 0);
        }

        override public bool IsCompleted { get { return IsLast; } }
    }

    public class P2PCommandPacket : P2PPacket
    {
        public string Command;

        public override bool Encapsulate()
        {
            base.Tag = (byte)P2PPacketTags.Command;

            if (!base.Encapsulate())
                return false;

            return WriteString(Command);
        }

        protected override void Unencapsulate()
        {
            base.Unencapsulate();
            Command = ReadString();
        }

        override public bool IsCompleted { get { 
            return !Command.Contains("Data");
        } }
    }

    public class P2PObjectPacket : P2PPacket
    {
        public object Object;

        public override bool Encapsulate()
        {
            base.Tag = (byte)P2PPacketTags.Object;
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














    public enum LocalSitePacketTags
    {
        None,
        OK,
        Command,
        Object,
    }


    public class LocalSitePacket : SessionStepPacket
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

        public static LocalSitePacket NetworkPacketToLocalSitePacket(NetworkPacket networkPacket)
        {
            LocalSitePacket lsPacket = networkPacket.ToPacket<LocalSitePacket>();

            switch ((LocalSitePacketTags)lsPacket.Tag)
            {
                case LocalSitePacketTags.Command:
                    {
                        return lsPacket.ToPacket<LocalSiteCommandPacket>();
                    }
                case LocalSitePacketTags.OK:
                    {
                        return lsPacket.ToPacket<LocalSiteOKPacket>();
                    }
                default:
                    System.Diagnostics.Debugger.Break();
                    break;
                
            }
            return null;
        }

    }

    public class LocalSiteOKPacket : LocalSitePacket
    {
        public override bool Encapsulate()
        {
            Tag = (byte)LocalSitePacketTags.OK;
            if (!base.Encapsulate())
                return false;
            return true;
        }
    }

    public class LocalSiteCommandPacket : LocalSitePacket
    {
        public string Command;

        public override bool Encapsulate()
        {
            Tag = (byte)LocalSitePacketTags.Command;
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

    public class LocalSiteObjectPacket : LocalSitePacket
    {
        public object Object;

        public override bool Encapsulate()
        {
            base.Tag = (byte)LocalSitePacketTags.Object;
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

    public class LocalSiteServerConnection : GenericServerConnection
    {
        public PacketQueue Packets = new PacketQueue();
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

        void EncapsulateAndSendP2PData(string p2pDest, byte []data, int offset, int length, bool isLast)
        {
            P2PDataPacket dataPacket = new P2PDataPacket();
            dataPacket.StepIndex = P2PPacket.StepIndexNone;
            dataPacket.SessionId = SessionId;
            dataPacket.IsLast = isLast;
            dataPacket.Encapsulate();
            dataPacket.WriteBytes(data, offset, length);

            PeerConnection peerConn = localSiteServer.P2PNetwork.GetConnection(p2pDest);
            if (peerConn != null)
                peerConn.SendPacket(dataPacket);
            else
                System.Diagnostics.Debugger.Break();
        }


        List<SessionStepPacket> stepWaitingPackets = new List<SessionStepPacket>();
        List<SessionStepPacket> stepDonePackets = new List<SessionStepPacket>();
        Dictionary<string, SessionStepPacket> incompletedPackets = new Dictionary<string, SessionStepPacket>();

        void ProcessSessionStepPacket(SessionStepPacket ssPacket)
        {
            if (ssPacket is LocalSiteCommandPacket)
            {
                LocalSiteCommandPacket packet = (LocalSiteCommandPacket)ssPacket;

                Debug.WriteLine(localSiteServer.Name + " LCL " + packet.Command);

                string[] args = packet.Command.Split(":".ToCharArray());
                if (args[0] == "Test")
                {
                    P2PCommandPacket p2pPacket = new P2PCommandPacket();
                    if (args[1] == "Set")
                    {
                        TestValue = int.Parse(args[2]);
                        SendOKPacket();
                    }
                    else if (args[1] == "Send")
                    {
                        /*
                        p2pPacket.Command = "Test:Set:" + TestValue.ToString();
                        EncapsulateAndSendP2PPacket(args[2], packet.StepIndex, p2pPacket);
                        Debug.WriteLine(localSiteServer.Name + " send P2P " + p2pPacket.Command);
                        */
                        p2pPacket.Command = "Test:Data";
                        EncapsulateAndSendP2PPacket(args[2], packet.StepIndex, p2pPacket);
                        Debug.WriteLine(localSiteServer.Name + " send P2P " + p2pPacket.Command);

                        EncapsulateAndSendP2PData(args[2], new byte[] { 1, 2, 3, 4, 5, 6 }, 0, 6, false);
                        Debug.WriteLine(localSiteServer.Name + " send P2P data");

                        EncapsulateAndSendP2PData(args[2], new byte[] { 9, 8, 7, 6, 5, 4 }, 0, 6, true);
                        Debug.WriteLine(localSiteServer.Name + " send P2P data");
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
            else if (ssPacket is P2PCommandPacket)
            {
                P2PCommandPacket packet = (P2PCommandPacket)ssPacket;

                Debug.WriteLine(localSiteServer.Name + " P2P " + packet.Command);

                string[] args = packet.Command.Split(":".ToCharArray());
                if (args[0] == "Test")
                {
                    if (args[1] == "Set")
                    {
                        TempValue = int.Parse(args[2]);
                    }
                    else if (args[1] == "Data")
                    {
                        P2PDataPacket dataPacket = (P2PDataPacket)packet.FollowingPackets[1];
                        Debug.WriteLine(localSiteServer.Name + " P2P " + packet.Command + " " + dataPacket.ReadByte().ToString());
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
                    if (ssPacket is P2PPacket)
                        if (ssPacket.SessionId != SessionId)
                            continue;

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

                    lock (stepWaitingPackets)
                    {
                        bool modified = false;

                        if (ssPacket is P2PPacket || (ssPacket as LocalSitePacket).StepFromIndex == SessionStepPacket.StepIndexNone)
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
                                if (stepWaitingPackets[i] != null && stepDonePackets[ (stepWaitingPackets[i] as LocalSitePacket).StepFromIndex] != null)
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
        }

        void SendOKPacket()
        {
            LocalSiteOKPacket packet = new LocalSiteOKPacket();
            packet.Encapsulate();
            SendPacket(packet);
        }

        override public void OnPacketArrived(NetworkPacket networkPacket)
        {
            LocalSiteServer localSiteServer = (LocalSiteServer)genericServer;
            LocalSitePacket lsPacket = LocalSitePacket.NetworkPacketToLocalSitePacket(networkPacket);
            Guid oldSessionId = SessionId;
            SessionId = lsPacket.SessionId;


            if (oldSessionId != lsPacket.SessionId)
            {
                lock (stepWaitingPackets)
                {
                    stepWaitingPackets.Clear(); stepDonePackets.Clear();
                    stepDonePackets.Add(null); stepDonePackets.Add(null); stepDonePackets.Add(null); stepDonePackets.Add(null); stepDonePackets.Add(null); stepDonePackets.Add(null);
                    stepWaitingPackets.Add(null); stepWaitingPackets.Add(null); stepWaitingPackets.Add(null); stepWaitingPackets.Add(null); stepWaitingPackets.Add(null); stepWaitingPackets.Add(null); stepWaitingPackets.Add(null);
                    incompletedPackets.Clear();
                }
            }

            lsPacket.FromHostName = localSiteServer.Name;
            Packets.Append(lsPacket);

            if (oldSessionId != lsPacket.SessionId)
            {
                //新建的会话，或者会话还没建立，但P2P数据已经到达
                localSiteServer.UpdateSessionConnectionTable(this, oldSessionId, SessionId);
            }
            
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
            Packets.Append(LocalSitePacket.NetworkPacketToLocalSitePacket(packet));
        }

    }

}

