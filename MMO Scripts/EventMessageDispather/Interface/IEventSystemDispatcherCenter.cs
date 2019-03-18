// // ================================================================
// // FileName:IEventSystemDispatcherCenter.cs
// // User: Baron
// // CreateTime:3/2/2018
// // Description: 消息广播中心
// // ================================================================
namespace HKLibrary
{
    public interface IEventSystemDispatcherCenter
    {
        /// <summary>
        /// 添加消息频道
        /// </summary>
        /// <param name="_channlName"></param>
        /// <returns></returns>
        IEventSystemChannel AddEventChannel(string _channlName);

        /// <summary>
        /// 添加消息频道
        /// </summary>
        /// <param name="_channel"></param>
        /// <returns></returns>
        IEventSystemChannel AddEventChannel(IEventSystemChannel _channel);

        /// <summary>
        /// 获取一个消息频道
        /// </summary>
        /// <param name="_channelName"></param>
        /// <returns></returns>
        IEventSystemChannel GetEventChannel(string _channelName);

        /// <summary>
        /// 创建一个Event 支持多个Tag
        /// </summary>
        /// <param name="_eventItemName"></param>
        /// <param name="_channelNames"></param>
        /// <returns></returns>
        IEventSystemItem AddEventItem(string _eventItemName, params string[] _channelNames);
        
        /// <summary>
        /// 添加一个消息对象
        /// </summary>
        /// <param name="_eventItem"></param>
        /// <param name="_channelName"></param>
        /// <returns></returns>
        IEventSystemItem AddEventItem(IEventSystemItem _eventItem, string _channelName = null);


        /// <summary>
        /// 添加一个消息对象
        /// </summary>
        /// <param name="_eventItem"></param>
        /// <param name="_eventChannel"></param>
        /// <returns></returns>
        IEventSystemItem AddEventItem(string _eventItem, IEventSystemChannel _eventChannel);

        /// <summary>
        /// 添加一个消息对象
        /// </summary>
        /// <param name="_eventItem"></param>
        /// <param name="_eventChannel"></param>
        /// <returns></returns>
        IEventSystemItem AddEventItem(IEventSystemItem _eventItem, IEventSystemChannel _eventChannel);

        /// <summary>
        /// 获取一个event item
        /// </summary>
        /// <param name="_eventName"></param>
        /// <returns></returns>
        IEventSystemItem GetEventItem(string _eventName);

        /// <summary>
        /// 删除一个event
        /// </summary>
        /// <param name="_eventItemName"></param>
        void RemoveEventItem(string _eventItemName);

        /// <summary>
        /// channel name
        /// </summary>
        /// <param name="_channelName"></param>
        void RemoveEventChannel(string _channelName);
    }
}