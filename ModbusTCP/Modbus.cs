using ModbusTCP.Enums;

namespace ModbusTCP
{
    public abstract class Modbus
    {
        protected byte address; // address slave
        private protected const byte FuncForWrite = 16;
        private protected const byte FuncForRead = 3;
        private protected const byte Byte = 8;
        private protected const byte UshortLenth = 2;
        private protected const byte FloatLenth = 4;
        private protected const byte IntLenth = 4;
        protected abstract byte[] Read(byte function, ushort register, ushort count);
        protected abstract void Write(byte function, ushort register, byte[] data);
        protected byte[] MakePacket(byte function, ushort register, ushort count)
        {
            return new byte[] {
                address,                //slave address
                function,               //function code
                (byte)(register >> Byte),  //start register high
                (byte)register,         //start register low
                (byte)(count >> Byte),     //# of registers high
                (byte)count             //# of registers low
            };
        }
        protected byte[] MakePacket(byte function, ushort register, byte[] data)
        {
            ushort count = (ushort)(data.Count() / UshortLenth);
            byte[] header = new byte[] {
                address,                //slave address
                function,               //function code
                (byte)(register >> Byte),  //start register high
                (byte)register,         //start register low
                (byte)(count >> Byte),     //# of registers high
                (byte)count,            //# of registers low
                (byte)data.Count()      //# of bytes to follow
            };
            return header.Concat(data).ToArray();
        }
        public float[] ReadHoldingFloat(ushort register, Endians endians = Endians.Endians_0123, ushort count = 1)
        {
            byte[] rVal = Read(FuncForRead, register, (ushort)(count * UshortLenth));
            float[] values = new float[rVal.Length / FloatLenth];
            for (int i = 0; i < rVal.Length; i += FloatLenth)
            {
                if (endians == Endians.Endians_2301)
                    values[i / FloatLenth] = BitConverter.ToSingle(new byte[] { rVal[i + 1], rVal[i], rVal[i + 3], rVal[i + 2] }, 0);
                else if (endians == Endians.Endians_0123)
                    values[i / FloatLenth] = BitConverter.ToSingle(new byte[] { rVal[i], rVal[i + 1], rVal[i + 2], rVal[i + 3] }, 0);
                else
                    values[i / FloatLenth] = BitConverter.ToSingle(new byte[] { rVal[i + 3], rVal[i + 2], rVal[i + 1], rVal[i] }, 0);
            }
            return values;
        }
        public int[] ReadHoldingInt(ushort register, Endians endians = Endians.Endians_0123, ushort count = 1)
        {
            byte[] rVal = Read(FuncForRead, register, (ushort)(count * UshortLenth));
            int[] values = new int[rVal.Length / IntLenth];
            for (int i = 0; i < rVal.Length; i += IntLenth)
            {
                if (endians == Endians.Endians_2301)
                    values[i / IntLenth] = BitConverter.ToInt32(new byte[] { rVal[i + 1], rVal[i], rVal[i + 3], rVal[i + 3] }, 0);
                else if (endians == Endians.Endians_0123)
                    values[i / IntLenth] = BitConverter.ToInt32(new byte[] { rVal[i], rVal[i + 1], rVal[i + 2], rVal[i + 3] }, 0);
                else
                    values[i / IntLenth] = BitConverter.ToInt32(new byte[] { rVal[i + 3], rVal[i + 2], rVal[i + 1], rVal[i] }, 0);
            }
            return values;
        }
        public ushort[] ReadHoldingUshort(ushort register, Endians endians = Endians.Endians_0123, ushort count = 1)
        {
            byte[] rVal = Read(FuncForRead, register, (ushort)(count * UshortLenth));
            ushort[] values = new ushort[rVal.Length / UshortLenth];
            for (int i = 0; i < rVal.Length; i += UshortLenth)
            {
                if (endians == Endians.Endians_2301)
                    values[i / UshortLenth] = BitConverter.ToUInt16(new byte[] { rVal[i], rVal[i + 1] }, 0);
                else
                    values[i / UshortLenth] = BitConverter.ToUInt16(new byte[] { rVal[i + 1], rVal[i] }, 0);
            }
            return values;
        }
        public void WriteHolding(ushort register, ushort[] data, Endians endians = Endians.Endians_0123)
        {
            byte[] bdata = new byte[data.Count() * UshortLenth];
            int i = 0;
            foreach (ushort item in data)
            {
               if(endians == Endians.Endians_0123)
                {
                    bdata[i] = (byte)(item >> Byte);
                    bdata[i + 1] = (byte)item;
                }
                else
                {
                    bdata[i + 1] = (byte)(item >> Byte);
                    bdata[i] = (byte)item;
                }
                i += UshortLenth;
            }
            Write(FuncForWrite, register, bdata);
        }
        public void WriteHolding(ushort register, ushort data, Endians endians = Endians.Endians_0123) => WriteHolding(register, new ushort[] { data }, endians);
        public void WriteHoldingFloat(ushort register, float[] data, Endians endians = Endians.Endians_0123)
        {
            byte[] bdata = new byte[data.Count() * FloatLenth];
            byte[] mydata;
            int i = 0;
            foreach (float item in data)
            {
                mydata = BitConverter.GetBytes(item);
                if (endians == Endians.Endians_0123)
                {
                    bdata[i] = mydata[0];
                    bdata[i + 1] = mydata[1];
                    bdata[i + 2] = mydata[2];
                    bdata[i + 3] = mydata[3];
                }
                else if (endians == Endians.Endians_2301)
                {
                    bdata[i + 1] = mydata[0];
                    bdata[i] = mydata[1];
                    bdata[i + 3] = mydata[2];
                    bdata[i + 2] = mydata[3];
                }
                else
                {
                    bdata[i + 3] = mydata[0];
                    bdata[i + 2] = mydata[1];
                    bdata[i + 1] = mydata[2];
                    bdata[i] = mydata[3];
                }
                i += FloatLenth;
            }
            Write(FuncForWrite, register, bdata);
        }
        public void WriteHoldingFloat(ushort register, float data, Endians endians = Endians.Endians_0123) => WriteHoldingFloat(register, new float[] { data }, endians);
        public void WriteHoldingFloat(ushort register, double data, Endians endians = Endians.Endians_0123) => WriteHoldingFloat(register, (float)data, endians);
    }
}
