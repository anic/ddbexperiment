using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace DistDBMS.Network
{
    public class NetworkPacket
    {
        const int HeaderSize = 4;
        const int DefaultNewPacketBufferSize = 1400;
        const int MaxSize = 1024 * 1024 * 1024;
        
        byte[] data;
        int size;
        int pos;  //读写偏移

        public byte[] Data  { get {
            //BitConverter.GetBytes((ushort)IPAddress.HostToNetworkOrder((short)(size))).CopyTo(data, 0);
            BitConverter.GetBytes((uint)IPAddress.HostToNetworkOrder((int)(size))).CopyTo(data, 0);
            return data;
        } }

        public int Size  { get {return size;} }

        public int Pos { get { return pos; } }

        //virtual public bool IsFragmented { get { return false; } }

        public void CopyFrom(NetworkPacket packet, int offset, int count)
        {
            Array.Copy(packet.data, offset, this.data, pos, count);
            pos += count;
            size += count;
        }

        public byte Tag;
        virtual public bool Encapsulate()
        {
            pos = HeaderSize;
            size = HeaderSize;
            if (!WriteByte(Tag))
                return false;

            return true;
        }

        protected void AttachPacket(NetworkPacket packet)
        {
            this.size = packet.size;
            this.data = packet.data;
            this.pos = packet.pos;
            Unencapsulate();
        }

        public T ToPacket<T>() where T : NetworkPacket, new()
        {
            T obj = new T();
            obj.AttachPacket(this);
            return obj;
        }

        virtual protected void Unencapsulate()
        {
            pos = HeaderSize;
            Tag = ReadByte();
        }


        virtual public bool IsCompleted { get { return true; } }
        public List<NetworkPacket> FollowingPackets;
        public void AddFollowingPacket(NetworkPacket packet)
        {
            if (FollowingPackets == null)
                FollowingPackets = new List<NetworkPacket>();
            FollowingPackets.Add(packet);
        }


        public static bool IsInvalidPacket(byte[] buffer, int size)
        {
            if (size >= HeaderSize)
            {
                int packetSize = IPAddress.NetworkToHostOrder(BitConverter.ToInt32(buffer, 0));

                if (packetSize <= 0 || packetSize > MaxSize)
                    return false;

            }
            return true;
        }

        private NetworkPacket(byte []buffer, int size)
        {
            this.size = size;
            this.data = new byte[size];
            Array.Copy(buffer, this.data, this.size);
            this.pos = HeaderSize;
        }

        public NetworkPacket()
        {
            size = HeaderSize;
            data = new byte[DefaultNewPacketBufferSize];
            pos = HeaderSize;
        }

        public static NetworkPacket FetchFromBuffer(ref byte[] buffer, ref int dataSizeInBuffer)
        {
            if (dataSizeInBuffer < HeaderSize)
                return null;

            int packetSize = IPAddress.NetworkToHostOrder(BitConverter.ToInt32(buffer, 0));
            if (packetSize <= dataSizeInBuffer)
            {
                NetworkPacket packet = new NetworkPacket(buffer, dataSizeInBuffer);
                Array.Copy(buffer, packetSize, buffer, 0, dataSizeInBuffer - packetSize);
                dataSizeInBuffer -= packetSize;
                try
                {
                    packet.Unencapsulate();
                }
                catch (System.Exception)
                {
                    return null;
                }
                
                return packet;
            }
            return null;
        }

        public byte ReadByte()
        {
            if(pos + sizeof(byte) > size)
                throw new EndOfStreamException();
            return data[pos++];
        }

        public short ReadShort()
        {
            if (pos + sizeof(short) > size)
                throw new EndOfStreamException();
            short val = BitConverter.ToInt16(data, pos);
            pos += sizeof(short);
            return IPAddress.NetworkToHostOrder(val);
        }
        public ushort ReadUShort()
        {
            return (ushort)ReadShort();
        }
        public int ReadInt()
        {
            if (pos + sizeof(int) > size)
                throw new EndOfStreamException();
            int val = BitConverter.ToInt32(data, pos);
            pos += sizeof(int);
            return IPAddress.NetworkToHostOrder(val);
        }
        public uint ReadUInt()
        {
            return (uint)ReadInt();
        }
        public long ReadLong()
        {
            if (pos + sizeof(long) > size)
                throw new EndOfStreamException();
            long val = BitConverter.ToInt64(data, pos);
            pos += sizeof(long);
            return IPAddress.NetworkToHostOrder(val);
        }
        public ulong ReadULong()
        {
            return (ulong)ReadLong();
        }
        public Guid ReadGuid()
        {
            byte []buf = new byte[16];
            ReadBytes(buf, 0, 16);
            return new Guid(buf);
        }
        
        public void ReadBytes(byte[] buffer, int offset, int count)
        {
            if(pos + count > size)
                throw new EndOfStreamException();

            Array.Copy(data, pos, buffer, offset, count);
            pos += count;
        }

        public string ReadString()
        {
            ushort len = ReadUShort();
            if(pos + len > size)
                throw new EndOfStreamException();
            string s = System.Text.Encoding.UTF8.GetString(data, pos, len);
            pos += len;
            return s;
        }


        public object ReadObject()
        {
            long timeStart = DateTime.Now.Ticks;

            int len = ReadInt();
            if (pos + len > size)
                throw new EndOfStreamException();

            //if (len > 1024 * 1024)
            //    System.Diagnostics.Debugger.Break();

            MemoryStream ms = new MemoryStream(data, pos, (int)len);
            BinaryFormatter bs = new BinaryFormatter();
            pos += len;


            object obj = bs.Deserialize(ms);

            timeDelay += DateTime.Now.Ticks - timeStart;
            DistDBMS.Common.Debug.WriteLine(" time : " + ((DateTime.Now.Ticks - timeStart) / 10000).ToString() + "ms, total : " + (timeDelay / 10000).ToString() + "ms");

            return obj;
        }



        public bool EnsureSize(int newSize)
        {
            if(newSize > MaxSize)
                throw new OverflowException();

            if(data.Length < newSize)
            {
                newSize = data.Length * 2 > newSize ? data.Length * 2 : newSize;
                if (newSize > MaxSize)
                    newSize = MaxSize;

                byte[] tmp = data;
                data = new byte[newSize];
                tmp.CopyTo(data, 0);
            }

            return true;
        }

        public bool WriteBytes(byte[] buffer, int offset, int count)
        {
            if (!EnsureSize(pos + count))
                return false;

            Array.Copy(buffer, offset, data, pos, count);
            pos += count;

            if (pos > size)
                size = pos;

            return true;
        }

        public bool WriteBytes(byte []buffer)
        {
            return WriteBytes(buffer, 0, buffer.Length);
        }


        public bool WriteByte(byte val)
        {
            if (!EnsureSize(pos + sizeof(byte)))
                return false;

            data[pos++] = val;
            if (pos > size)
                size = pos;
            return true;
        }
        public bool WriteShort(short val)
        {
            return WriteBytes(BitConverter.GetBytes(IPAddress.HostToNetworkOrder(val)));
        }
        public bool WriteUShort(ushort val)
        {
            return WriteShort((short)val);
        }
        public bool WriteInt(int val)
        {
            return WriteBytes(BitConverter.GetBytes(IPAddress.HostToNetworkOrder(val)));
        }
        public bool WriteUInt(uint val)
        {
            return WriteInt((int)val);
        }
        public bool WriteLong(long val)
        {
            return WriteBytes(BitConverter.GetBytes(IPAddress.HostToNetworkOrder(val)));
        }
        public bool WriteULong(ulong val)
        {
            return WriteLong((long)val);
        }

        public bool WriteString(string s)
        {
            if (!WriteUShort((ushort)s.Length))
                return false;

            byte[] data = System.Text.Encoding.UTF8.GetBytes(s);
            if(!WriteBytes(data))
            {
                pos -= sizeof(ushort);
                size = pos;
                return false;
            }
            return true;
        }

        static long timeDelay = 0;
        public bool WriteObject(object obj)
        {

            long timeStart = DateTime.Now.Ticks;

            MemoryStream ms = new MemoryStream();
            BinaryFormatter bs = new BinaryFormatter();
            

            bs.Serialize(ms, obj);

            long timeStart1 = DateTime.Now.Ticks;

            ms.Position = 0;
            if (!WriteInt((int)ms.Length))
                return false;


            if(!WriteBytes(ms.ToArray()))
            {
                pos -= sizeof(int);
                size = pos;
                return false;
            }

            timeDelay += DateTime.Now.Ticks - timeStart;
            DistDBMS.Common.Debug.WriteLine(" time : " + ((DateTime.Now.Ticks - timeStart) / 10000).ToString() + "ms, total: : " +  ((timeDelay) / 10000).ToString() + "ms");
            
            return true;
        }

        public bool WriteGuid(Guid val)
        {
            return WriteBytes(val.ToByteArray());
        }

    }
}

