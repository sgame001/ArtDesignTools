// // ================================================================
// // FileName:AppEvent.cs
// // User: Baron
// // CreateTime:3/2/2018
// // Description: App Event 提供静态方法，用来处理事件
// // ================================================================

using System;
using System.Collections.Generic;

namespace HKLibrary
{
    public class AppEvent
    {
        public static AppEvent AppEventHander;
        
        /// <summary>
        /// 空消息
        /// 如果某个消息被移除，获取时会返回这个空消息
        /// </summary>
        private static EventSystemItemEmpty emptyEventItem = new EventSystemItemEmpty("Empty Event");

        /// <summary>
        /// 空频道
        /// </summary>
        private static EventChannelEmpty emptyChannel = new EventChannelEmpty("Empty Channel");
        
        #region 触发

        /// <summary>
        /// 事件派发
        /// 返回事件自身，可以连续触发
        /// </summary>
        /// <param name="_eventItemName"></param>
        /// <param name="_args">可以为空的</param>
        public static IEventSystemItem BroadCastEvent(string _eventItemName, object _args = null)
        {
            if (true == string.IsNullOrEmpty(_eventItemName))
            {
                return null;
            }
            var eventItem = GetEvent(_eventItemName);
            eventItem.BroadCast(_args);
            return eventItem;
        }

        /// <summary>
        /// 带有返回值的触发
        /// </summary>
        /// <param name="_eventItemName"></param>
        /// <param name="_args"></param>
        /// <returns></returns>
        public static object TriggerEvent(string _eventItemName, object _args)
        {
            if (true == string.IsNullOrEmpty(_eventItemName) || null == _args)
            {
                return null;
            }

            var eventItem = GetEvent(_eventItemName);
            if (null != eventItem)
            {
                return eventItem.Trigger(_args);
            }

            return null;
        }

        /// <summary>
        /// 频道广播
        /// </summary>
        /// <param name="_channelName"></param>
        /// <param name="_args"></param>
        /// <returns></returns>
        public static IEventSystemChannel BroadCastChannelEvents(string _channelName, object _args)
        {
            if (true == string.IsNullOrEmpty(_channelName))
            {
                return null;
            }

            var channel = GetEventChannel(_channelName);
            channel.BroadCastEventItems(_args);
            return channel;
        }

        /// <summary>
        /// 频道派发
        /// </summary>
        /// <param name="_channelName"></param>
        /// <param name="_args"></param>
        /// <returns></returns>
        public static List<object> TriggerChannelEvents(string _channelName, object _args)
        {
            if (true == string.IsNullOrEmpty(_channelName))
            {
                return null;
            }
            
            var channel = GetEventChannel(_channelName);
            return channel.TriggerEventItems(_args);
        }

        #endregion
        
        #region 获取

        /// <summary>
        /// 获取事件channel
        /// </summary>
        /// <param name="_channelName"></param>
        /// <returns></returns>
        public static IEventSystemChannel GetEventChannel(string _channelName)
        {
            var eventChannel = EventSystemDispatcherCenter.Instance.GetEventChannel(_channelName);
            if (null == eventChannel)
            {
                emptyChannel.TargetChannelName = _channelName;
                return emptyChannel;
            }
            return eventChannel;
        }

        /// <summary>
        /// 获取事件对象
        /// </summary>
        /// <param name="_eventName"></param>
        /// <returns></returns>
        public static IEventSystemItem GetEvent(string _eventName)
        {
            var eventItem = EventSystemDispatcherCenter.Instance.GetEventItem(_eventName);
            if (null == eventItem)
            {
                emptyEventItem.TargetEventItemName = _eventName;
                return emptyEventItem;
            }
            return eventItem;
        }

        #endregion

        #region 监听

        /// <summary>
        /// 监听event
        /// </summary>
        /// <param name="_eventItemName"></param>
        /// <param name="_callback"></param>
        /// <param name="_channelNames"></param>
        /// <returns></returns>
        public static IEventSystemItem OnEvent(string _eventItemName, Action<object> _callback,  params string[] _channelNames)
        {
            if(null == _callback)
            {
                return null;
            }
            var eventItem = EventSystemDispatcherCenter.Instance.AddEventItem(_eventItemName, _channelNames);
            if (null != eventItem)
            {
                eventItem.Register(_callback);
            }
            return eventItem;
        }


