using System;

namespace PCSC_Sample
{
    internal class ClassTLV
    {
        private byte[] tag_;
        public byte[] tag
        {
            get => tag_;
            private set => tag_ = value;
        }

        private int len_;
        public int len
        {
            get => len_;
            private set => len_ = value;
        }

        private byte[] val_;
        public byte[] val
        {
            get => val_;
            private set => val_ = value ?? new byte[0];
        }

        public static ClassTLV create(byte[] data, int tagLen, ref int offset)
        {
            ClassTLV obj = new ClassTLV();

            byte[] data2 = new byte[data.Length - offset];
            Array.Copy(data, offset, data2, 0, data.Length - offset);
            obj.tag = obj.getTag(data, tagLen, ref offset);
            obj.len = obj.getLen(data, ref offset);
            obj.val = obj.getVal(data, obj.len, ref offset);
            return obj;
        }

        private byte[] getTag(byte[] respData, int len, ref int offset)
        {
            byte[] result = new byte[len];
            for (int i = 0; i < len; i++)
            {
                result[i] = respData[offset + i];
            }
            offset += len;
            return result;
        }

        private int getLen(byte[] respData, ref int offset)
        {
            int len = 0;
            if ((respData[offset] & 0x80) == 0x80)
            {
                int dataLen = (int)(respData[offset] & 0x7f);
                offset++;

                for (int l = 0; l < dataLen; l++)
                {
                    len *= ((int)0x100 * l);
                    len += (int)respData[offset];
                    offset++;
                }
            }
            else
            {
                len = (int)respData[offset];
                offset++;
            }

            return len;
        }

        private byte[] getVal(byte[] respData, int len, ref int offset)
        {
            byte[] data = new byte[len];
            Array.Copy(respData, offset, data, 0, len);
            offset += len;
            return data;
        }

        public static void dispData(ClassTLV obj)
        {
            Console.Write("TAG:");
            dispRowData(obj.tag);
            Console.WriteLine("LEN:{0:X2}", obj.len);
            dispRowData(obj.val);
        }
        public static void dispRowData(byte[] data)
        {
            foreach (var d in data)
            {
                Console.Write("{0:X2}", d);
            }
            Console.WriteLine("");
        }
    }
}
