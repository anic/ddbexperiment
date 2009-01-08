using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Threading;
using System.Net;

namespace DistDBMS.Network
{

    enum GenericP2PPacketTags : byte
    {
        GenericP2PControl,
        Application,
    }

    public class GenericP2PControlPacket : NetworkPacket
    {
        public string Command;

        public override bool Encapsulate()
        {
            if (!base.Encapsulate())
                return false;
            return WriteString(Command);
        }

        protected override void Unencapsulate()
        {
            Command = ReadString();
        }
    }


    public class PeerConnection : IDisposable
    {
        public Socket PeerSocket;
        public byte[] Buffer = new byte[65536];
        //public byte[] Buffer = new byte[10*1024*1024];
        public int DataSizeInBuffer = 0;

        public string Name { get; set; }
        public PeerConnection(string name, Socket socket)
        {
            Name = name;
            PeerSocket = socket;
        }

        public void SendPacket(NetworkPacket packet)
        {
            long timeStart = DateTime.Now.Ticks;
            PeerSocket.Send(packet.Data, 0, packet.Size, SocketFlags.None);
            DistDBMS.Common.Debug.WriteLine(" size = " + packet.Size.ToString() + ", time = " + ((DateTime.Now.Ticks - timeStart) / 10000).ToString() + "ms");
        }


        public void Dispose()
        {
            if (PeerSocket != null)
            {
                try
                {
                    PeerSocket.Close();
                }
                catch (System.Exception)
                {

                }
            }
        }
    }

    public abstract class GenericP2PNetwork<PACKET_TYPE> where PACKET_TYPE : NetworkPacket, new()
    {
        Dictionary<string, PeerConnection> peerConns;

        public string  LocalName { set; get; }

        public class PeerHost
        {
            public PeerHost(string host, int port)
            {
                Host = host;
                Port = port;
            }

            public string Host;
            public int Port;
        }

        public Dictionary<string, PeerHost> PeerHosts;

        public GenericP2PNetwork()
        {
            peerConns = new Dictionary<string, PeerConnection>();
            PeerHosts = new Dictionary<string, PeerHost>();
            LocalName = Guid.NewGuid().ToString();
        }

        public void AddHost(string name, string host, int port)
        {
            lock(PeerHosts)
            {
                PeerHosts[name] = new PeerHost(host, port);
            }
        }

        public void ChangePeerName(PeerConnection peerConn, string name)
        {
            if(peerConn.Name != name)
            {
                lock(peerConns)
                {
                    string oldname = peerConn.Name;
                    peerConns.Remove(oldname);
                    peerConns[name] = peerConn;
                    peerConn.Name = name;

                    DistDBMS.Common.Debug.WriteLine(oldname + " => " + name);
                }
            }
        }

        void ProcessControlPacket(PeerConnection peerConn, GenericP2PControlPacket packet)
        {
            string s = packet.Command;

            string[] cmds = s.Split("\r\n".ToCharArray());
            for(int i = 0; i < cmds.Length; i++)
            {
                string[] pair = cmds[i].Split(":".ToCharArray(), 2);
                if(pair[0] == "LocalName")
                {
                    ChangePeerName(peerConn, pair[1]);
                }
                else
                {
                    DistDBMS.Common.Debug.WriteLine("Unknown command: " + pair[0]);
                }
            }
            
        }

        public void PeerAsyncReceiveCallback(IAsyncResult ar)
        {
            PeerConnection peerConn = (PeerConnection)ar.AsyncState;

            bool connTerm = false;

            try
            {
                int bytesRead = peerConn.PeerSocket.EndReceive(ar);

                if (bytesRead > 0)
                {
                    peerConn.DataSizeInBuffer += bytesRead;

                    while(true)
                    {
                        if (!NetworkPacket.IsInvalidPacket(peerConn.Buffer, peerConn.DataSizeInBuffer))
                        {
                            peerConn.DataSizeInBuffer = 0;
                            break;
                        }
                        else
                        {
                            NetworkPacket packet = NetworkPacket.FetchFromBuffer(ref peerConn.Buffer, ref peerConn.DataSizeInBuffer);
                            if (packet != null)
                            {
                                if (packet.Tag == (byte)GenericP2PPacketTags.GenericP2PControl)
                                {
                                    ProcessControlPacket(peerConn, packet.ToPacket<GenericP2PControlPacket>());
                                }
                                else
                                {
                                    OnPacketArrived(peerConn, packet.ToPacket<PACKET_TYPE>());
                                }
                            }
                            else
                            {
                                break;
                            }
                        }
                    }

                    /*
                     * //shrink
                    if (peerConn.Buffer.Length >= 1024 * 1024 && peerConn.DataSizeInBuffer < peerConn.Buffer.Length / 2)
                    {
                        byte[] old = peerConn.Buffer;
                        peerConn.Buffer = new byte[peerConn.Buffer.Length / 2];
                        Array.Copy(old, 0, peerConn.Buffer, 0, peerConn.DataSizeInBuffer);
                    }
                     */
                    if (peerConn.Buffer.Length - peerConn.DataSizeInBuffer <= 8 * 1024)
                    {
                        byte[] old = peerConn.Buffer;
                        int newSize = peerConn.DataSizeInBuffer > 8 * 1024 ? peerConn.DataSizeInBuffer * 2 : peerConn.DataSizeInBuffer + 8 * 1024;
                        peerConn.Buffer = new byte[newSize];
                        old.CopyTo(peerConn.Buffer, 0);
                    }

                    peerConn.PeerSocket.BeginReceive(peerConn.Buffer, (int)peerConn.DataSizeInBuffer, (int)(peerConn.Buffer.Length - peerConn.DataSizeInBuffer), SocketFlags.None,  new AsyncCallback(PeerAsyncReceiveCallback), peerConn);
                }
                else
                {
                    connTerm = true;
                }
            }
            catch (System.Net.Sockets.SocketException)
            {
                connTerm = true;
            }
            catch(System.IO.IOException)
            {
                connTerm = true;
            }

            if (connTerm)
            {
                RemovePeerConnection(peerConn.Name);
            }

        }

