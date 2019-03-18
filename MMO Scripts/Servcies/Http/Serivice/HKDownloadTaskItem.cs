// // // ================================================================
// // // FileName:HKDownloadTaskItem.cs
// // // User: Baron
// // // CreateTime:2017-09-09-16:59
// // // Description:下载任务对象
// // // 
// // // Copyright (c) 2016 Heeking.Co.Ltd. All rights reserved.
// // // ================================================================

using System;

namespace HKLibrary
{
    /// <summary>
    /// 下载阶段
    /// </summary>
    public enum DownloadStage
    {
        None, // 没有被使用中,可能曾经创建过,现在处在缓存池中
        Ready,
        Downloading,
        Complete
    }

    /// <summary>
    /// 下载结果
    /// </summary>
    public enum DownloadResult
    {
        Success,
        Fail
    }


    /// <summary>
    /// 下载类型, 会根据下载类型 选择不同的处理方式
    /// </summary>
    public enum DownloadResType
    {
        ByteArray, // 字节流
        StringContent, // 字符串
        AssetBundle, // AssetBundle
    }

    public class HKDownloadTaskItem : ICanbeCache
    {
        /// <summary>
        /// 下载对象的Item
        /// </summary>
        public static HKCachePool<HKDownloadTaskItem> downloadItemCache = new HKCachePool<HKDownloadTaskItem>(100);

        /// <summary>
        /// 下载路径
        /// </summary>
        private string url = "";

        public string Url
        {
            get { return url; }
            set
            {
                url = value;

                if (false == string.IsNullOrEmpty(url))
                {
                    fileName = System.IO.Path.GetFileName(url);
                }
            }
        }

        /// <summary>
        /// 文件名称
        /// </summary>
        private string fileName = "";

        public string FileName
        {
            get { return fileName; }
        }

        /// <summary>
        /// 下载回调
        /// </summary>
        public Action<string, object, object> CompleteAction { get; set; }

        /// <summary>
        /// 是否在下载中
        /// </summary>
        public DownloadStage Stage { get; set; }

        /// <summary>
        /// 下载结果
        /// </summary>
        public DownloadResult DownloadResult { get; set; }

        /// <summary>
        /// 下载类型
        /// </summary>
        public DownloadResType DownloadResType { get; set; }

        /// <summary>
        /// 下载的优先级,优先级越高,会越先下载 默认是0
        /// </summary>
        public int downloadPriority { get; set; }

        /// <summary>
        /// 设定的重试次数
        /// </summary>
        private int retryCount = 0;

        public int RetryCount
        {
            get { return retryCount; }
        }

        /// <summary>
        /// user data
        /// </summary>
        public object UserData { get; set; }
        
        public HKDownloadTaskItem SetRetryCount(int _count)
        {
            retryCount = _count;
            CurrentSurplusRetryCount = RetryCount;
            return this;
        }

        /// <summary>
        /// 当前尝试剩余的重试次数
        /// </summary>
        public int CurrentSurplusRetryCount { get; set; }


        /// <summary>
        /// 由于缓存池需要,必须给一个默认构造
        /// </summary>
        public HKDownloadTaskItem()
        {
            Init();
        }

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="_url">需要下载的路径</param>
        public HKDownloadTaskItem(string _url)
        {
            Url = _url;
            Init();
        }

        /// <summary>
        /// init
        /// </summary>
        public void Init()
        {
            Stage = DownloadStage.Ready; // 默认进入ready阶段
        }

        /// <summary>
        /// 重置
        /// </summary>
        public void Reset()
        {
            Url = null;
            CompleteAction = null;
            Stage = DownloadStage.None;
            DownloadResult = DownloadResult.Fail;
            DownloadResType = DownloadResType.ByteArray;
            CurrentSurplusRetryCount = 0;
            downloadPriority = 0;
            UserData = null;
        }
    }
}