        /// <summary>
        /// 添加一个事件，延迟做监听
        /// </summary>
        /// <param name="_eventItemName"></param>
        /// <param name="_channelName"></param>
        /// <returns></returns>
        public static IEventSystemItem AddEvent(string _eventItemName, string _channelName)
        {
            var eventItem = EventSystemDispatcherCenter.Instance.GetEventItem(_eventItemName); // 之所以没有直接调用Add，是为了防止不必要的param object[]开销
            if (null != eventItem)
            {
                return eventItem;
            }

            if (true == string.IsNullOrEmpty(_channelName))
            {
                _channelName = EventSystemDispatcherCenter.DEFAULT_CHNANEL;
            }
            eventItem = EventSystemDispatcherCenter.Instance.AddEventItem(_eventItemName, _channelName);
            return eventItem;
        }
        
        

        /// <summary>
        /// 监听channel
        /// </summary>
        /// <param name="_eventChanelName"></param>
        /// <param name="_callback"></param>
        /// <returns></returns>
        public static IEventSystemChannel OnChannel(string _eventChanelName, Action<string, object> _callback)
        {
            if (null == _callback)
            {
                return null;
            }
            var channel = EventSystemDispatcherCenter.Instance.AddEventChannel(_eventChanelName);
            if (null != channel)
            {
                channel.Register(_callback);
            }
            return channel;
        }

        /// <summary>
        /// 带有返回值的监听
        /// </summary>
        /// <param name="_eventItemName"></param>
        /// <param name="_funcCallback"></param>
        /// <param name="_channelNames"></param>
        /// <returns></returns>
        public static IEventSystemItem ListenerEvent(string _eventItemName, Func<object, object> _funcCallback,
            params string[] _channelNames)
        {
            if(null == _funcCallback)
            {
                return null;
            }
            var eventItem = EventSystemDispatcherCenter.Instance.AddEventItem(_eventItemName, _channelNames);
            if (null != eventItem)
            {
                eventItem.Listener(_funcCallback);
            }
            return eventItem;
        }


        /// <summary>
        /// 监听带返回值类型的频道
        /// </summary>
        /// <param name="_channelName"></param>
        /// <param name="_funcCallback"></param>
        /// <returns></returns>
        public static IEventSystemChannel ListenerChannel(string _channelName, Func<object, object> _funcCallback)
        {
            if (null == _funcCallback)
            {
                return null;
            }
            var channel = EventSystemDispatcherCenter.Instance.AddEventChannel(_channelName);

            if (null != channel)
            {
                channel.Listener(_funcCallback);
            }

            return channel;
        }
        
        #endregion

        #region 移除

        /// <summary>
        /// 通过名字移除一个事件
        /// </summary>
        /// <param name="_eventName"></param>
        public static void RemoveEvent(string _eventName)
        {
            EventSystemDispatcherCenter.Instance.RemoveEventItem(_eventName);
        }

        /// <summary>
        /// 通过指定EvnetItem移除一个事件
        /// </summary>
        /// <param name="_eventItem"></param>
        public static void RemoveEvent(IEventSystemItem _eventItem)
        {
            if (null == _eventItem)
            {
                return;
            }
            EventSystemDispatcherCenter.Instance.RemoveEventItem(_eventItem.EventName);
        }

        /// <summary>
        /// 通过名字移除一个消息频道
        /// </summary>
        /// <param name="_channelName"></param>
        public static void RemoveChannel(string _channelName)
        {
            EventSystemDispatcherCenter.Instance.RemoveEventChannel(_channelName);
        }

        /// <summary>
        /// 通过指定的channel移除
        /// </summary>
        /// <param name="_channel"></param>
        public static void RemoveChannel(IEventSystemChannel _channel)
        {
            if (null == _channel)
            {
                return;
            }
            RemoveChannel(_channel.ChannelName);
        }

        #endregion
        
    }
}