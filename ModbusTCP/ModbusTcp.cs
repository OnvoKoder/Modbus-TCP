using System.Net.Sockets;

namespace ModbusTCP
{
    public class ModbusTcp : Modbus
    {
        private TcpClient client;
        private Socket sock;
        private ushort id;
        private const int DefaultTimeOut = 3000;
        private const byte BeginCodeError = 128;
        public ModbusTcp(string ipAddress, int port = 502, byte address_slave = 1)
        {
            client = new TcpClient();
            address = address_slave;
            id = 0;
            IAsyncResult result = client.BeginConnect(ipAddress, port, null, null);
            if (!result.AsyncWaitHandle.WaitOne(DefaultTimeOut, true))
            {
                client.Close();
                throw new ApplicationException("Failed: Connecting to a device with an error");
            }
            else
            {
                sock = client.Client;
                sock.ReceiveTimeout = DefaultTimeOut;
                sock.SendTimeout = DefaultTimeOut;
            }
        }
        private byte[] MakeMBAP(ushort count)
        {
            byte[] idBytes = BitConverter.GetBytes((short)id);
            return new byte[] {
                idBytes[0],         //message id high byte
                idBytes[1],         //message id low byte
                0,                  //protocol id high byte
                0,                  //protocol id low byte
                (byte)(count >> Byte), //length high byte
                (byte)(count)       //length low byte
            };
        }
        private byte[] SendReceive(byte[] packet)
        {
            byte[] mbap = new byte[7];
            byte[] response;
            ushort count;
            sock.Send(packet);
            sock.Receive(mbap, 0, mbap.Length, SocketFlags.None);
            count = mbap[4];
            count <<= Byte;
            count += mbap[5];
            response = new byte[count - 1];
            sock.Receive(response, 0, response.Count(), SocketFlags.None);

            if (response[0] > BeginCodeError)
                throw new IOException("ModbusTCP error (" + (response[1]) + ")");
            return response;
        }
        protected override byte[] Read(byte function, ushort register, ushort count)
        {
            byte[] rtn;
            byte[] packet = MakePacket(function, register, count);
            byte[] mbap = MakeMBAP((ushort)packet.Count());
            byte[] response = SendReceive(mbap.Concat(packet).ToArray());
            rtn = new byte[response[1]];
            Array.Copy(response, 2, rtn, 0, rtn.Length);
            return rtn;
        }
        protected override void Write(byte function, ushort register, byte[] data)
        {
            byte[] packet;
            if (function == FuncForWrite)
                packet = MakePacket(function, register, data);
            else
                throw new IOException("Code function on write must 16");
            SendReceive(MakeMBAP((ushort)packet.Count()).Concat(packet).ToArray());
        }
    }
}
