// // ================================================================
// // FileName:IEventSystemItem.cs
// // User: Baron
// // CreateTime:3/2/2018
// // Description: 消息事件最基本的单元，每个单元包含唯一的消息key
// // ================================================================

using System;

namespace HKLibrary
{
    public interface IEventSystemItem
    {
        /// <summary>
        /// 消息key
        /// 只读，在构造函数时就已经定义好，不可修改
        /// </summary>
        string EventName { get; }
        
        /// <summary>
        /// 获取当前频道队列
        /// </summary>
        /// <returns></returns>
        string[] GetChannelTags();

        /// <summary>
        /// 为当前event item 补充一个channel
        /// </summary>
        /// <param name="_channelName"></param>
        void AddChannelTag(string _channelName);

        /// <summary>
        /// 删除一个Channel Tag
        /// </summary>
        /// <param name="_channelName"></param>
        void RemoveChannelTag(string _channelName);

        /// <summary>
        /// 监听消息
        /// </summary>
        /// <param name="_args"></param>
        void Register(Action<object> _args);

        /// <summary>
        /// 取消注册
        /// </summary>
        /// <param name="_args"></param>
        void UnRegister(Action<object> _args);
        
        /// <summary>
        /// 带有返回值的监听
        /// </summary>
        /// <param name="_args"></param>
        void Listener(Func<object, object> _args);

        /// <summary>
        /// 取消监听
        /// </summary>
        /// <param name="_args"></param>
        void UnListener(Func<object, object> _args);
        
        /// <summary>
        /// 带有载荷的消息广播
        /// </summary>
        /// <param name="_args"></param>
        void BroadCast(object _args);

        /// <summary>
        /// 带有返回值的广播
        /// </summary>
        /// <param name="_args"></param>
        object Trigger(object _args);

        /// <summary>
        /// 获取总监听数量
        /// action + func
        /// </summary>
        /// <returns></returns>
        int GetCallbackCount();
    }
    
}