// // ================================================================
// // FileName:EventSystemItem.cs
// // User: Baron-Fisher
// // CreateTime:2018/3/2
// // Description:事件对象
// // Copyright (c) 2018 Greg.Co.Ltd. All rights reserved.
// // ================================================================
using System;
using System.Collections.Generic;

namespace HKLibrary
{
    public class EventSystemItem : IEventSystemItem, IEventDispose
    {
        /// <summary>
        /// 事件对象名称
        /// </summary>
        public string EventName { get; private set; }
        
        /// <summary>
        /// Tags
        /// </summary>
        private readonly List<string> channelTags = new List<string>();
        
        /// <summary>
        /// event事件回调集合
        /// </summary>
        private readonly List<Action<object>> eventCallbacks = new List<Action<object>>();

        /// <summary>
        /// func事件回调集合
        /// </summary>
        private readonly List<Func<object, object>> funcCallbacks = new List<Func<object, object>>();
        
        /// <summary>
        /// 构造
        /// </summary>
        public EventSystemItem(string _eventName, params string [] _channelTags)
        {
            EventName = _eventName;
            
            // 循环加入tag
            if (null != _channelTags)
            {
                for (var index = 0; index < _channelTags.Length; index++)
                {
                    var channelTag = _channelTags[index];
                    if (false == string.IsNullOrEmpty(channelTag))
                    {
                        AddChannelTag(channelTag);
                    }
                }
            }
        }
        
        /// <summary>
        /// 获取Tag队列
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public string[] GetChannelTags()
        {
            return channelTags.ToArray();
        }

        /// <summary>
        /// 加入一个tag
        /// </summary>
        /// <param name="_channelName"></param>
        /// <exception cref="NotImplementedException"></exception>
        public void AddChannelTag(string _channelName)
        {
            if (false == string.IsNullOrEmpty(_channelName))
            {
                channelTags.Add(_channelName);
            }
        }

        /// <summary>
        /// 移除tag
        /// </summary>
        /// <param name="_channelName"></param>
        public void RemoveChannelTag(string _channelName)
        {
            if (true == string.IsNullOrEmpty(_channelName))
            {
                return;
            }
            channelTags.Remove(_channelName);
        }

        /// <summary>
        /// 注册监听
        /// </summary>
        /// <param name="_args"></param>
        public void Register(Action<object> _args)
        {
            if (null != _args && false == eventCallbacks.Contains(_args))
            {
                eventCallbacks.Add(_args);
            }
        }

        /// <summary>
        /// 取消
        /// </summary>
        /// <param name="_args"></param>
        public void UnRegister(Action<object> _args)
        {
            if (true == eventCallbacks.Contains(_args))
            {
                eventCallbacks.Remove(_args);
            } 
        }

        /// <summary>
        /// 注册带返回值的监听
        /// </summary>
        /// <param name="_args"></param>
        /// <exception cref="NotImplementedException"></exception>
        public void Listener(Func<object, object> _args)
        {
            if (null != _args && false == funcCallbacks.Contains(_args))
            {
                funcCallbacks.Add(_args);
            }
        }

        /// <summary>
        /// 取消监听
        /// </summary>
        /// <param name="_args"></param>
        public void UnListener(Func<object, object> _args)
        {
            if (true == funcCallbacks.Contains(_args))
            {
                funcCallbacks.Remove(_args);
            }
        }

        /// <summary>
        /// 广播消息，是否冒泡给Channel
        /// </summary>
        /// <param name="_args"></param>
        /// <param name="_BubbleChannel"></param>
        public void BroadCastSelf(object _args, bool _BubbleChannel)
        {
            // 广播所有事件
            if (eventCallbacks.Count > 0)
            {
                for (var index = 0; index < eventCallbacks.Count; index++)
                {
                    var eventCallback = eventCallbacks[index];
                    if (null != eventCallback)
                    {
                        eventCallback(_args);
                    }
                }
            }

            if (true == _BubbleChannel)
            {
                // 监听了对应频道的也进行广播
                if (channelTags.Count > 0)
                {
                    for (var index = 0; index < channelTags.Count; index++)
                    {
                        var channelTag = channelTags[index];
                        var channel = AppEvent.GetEventChannel(channelTag);
                        if (null != channel)
                        {
                            ((EventSystemChannel)channel).BroadCastSelf(EventName, _args);
                        }
                    }
                }
            }
        }
        

        /// <summary>
        /// 广播消息
        /// </summary>
        /// <param name="_args"></param>
        /// <exception cref="NotImplementedException"></exception>
        public virtual void BroadCast(object _args)
        {
            BroadCastSelf(_args, true);
        }

        /// <summary>
        /// 触发带返回的消息
        /// </summary>
        /// <param name="_args"></param>
        /// <param name="_bubbleChanel"></param>
        public object TriggerSelf(object _args, bool _bubbleChanel)
        {
            if (funcCallbacks.Count > 0)
            {
                for (var index = 0; index < funcCallbacks.Count; index++)
                {
                    var funcCallback = funcCallbacks[index];
                    if (null != funcCallback)
                    {
                        return funcCallback(_args);
                    }
                }
            }

            if (true == _bubbleChanel)
            {
                for (var index = 0; index < channelTags.Count; index++)
                {
                    var channelTag = channelTags[index];
                    var channel = AppEvent.GetEventChannel(channelTag);
                    if (null != channel)
                    {
                        ((EventSystemChannel)channel).TriggerSelf(_args);
                    }
                }
            }
            return null;
        }
        

        /// <summary>
        /// 带有返回值的广播
        /// </summary>
        /// <param name="_args"></param>
        public virtual object Trigger(object _args)
        {
            return TriggerSelf(_args, true);
        }

        /// <summary>
        /// 获取回调数量
        /// </summary>
        /// <returns></returns>
        public int GetCallbackCount()
        {
            return eventCallbacks.Count + funcCallbacks.Count;
        }

        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            channelTags.Clear();
            eventCallbacks.Clear();
            funcCallbacks.Clear();
        }


        /// <summary>
        /// 获取callbacks
        /// </summary>
        /// <returns></returns>
        public List<Action<object>> GetReigsterCallbacks()
        {
            return eventCallbacks;
        }

        /// <summary>
        /// 获取带有回调的callbacks
        /// </summary>
        /// <returns></returns>
        public List<Func<object, object>> GetFuncCallbacks()
        {
            return funcCallbacks;
        }
        
    }
}