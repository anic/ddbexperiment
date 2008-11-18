using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace DistDBMS.Network
{

    public class PacketQueue
    {
        Queue<NetworkPacket> packets = new Queue<NetworkPacket>();
        ManualResetEvent dataEvent = new ManualResetEvent(false);
        ReaderWriterLockSlim rwlock = new ReaderWriterLockSlim();

        public void Append(NetworkPacket packet)
        {
            lock (packets)
            {
                packets.Enqueue(packet);
                dataEvent.Set();
            }
        }

        public NetworkPacket WaitAndRead()
        {
            return WaitAndRead(Timeout.Infinite);
        }

        public void MoveTo(PacketQueue other)
        {
            lock (packets)
            {
                while (packets.Count != 0)
                {
                    other.packets.Enqueue(packets.Dequeue());
                }
            }
        }

        public NetworkPacket WaitAndRead(int timeout)
        {
            bool needWait = false;

            lock (packets)
            {
                if (packets.Count == 0)
                    needWait = true;
            }

            if (needWait)
            {
                dataEvent.WaitOne(timeout, false);
                if (packets.Count == 0)
                    System.Diagnostics.Debugger.Break();
            }
            NetworkPacket packet = null;
            lock (packets)
            {
                packet = packets.Dequeue();
                dataEvent.Reset();
            }

            return packet;
        }


    }
}
