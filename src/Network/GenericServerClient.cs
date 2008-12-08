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
    public abstract class GenericNetworkConnection
    {
        protected TcpClient tcpClient;
        protected NetworkStream networkStream;

        byte[] buffer = new byte[65536];
        int dataSizeInBuffer = 0;


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
                    networkStream.BeginRead(buffer, dataSizeInBuffer, buffer.Length - dataSizeInBuffer, new AsyncCallback(AsyncReceiveCallback), this);
                }
            }
            catch (System.Exception)
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


        abstract public void OnPacketArrived(NetworkPacket packet);
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
