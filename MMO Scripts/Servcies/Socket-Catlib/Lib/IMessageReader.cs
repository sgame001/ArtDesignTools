// // ================================================================
// // FileName:IMessageReader.cs
// // User: Baron
// // CreateTime:2/3/2018
// // Description: 读取的消息接口
// // ================================================================

using System.IO;

namespace HKLibrary
{
    public interface IMessageReader
    {
        IMessageReader BeginRead(BinaryReader _binaryReader);
        
        byte ReadByte();

        short ReadShort();

        ushort ReadUShort();

        int ReadInt();

        long ReadLong();

        float ReadFloat();

        string ReadString();

        bool ReadBool();
    }
}