// // ================================================================
// // FileName:IEventSystemChannel.cs
// // User: Baron
// // CreateTime:3/2/2018
// // Description: 消息频道
// // ================================================================

using System;
using System.Collections.Generic;

namespace HKLibrary
{
    public interface IEventSystemChannel
    {
        /// <summary>
        /// 消息频道唯一名字
        /// </summary>
        string ChannelName { get;}

        /// <summary>
        /// 添加一个Event Item到队列中
        /// </summary>
        /// <param name="_eventtem"></param>
        void AddEventItem(IEventSystemItem _eventtem);

        /// <summary>
        /// 从channel队列中将指定event item删除
        /// </summary>
        /// <param name="_eventItem"></param>
        void RemoveEventItem(IEventSystemItem _eventItem);
        
        /// <summary>
        /// 带有载荷版本的时间广播
        /// </summary>
        /// <param name="_args"></param>
        void BroadCastEventItems(object _args);


        /// <summary>
        /// 触发返回值带有载荷的消息对象
        /// </summary>
        /// <param name="_args"></param>
        List<object> TriggerEventItems(object _args);

//        /// <summary>
//        /// 带有返回值的广播
//        /// </summary>
//        /// <param name="_args"></param>
//        object TriggerSelf(object _args);
        
        /// <summary>
        /// 添加一个监听
        /// 会监听所有包含当前tag的event item
        /// </summary>
        /// <param name="_args"></param>
        void Register(Action<string, object> _args);

        /// <summary>
        /// 带有返回值版本的监听
        /// </summary>
        /// <param name="_args"></param>
        /// <returns></returns>
        void Listener(Func<object, object> _args);
        
    }
}