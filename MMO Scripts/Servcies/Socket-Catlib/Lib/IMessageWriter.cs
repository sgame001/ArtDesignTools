// // ================================================================
// // FileName:IMessageWriter.cs
// // User: Baron
// // CreateTime:2/3/2018
// // Description: 写入的消息接口
// // ================================================================

using CatLib.API.Network;

namespace HKLibrary
{
    public interface IMessageWriter
    {
        /// <summary>
        /// 开始写入
        /// </summary>
        /// <param name="_messageId"></param>
        /// <returns></returns>
        IMessageWriter WriteBegin(int _messageId);
        
        IMessageWriter AddByte(byte _value);

        IMessageWriter AddShort(short _value);

        IMessageWriter AddUShort(ushort _value);

        IMessageWriter AddInt(int _value);

        IMessageWriter AddLong(long _value);
        
        IMessageWriter AddFloat(float _value);

        IMessageWriter AddBool(bool _value);

        IMessageWriter AddStr(string _value);
        
        void Send(INetworkChannel _channel);
    }
}