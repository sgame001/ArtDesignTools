// // ================================================================
// // FileName:LoggerQConfig.cs
// // User: Baron
// // CreateTime:2018/6/27
// // Description: Logger Q 配置
// // ================================================================

using System.Collections.Generic;
using UnityEngine;

public class LoggerQConfig : ScriptableObject
{
    /// <summary>
    /// 发送的消息队列
    /// </summary>
    public List<LogNetMessageItem> SendMessageItemes = new List<LogNetMessageItem>();

    /// <summary>
    /// 接受的消息队列
    /// </summary>
    public List<LogNetMessageItem> ReceiverMesasgeItems = new List<LogNetMessageItem>();
}

[System.Serializable]
public class LogNetMessageItem
{
    /// <summary>
    /// 网络消息号
    /// </summary>
    public int MessageCode;

    /// <summary>
    /// 协议名字（方便查看）
    /// </summary>
    public string MessageName;
    
    /// <summary>
    /// 是否输出log
    /// </summary>
    public bool IsLog;
}