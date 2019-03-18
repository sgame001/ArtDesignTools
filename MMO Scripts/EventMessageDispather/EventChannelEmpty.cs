// // ================================================================
// // FileName:HKSpriteManager.cs
// // User: Baron-Fisher
// // CreateTime:2018 0303 8:32
// // Description:
// // Copyright (c) 2016 Greg.Co.Ltd. All rights reserved.
// // ================================================================

using System.Collections.Generic;

namespace HKLibrary
{
    public class EventChannelEmpty : EventSystemChannel
    {
        /// <summary>
        /// 目标渠道名称
        /// </summary>
        public string TargetChannelName { get; set; }
        
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="_channelName"></param>
        public EventChannelEmpty(string _channelName) : base(_channelName)
        {
        }

        public override void BroadCastEventItems(object _args)
        {
            this.Error("消息频道已关闭，广播失败 = " + TargetChannelName);
        }

        public override List<object> TriggerEventItems(object _args)
        {
            this.Error("消息频道已关闭，广播失败 = " + TargetChannelName);
            return null;
        }
    }
}