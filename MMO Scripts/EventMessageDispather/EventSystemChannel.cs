using System;
using System.Collections.Generic;

namespace HKLibrary
{
    public class EventSystemChannel : IEventSystemChannel, IEventDispose
    {
        /// <summary>
        /// 事件对象队列
        /// </summary>
        private readonly Dictionary<string, IEventSystemItem> eventItems = new Dictionary<string, IEventSystemItem>();
        
        /// <summary>
        /// event事件回调集合
        /// </summary>
        private readonly List<Action<string, object>> eventCallbacks = new List<Action<string, object>>();

        /// <summary>
        /// func事件回调集合
        /// </summary>
        private readonly List<Func<object, object>> funcCallbacks = new List<Func<object, object>>();

        /// <summary>
        /// 事件频道名称
        /// </summary>
        public string ChannelName { get; private set; }
        
        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="_channelName"></param>
        public EventSystemChannel(string _channelName)
        {
            ChannelName = _channelName;
        }

        
        /// <summary>
        /// 添加一个事件对象
        /// </summary>
        /// <param name="_eventtem"></param>
        public void AddEventItem(IEventSystemItem _eventtem)
        {
            if (null == _eventtem)
            {
                return;
            }

            if (false == eventItems.ContainsKey(_eventtem.EventName))
            {
                eventItems.Add(_eventtem.EventName, _eventtem);
            }
        }

        /// <summary>
        /// 移除event item
        /// </summary>
        /// <param name="_eventItem"></param>
        public void RemoveEventItem(IEventSystemItem _eventItem)
        {
            if (null == _eventItem)
            {
                return;
            }

            // 从队列中移除
            if (true == eventItems.ContainsKey(_eventItem.EventName))
            {
                eventItems.Remove(_eventItem.EventName);
            }
        }

        /// <summary>
        /// 带荷载的广播
        /// </summary>
        /// <param name="_args"></param>
        public virtual void BroadCastEventItems(object _args)
        {
            foreach (var eventSystemItem in eventItems)
            {
                if (null == eventSystemItem.Value)
                {
                    continue;
                }
                ((EventSystemItem) eventSystemItem.Value).BroadCastSelf(_args, false);
            }
        }

        /// <summary>
        /// 触发
        /// </summary>
        /// <param name="_args"></param>
        public virtual List<object> TriggerEventItems(object _args)
        {
            List<object> result = new List<object>();
            foreach (var eventSystemItem in eventItems)
            {
                if (null == eventSystemItem.Value)
                {
                    continue;
                }
                var resultObject = ((EventSystemItem) eventSystemItem.Value).TriggerSelf(_args, false);
                if (null != resultObject)
                {
                    result.Add(resultObject);
                }
            }
            return result;
        }

        /// <summary>
        /// 广播自身
        /// </summary>
        /// <param name="_eventItemName"></param>
        /// <param name="_args"></param>
        public void BroadCastSelf(string _eventItemName, object _args)
        {
            foreach (var eventCallback in eventCallbacks)
            {
                if (null != eventCallback)
                {
                    eventCallback(_eventItemName, _args);
                }
            }
        }

        /// <summary>
        /// 带有返回值的广播
        /// </summary>
        /// <param name="_args"></param>
        public object TriggerSelf(object _args)
        {
            foreach (var funcCallback in funcCallbacks)
            {
                if (null != funcCallback)
                {
                    return funcCallback(_args);
                }
            }
            return null;
        }

        /// <summary>
        /// 注册监听
        /// </summary>
        /// <param name="_args"></param>
        public void Register(Action<string, object> _args)
        {
            if (null != _args)
            {
                if (false == eventCallbacks.Contains(_args))
                {
                    eventCallbacks.Add(_args);
                }
            }
        }

        /// <summary>
        /// 注册带返回值的监听
        /// </summary>
        /// <param name="_args"></param>
        public void Listener(Func<object, object> _args)
        {
            if (null != _args)
            {
                if (false == funcCallbacks.Contains(_args))
                {
                    funcCallbacks.Add(_args);
                }
            }
        }

        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            eventItems.Clear();
            eventCallbacks.Clear();
            funcCallbacks.Clear();
        }
    }
}