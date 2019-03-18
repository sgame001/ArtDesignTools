// // ================================================================
// // FileName:IEventSystemDispatcherCenter.cs
// // User: Baron
// // CreateTime:3/2/2018
// // Description: 消息广播中心
// // ================================================================

using System.Collections.Generic;
using CatLib;

namespace HKLibrary
{
    public class EventSystemDispatcherCenter : HKSingletonMonoBehaviour<EventSystemDispatcherCenter>/**为了提高调用效率，所以使用了这种单例方式*/, IEventSystemDispatcherCenter
    {
        /// <summary>
        /// 默认的频道名称
        /// </summary>
        public const string DEFAULT_CHNANEL = "Default_Channel";

        /// <summary>
        /// 全局频道名称
        /// </summary>
        public const string GLOBAL_CHNNEL = "Global_Channel";
        
        /// <summary>
        /// 消息频道队列
        /// </summary>
        private Dictionary<string, IEventSystemChannel> channelDic = new Dictionary<string, IEventSystemChannel>();
        
        /// <summary>
        /// 消息对象队列
        /// </summary>
        private Dictionary<string, IEventSystemItem> eventItemDic = new Dictionary<string, IEventSystemItem>();
        
        
        /// <summary>
        /// 添加消息频道
        /// </summary>
        /// <param name="_channlName"></param>
        /// <returns></returns>
        public IEventSystemChannel AddEventChannel(string _channlName)
        {
            if (true == string.IsNullOrEmpty(_channlName))
            {
                return null;
            }

            IEventSystemChannel eventChannel = null;
            if (false == channelDic.ContainsKey(_channlName))
            {
                eventChannel = new EventSystemChannel(_channlName);
                channelDic.Add(_channlName, eventChannel);
                ChannelTagCheck(eventChannel);
            }
            else
            {
                this.Info("已经存在相同key的channel = " + _channlName);
                eventChannel = channelDic[_channlName];
            }
            return eventChannel;
        }

       /// <summary>
       /// 添加一个channel
       /// </summary>
       /// <param name="_channel"></param>
       /// <returns></returns>
        public IEventSystemChannel AddEventChannel(IEventSystemChannel _channel)
        {
            if (null == _channel)
            {
                return null;
            }

            if (true == channelDic.ContainsKey(_channel.ChannelName))
            {
                return _channel;
            }

            ChannelTagCheck(_channel);
            channelDic.Add(_channel.ChannelName, _channel);
            return _channel;
        }

        /// <summary>
        /// 获取一个Channel
        /// </summary>
        /// <param name="_channelName"></param>
        /// <returns></returns>
        public IEventSystemChannel GetEventChannel(string _channelName)
        {
            if (true == channelDic.ContainsKey(_channelName))
            {
                return channelDic[_channelName];
            }
            return null;
        }


        /// <summary>
        /// 根据 事件名字，和事件频道，添加一个具体的事件
        /// </summary>
        /// <param name="_eventItemName"></param>
        /// <param name="_channelNames"></param>
        /// <returns></returns>
        public IEventSystemItem AddEventItem(string _eventItemName, params string[] _channelNames)
        {
            if (true == string.IsNullOrEmpty(_eventItemName))
            {
                return null;
            }

            if (null == _channelNames)
            {
                return null;
            }
            
            // 检测是否已经存在event item
            IEventSystemItem eventItem = GetEventItem(_eventItemName);

            if (null != eventItem)
            {
                return eventItem;
            }
            
            // 检测 chanenl
            foreach (var channelName in _channelNames)
            {
                IEventSystemChannel eventSystemChannel = GetEventChannel(channelName);
                if (null == eventSystemChannel)
                {
                    var channel = AddEventChannel(channelName);
                    if (null == channel)
                    {
                        this.Error(string.Format("创建Event Item[{0}]时，对应的Channel[{1}]创建失败", _eventItemName, channelName));
                    }
                }
            }
            
            // 创建event item
            eventItem = new EventSystemItem(_eventItemName, _channelNames);
            eventItemDic.Add(eventItem.EventName, eventItem);
            EventItemCheck(eventItem);
            return eventItem;
        }

        /// <summary>
        /// 添加event item
        /// </summary>
        /// <param name="_eventItem"></param>
        /// <param name="_channelName"></param>
        /// <returns></returns>
        public IEventSystemItem AddEventItem(IEventSystemItem _eventItem, string _channelName = null)
        {
            if (null == _eventItem)
            {
                return null;
            }

            return AddEventItem(_eventItem.EventName, _channelName);
        }

