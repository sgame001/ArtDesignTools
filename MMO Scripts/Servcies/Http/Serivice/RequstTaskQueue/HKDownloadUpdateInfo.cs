// // ================================================================
// // FileName:HKSpriteManager.cs
// // User: Baron-Fisher
// // CreateTime:2017 0910 9:28
// // Description:封装一个下载时进度更新的对象信息
// // Copyright (c) 2016 Greg.Co.Ltd. All rights reserved.
// // ================================================================
namespace HKLibrary
{
    public class HKDownloadUpdateInfo
    {
        /// <summary>
        /// 文件名称
        /// </summary>
        public string FileName { get; set; }
        
        /// <summary>
        /// 当前下载的的索引
        /// </summary>
        public int CurrentIndex { get; set; }

        /// <summary>
        /// 任务队列总数
        /// </summary>
        public int TaskQueueCount { get; set; }
        
        /// <summary>
        /// 当前正在下载的的进度
        /// </summary>
        public int CurrentProgress { get; set; }

        /// <summary>
        /// 当前一帧的流量
        /// 用来累加计算下载速度
        /// </summary>
        public int currentFrameSize = 0;
        
        /// <summary>
        /// 重置下数据
        /// 因为可能会在一个可重用的对象中使用
        /// </summary>
        public void Reset()
        {
            FileName = "";
            CurrentIndex = 0;
            TaskQueueCount = 0;
            CurrentProgress = 0;
        }
    }
}