        public void RemovePeerConnection(string name)
        {
            lock (peerConns)
            {
                PeerConnection conn = peerConns[name];
                peerConns.Remove(name);
                conn.Dispose();
            }
        }





        public string GetPeerIPByName(string name)
        {
            return (string)PeerHosts[name].Host;
        }

        public int GetPeerPortByName(string name)
        {
            return (int)PeerHosts[name].Port;
        }


        Thread thread;
        class ThreadParam
        {
            public int ListenPort;
        }




        public void ThreadProc(object obj)
        {
            ThreadParam param = (ThreadParam)obj;

            Socket p2pListener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
            p2pListener.Bind(new IPEndPoint(IPAddress.Any, param.ListenPort));
            p2pListener.Listen(10);

            while (true)
            {

#if DEBUG
                int sleepTime = 50;
#else
                int sleepTime = 5000;
#endif                // 作为主机监听，处理连接请求
                List<Socket> checkReadSockets, checkErrorSockets;
                checkReadSockets = new List<Socket>();
                checkReadSockets.Add(p2pListener);

                checkErrorSockets = new List<Socket>();
                checkErrorSockets.Add(p2pListener);

                try
                {
                    Socket.Select(checkReadSockets, null, checkErrorSockets, sleepTime);
                    for (int i = 0; i < checkReadSockets.Count; i++)
                    {
                        if (checkReadSockets[i] == p2pListener)
                        {
                            Socket socket = p2pListener.Accept();
                            string name = (socket.RemoteEndPoint as IPEndPoint).Address.ToString() + ":" + (socket.RemoteEndPoint as IPEndPoint).Port.ToString();

                            lock (peerConns)
                            {
                                //这里只是临时的一个名字，后续通信过程，会赋予真实的名字
                                PeerConnection peerConn = new PeerConnection(name, socket);
                                peerConns[name] = peerConn;
                                peerConn.PeerSocket.BeginReceive(peerConn.Buffer, peerConn.DataSizeInBuffer, peerConn.Buffer.Length - peerConn.DataSizeInBuffer, SocketFlags.None, new AsyncCallback(PeerAsyncReceiveCallback), peerConn);
                            }
                        }
                    }
                }
                catch (SocketException e)
                {
                    if (e.ErrorCode == (int)System.Net.Sockets.SocketError.TimedOut)
                    {

                    }
                    else
                    {
                        throw e;
                    }
                }



                //主动连接外部主机
                lock (peerConns)
                {
                    lock (PeerHosts)
                    {
                        foreach (KeyValuePair<string, PeerHost> kv in PeerHosts)
                        {
                            //只对比自己名字大的主机发起连接，避免竞争。
                            if (kv.Key.CompareTo(LocalName) > 0 && !peerConns.ContainsKey(kv.Key))
                            {
                                try
                                {
                                    Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
                                    socket.Connect(GetPeerIPByName(kv.Key), GetPeerPortByName(kv.Key));

                                    //主动通知对方本地名字                                    
                                    PeerConnection peerConn = new PeerConnection(kv.Key, socket);
                                    peerConns[kv.Key] = peerConn;
                                    peerConn.PeerSocket.BeginReceive(peerConn.Buffer, peerConn.DataSizeInBuffer, peerConn.Buffer.Length - peerConn.DataSizeInBuffer, SocketFlags.None, new AsyncCallback(PeerAsyncReceiveCallback), peerConn);


                                    GenericP2PControlPacket packet = new GenericP2PControlPacket();
                                    packet.Command = "LocalName:" + LocalName;
                                    packet.Encapsulate();
                                    peerConns[kv.Key].SendPacket(packet);


                                    
                                }
                                catch (System.Net.Sockets.SocketException)
                                {
                                    DistDBMS.Common.Debug.WriteLine("GenericP2PNetwork: can not connect to " + kv.Key);

                                    if (peerConns.ContainsKey(kv.Key))
                                    {
                                        peerConns[kv.Key].Dispose();
                                        peerConns.Remove(kv.Key);
                                    }

                                    Thread.Sleep(2000);
                                }
                            }
                        }
                    }
                }
            }
        }

        public void Start(int port)
        {
            ThreadParam param = new ThreadParam();
            thread = new Thread(new ParameterizedThreadStart(ThreadProc));

            param.ListenPort = port;
            thread.Start(param);
        }




        public PeerConnection GetConnection(string name)
        {
            if (name == LocalName)
                return null;   //P2P中不能自己对自己发请求

            lock (peerConns)
            {
                if (!peerConns.ContainsKey(name))
                {
                    return null;
                }

                return peerConns[name];
            }
        }

        public abstract void OnPacketArrived(PeerConnection conn, PACKET_TYPE packet);
    }
}

