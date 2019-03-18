// // ================================================================
// // FileName:MessageWriter.cs
// // User: Baron
// // CreateTime:2/3/2018
// // Description: 写入流
// // ================================================================

using System;
using System.IO;
using System.Text;
using CatLib;
using CatLib.API.Network;

namespace HKLibrary
{
    public class MessageWriter : IMessageWriter
    {
        /// <summary>
        /// 写入流
        /// </summary>
        private MemoryStream writerStream;

        /// <summary>
        /// 二进制处理
        /// </summary>
        private BinaryWriter binaryWrite;

        /// <summary>
        /// 协议头标识
        /// </summary>
        private static short MESSAGE_HEAD_TAG = 127;

        /// <summary>
        /// 消息的默认长度
        /// </summary>
        private static short DEFAULT_SIZE = 1;

        /// <summary>
        /// log
        /// </summary>
        private StringListCacche LogCache = new StringListCacche();

        /// <summary>
        /// index
        /// </summary>
        private int logIndex = 0;

        /// <summary>
        /// 是否记录发送的Log
        /// </summary>
        private bool IsSendLog = false;

        /// <summary>
        /// 重置写入流
        /// </summary>
        private void ResetWriter()
        {
            if (null == writerStream)
            {
                writerStream = new MemoryStream();
            }
            else
            {
                writerStream.SetLength(0);
            }

            if (null == binaryWrite)
            {
                binaryWrite = new BinaryWriter(writerStream);
            }

            logIndex = 0;
            LogCache.SetSplit("\n");
        }

        /// <summary>
        /// 开始写入
        /// </summary>
        /// <param name="_messageId"></param>
        /// <returns></returns>
        public IMessageWriter WriteBegin(int _messageId)
        {
            ResetWriter();
            binaryWrite.Write(MESSAGE_HEAD_TAG.SwapInt16());
            binaryWrite.Write(DEFAULT_SIZE.SwapInt16());
            binaryWrite.Write(_messageId.SwapInt32());
            IsSendLog = LoggerQ.IsLogByMessageCode(_messageId);

            if (true == IsSendLog)
            {
                // 添加消息号
                AddSpecifyStringLogCache("SEND = " + _messageId + "   TIME : " +
                                         string.Format("[{0}]", DateTime.Now.TimeOfDay));
                LogCache.Add("—————————————————————");
            }

            return this;
        }

        public IMessageWriter AddByte(byte _value)
        {
            binaryWrite.Write(_value);
            AddLogCache(_value);
            return this;
        }

        public IMessageWriter AddShort(short _value)
        {
            binaryWrite.Write(_value.SwapInt16());
            AddLogCache(_value);
            return this;
        }

        public IMessageWriter AddUShort(ushort _value)
        {
            binaryWrite.Write(_value.SwapUInt16());
            AddLogCache(_value);
            return this;
        }

        public IMessageWriter AddInt(int _value)
        {
            binaryWrite.Write(_value.SwapInt32());
            AddLogCache(_value);
            return this;
        }

        public IMessageWriter AddLong(long _value)
        {
            binaryWrite.Write(_value.SwapInt64());
            AddLogCache(_value);
            return this;
        }

        public IMessageWriter AddFloat(float _value)
        {
            byte[] a = BitConverter.GetBytes(_value);
            a = Arr.Reverse(a);
            binaryWrite.Write(a);
            AddLogCache(_value);
            return this;
        }

        public IMessageWriter AddBool(bool _value)
        {
            binaryWrite.Write(_value);
            AddLogCache(_value);
            return this;
        }

        public IMessageWriter AddStr(string _value)
        {
            if (null == _value)
            {
                _value = "";
            }
            else
            {
                _value = _value.Trim();
            }

            byte[] bytes = Encoding.UTF8.GetBytes(_value);
            short  len   = (short) bytes.Length;
//            Add(len);
            binaryWrite.Write(len.SwapInt16());
            binaryWrite.Write(bytes);
            AddLogCache(_value);
            return this;
        }

        public void Send(INetworkChannel _channel)
        {
            if (null == _channel)
            {
                return;
            }

            var bodyLength = writerStream.Length - 4;            //标记的tag + 及默认Size的长度
            binaryWrite.Seek(2, SeekOrigin.Begin);               // 将写入位置放在2的位置
            binaryWrite.Write(((short) bodyLength).SwapInt16()); // 写入内容长度-实际内容长度，不包含包头
            var bytes     = writerStream.GetBuffer();
            var sendBytes = Arr.Slice(bytes, 0, (int) writerStream.Length);
            _channel.Send(sendBytes);

            // 输出打印
            if (true == IsSendLog)
            {
                LogCache.Add("message length = " + sendBytes.Length);
                this.Debug(LogCache.Release());
            }
        }


        /// <summary>
        /// 打印消息log
        /// </summary>
        /// <param name="_value"></param>
        private void AddLogCache(object _value)
        {
            if (LoggerQ.GameMakeEnv == GameMakeEnv.Dev)
            {
                if (true == _value is string && true == string.IsNullOrEmpty((string) _value))
                {
                    _value = "\"\"";
                }

                var printValue = _value;

                if (true == _value is string && (string) _value != "\"\"")
                {
                    printValue = string.Format("\"{0}\"", printValue);
                }

                LogCache.Add(string.Format("{2}:\t[{1}]\t{0}", printValue, _value.GetType().Name, logIndex));
                LogCache.Add("—————————————————————");
                logIndex++;
            }
        }

        /// <summary>
        /// 添加string形式的
        /// </summary>
        /// <param name="_value"></param>
        private void AddSpecifyStringLogCache(string _value)
        {
            if (LoggerQ.GameMakeEnv == GameMakeEnv.Dev)
            {
                LogCache.Add("\t" + _value);
            }
        }
    }
}