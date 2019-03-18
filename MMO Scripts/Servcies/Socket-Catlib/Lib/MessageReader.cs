// // ================================================================
// // FileName:MessageReader.cs
// // User: Baron
// // CreateTime:2/3/2018
// // Description: 读取流
// // ================================================================

using System;
using System.IO;
using System.Text;
using CatLib;

namespace HKLibrary
{
    public class MessageReader : IMessageReader
    {
        /// <summary>
        /// 转换float用的输出cache
        /// </summary>
        private byte[] floatConver = new byte[4];
        
        /// <summary>
        /// 写入流
        /// </summary>
        protected BinaryReader binaryReader;
        
        public IMessageReader BeginRead(BinaryReader _binaryReader)
        {
            binaryReader = _binaryReader;
            if (null == binaryReader)
            {
                this.Error(" 读取流为空，无法解析 ");
                return this;
            }
            return this;
        }

        public byte ReadByte()
        {
            return binaryReader.ReadByte();
        }

        public short ReadShort()
        {
            return binaryReader.ReadInt16().SwapInt16();
        }

        public ushort ReadUShort()
        {
            return binaryReader.ReadUInt16().SwapUInt16();
        }

        public int ReadInt()
        {
            return binaryReader.ReadInt32().SwapInt32();
        }

        public long ReadLong()
        {
            return binaryReader.ReadInt64().SwapInt64();
        }

        public float ReadFloat()
        {
            byte[] ins = binaryReader.ReadBytes(4);
            floatConver[0] = ins[3];
            floatConver[1] = ins[2];
            floatConver[2] = ins[1];
            floatConver[3] = ins[0];
            return BitConverter.ToSingle(floatConver, 0);
        }

        public string ReadString()
        {
            short length = ReadShort();
            var bytes = binaryReader.ReadBytes(length);
            string str = Encoding.UTF8.GetString(bytes);
            return str;
        }

        public bool ReadBool()
        {
            return binaryReader.ReadBoolean();
        }
    }
}