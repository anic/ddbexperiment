using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.Runtime;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace DistDBMS.Network
{
    public enum ServerClientPacketTags
    {
        None,
        Text,
        TextObject,
    }


    public class ServerClientPacket : NetworkPacket
    {
        public static ServerClientPacket NetworkPacketToServerClientPacket(NetworkPacket networkPacket)
        {
            switch ((ServerClientPacketTags)networkPacket.Tag)
            {
                case ServerClientPacketTags.Text:
                    {
                        return networkPacket.ToPacket<ServerClientTextPacket>();
                    }
                case ServerClientPacketTags.TextObject:
                    {
                        return networkPacket.ToPacket<ServerClientTextObjectPacket>();
                    }
                default:
                    System.Diagnostics.Debugger.Break();
                    break;

            }
            return null;
        }


    }

    public class ServerClientTextPacket : ServerClientPacket
    {
        public string Text = "";

        public override bool Encapsulate()
        {
            Tag = (byte)ServerClientPacketTags.Text;
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

    public class ServerClientTextObjectPacket : ServerClientPacket
    {
        public string Text = "";
        public object Object;

        public override bool Encapsulate()
        {
            Tag = (byte)ServerClientPacketTags.TextObject;
            if (!base.Encapsulate())
                return false;
            return WriteString(Text) && WriteObject(Object);
        }

        protected override void Unencapsulate()
        {
            base.Unencapsulate();
            Text = ReadString();
            Object = ReadObject();
        }
    }




    public abstract class GenericNetworkConnection
    {
        PacketQueue packets = new PacketQueue();
        public PacketQueue Packets { get { return packets; } }

        protected TcpClient tcpClient;
        protected NetworkStream networkStream;

        byte[] buffer = new byte[65536];
        int dataSizeInBuffer = 0;

        public Object State { get; set; }

        public void AsyncReceiveCallback(IAsyncResult ar)
        {
            bool term = false;
            try
            {
                int bytesRead = networkStream.EndRead(ar);
                if (bytesRead == 0)
                {
                    //it's over;
                    term = true;
                }
                else
                {
                    dataSizeInBuffer += bytesRead;
                    while (true)
                    {
                        if (!NetworkPacket.IsInvalidPacket(buffer, dataSizeInBuffer))
                        {
                            dataSizeInBuffer = 0;
                            break;
                        }
                        else
                        {
                            NetworkPacket packet = NetworkPacket.FetchFromBuffer(ref buffer, ref dataSizeInBuffer);
                            if (packet != null)
                                OnPacketArrived(packet);
                            else
                                break;
                        }
                    }



                    if (buffer.Length >= 1024 * 1024 && dataSizeInBuffer < buffer.Length / 2)
                    {
                        byte[] old = buffer;
                        buffer = new byte[buffer.Length / 2];
                        Array.Copy(old, 0, buffer, 0, dataSizeInBuffer);
                    }
                    if (buffer.Length - dataSizeInBuffer <= 8 * 1024)
                    {
                        byte[] old = buffer;
                        buffer = new byte[dataSizeInBuffer + 8 * 1024];
                        old.CopyTo(buffer, 0);
                    }
                    networkStream.BeginRead(buffer, dataSizeInBuffer, buffer.Length - dataSizeInBuffer, new AsyncCallback(AsyncReceiveCallback), this);
                }
            }
            catch (System.Net.Sockets.SocketException e)
            {
                term = true;
            }
            catch (System.IO.IOException e)
            {
                term = true;
            }
            if(term)
                tcpClient.Close();

        }

        virtual public void Start()
        {
            networkStream.BeginRead(buffer, dataSizeInBuffer, buffer.Length - dataSizeInBuffer, new AsyncCallback(AsyncReceiveCallback), this);
        }

        public void SendPacket(NetworkPacket packet)
        {
            networkStream.Write(packet.Data, 0, packet.Size);
        }

        public void SendServerClientTextPacket(string text)
        {
            ServerClientTextPacket packet = new ServerClientTextPacket();
            packet.Text = text;
            packet.Encapsulate();
            SendPacket(packet);
        }

        public void SendServerClientTextObjectPacket(string text, object obj)
        {
            ServerClientTextObjectPacket packet = new ServerClientTextObjectPacket();
            packet.Text = text;
            packet.Object = obj;
            packet.Encapsulate();
            SendPacket(packet);
        }

        virtual public void OnPacketArrived(NetworkPacket packet)
        {
            Packets.Append(packet);
        }
    }

    public abstract class GenericServerConnection : GenericNetworkConnection
    {
        protected object genericServer;

        public void Attach(object server, TcpClient tcpClient)
        {
            this.tcpClient = tcpClient;
            networkStream = tcpClient.GetStream();
            genericServer = server;
        }
    }

    public abstract class GenericClientConnection : GenericNetworkConnection
    {
        public void Connect(string ip, int port)
        {
            tcpClient = new TcpClient();
            tcpClient.Connect(ip, port);
            networkStream = tcpClient.GetStream();

            Start();
        }

    }

    public class GenericServer<CONNECTION> where CONNECTION : GenericServerConnection, new()
    {
        Thread thread;
        class ThreadParam
        {
            public int ListenPort;
        }

        public delegate void ConnectionStartCloseDelegate(GenericServerConnection conn);
        public ConnectionStartCloseDelegate ConnectionStart { get; set; }

        public void ThreadProc(object obj)
        {
            ThreadParam param = (ThreadParam)obj;
            TcpListener listener = new TcpListener(IPAddress.Any, param.ListenPort);
            listener.Start();

            while (true)
            {
                TcpClient tcpClient = listener.AcceptTcpClient();
                CONNECTION conn = new CONNECTION();
                conn.Attach(this, tcpClient);

                if(ConnectionStart != null)
                    ConnectionStart(conn);

                conn.Start();
            }
        }


        public void Start(int port)
        {
            ThreadParam param = new ThreadParam();
            thread = new Thread(new ParameterizedThreadStart(ThreadProc));

            param.ListenPort = port;
            thread.Start(param);
        }
    }

}