        /// <summary>
        /// 添加event item
        /// </summary>
        /// <param name="_eventItem"></param>
        /// <param name="_eventChannel"></param>
        /// <returns></returns>
        public IEventSystemItem AddEventItem(string _eventItem, IEventSystemChannel _eventChannel)
        {
            if (null == _eventChannel)
            {
                return null;
            }
            return AddEventItem(_eventItem, _eventChannel.ChannelName);
        }
        
        
        /// <summary>
        /// 添加event item
        /// </summary>
        /// <param name="_eventItem"></param>
        /// <param name="_eventChannel"></param>
        /// <returns></returns>
        public IEventSystemItem AddEventItem(IEventSystemItem _eventItem, IEventSystemChannel _eventChannel)
        {
            if (null == _eventItem || null == _eventChannel)
            {
                return null;
            }

            return AddEventItem(_eventItem, _eventChannel);
        }

        
        /// <summary>
        /// 获取event item
        /// </summary>
        /// <param name="_eventName"></param>
        /// <returns></returns>
        public IEventSystemItem GetEventItem(string _eventName)
        {
            if (true == string.IsNullOrEmpty(_eventName))
            {
                return null;
            }

            if (true == eventItemDic.ContainsKey(_eventName))
            {
                return eventItemDic[_eventName];
            }

            return null;
        }

        /// <summary>
        /// 删除一个event
        /// </summary>
        /// <param name="_eventItemName"></param>
        public void RemoveEventItem(string _eventItemName)
        {
            if (true == string.IsNullOrEmpty(_eventItemName))
            {
                return;
            }

            if (true == eventItemDic.ContainsKey(_eventItemName))
            {
                IEventSystemItem eventItem = eventItemDic[_eventItemName];
                if (null != eventItem)
                {
                    RemoveEventItemInChannels(eventItem);
                    ((IEventDispose)eventItem).Dispose();
                }
                eventItemDic.Remove(_eventItemName);
            }
        }

        /// <summary>
        /// channel name
        /// </summary>
        /// <param name="_channelName"></param>
        public void RemoveEventChannel(string _channelName)
        {
            if (true == string.IsNullOrEmpty(_channelName))
            {
                return;
            }

            if (true == channelDic.ContainsKey(_channelName))
            {
                IEventSystemChannel eventChannel = channelDic[_channelName];
                if (null != eventChannel)
                {
                    RemoveChannelTagInEventItems(eventChannel);
                    ((IEventDispose)eventChannel).Dispose();
                }
                channelDic.Remove(_channelName);
            }
        }


        /// <summary>
        /// 检测已有的event item中是否存在和自己tag相同的，加入到自己的队中
        /// </summary>
        /// <param name="_channel"></param>
        private void ChannelTagCheck(IEventSystemChannel _channel)
        {
            if (null == _channel)
            {
                return;
            }

            foreach (var eventSystemItem in eventItemDic)
            {
                if (null != eventSystemItem.Value)
                {
                    if (-1 != Arr.IndexOf(eventSystemItem.Value.GetChannelTags(), _channel.ChannelName))
                    {
                        _channel.AddEventItem(eventSystemItem.Value);
                    }
                }
            }
        }

        /// <summary>
        /// 创建Event Item时检测，将自己的tag加入到对应的队列中
        /// </summary>
        /// <param name="_eventSystemItem"></param>
        private void EventItemCheck(IEventSystemItem _eventSystemItem)
        {
            if (null == _eventSystemItem)
            {
                return;
            }

            foreach (var channelTag in _eventSystemItem.GetChannelTags())
            {
                if (false == string.IsNullOrEmpty(channelTag))
                {
                    var channel = GetEventChannel(channelTag);
                    if (null != channel)
                    {
                        channel.AddEventItem(_eventSystemItem);
                    }
                }
            }
        }

        /// <summary>
        /// 从所有Channel里遍历当前要移除的EventItem
        /// </summary>
        /// <param name="_eventItem"></param>
        private void RemoveEventItemInChannels(IEventSystemItem _eventItem)
        {
            if (null == _eventItem)
            {
                return;
            }

            foreach (var eventSystemChannel in channelDic)
            {
                if (null != eventSystemChannel.Value)
                {
                    eventSystemChannel.Value.RemoveEventItem(_eventItem);
                }
            }
        }

        /// <summary>
        /// 从所有Event Items中移除_channel.name名字的标签
        /// </summary>
        /// <param name="_channel"></param>
        private void RemoveChannelTagInEventItems(IEventSystemChannel _channel)
        {
            if (null == _channel)
            {
                return;
            }

            foreach (var eventSystemItem in eventItemDic)
            {
                if (null != eventSystemItem.Value)
                {
                    eventSystemItem.Value.RemoveChannelTag(_channel.ChannelName);
                }
            }
        }

        /// <summary>
        /// 获取所有事件
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, IEventSystemItem> GetDispatcherEvents()
        {
            return eventItemDic;
        }
        
    }
}