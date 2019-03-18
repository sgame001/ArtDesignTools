// // ================================================================
// // FileName:HKSpriteManager.cs
// // User: Baron-Fisher
// // CreateTime:2018 0303 8:19
// // Description:
// // Copyright (c) 2016 Greg.Co.Ltd. All rights reserved.
// // ================================================================

namespace HKLibrary
{
    public class EventSystemItemEmpty : EventSystemItem
    {
        /// <summary>
        /// 目标消息的名字
        /// 获取目标消息获取不到，才转到空消息这里
        /// </summary>
        public string TargetEventItemName { get; set; }
        
        public EventSystemItemEmpty(string _eventName, params string[] _channelTags) : base(_eventName, _channelTags)
        {
        }

        public override void BroadCast(object _args)
        {
//            this.Error("消息对象已销毁，广播失败 = " + TargetEventItemName);
        }

        public override object Trigger(object _args)
        {
            return null;
        }
    }
}