// // // ================================================================
// // // FileName:IDownloadItem.cs
// // // User: Baron
// // // CreateTime:2017-09-28-16:27
// // // Description:
// // // 
// // // Copyright (c) 2016 Heeking.Co.Ltd. All rights reserved.
// // // ================================================================

namespace HKLibrary
{
    public interface IDownloadItem : ICanbeCache
    {
        /// <summary>
        /// 下载任务的唯一id
        /// </summary>
        int SerialId { get; set; }

        /// <summary>
        /// 远程路径
        /// </summary>
        string RemoteUrl { get; set; }

        /// <summary>
        /// 本地路径
        /// </summary>
        string LocalUrl { get; set; }

        /// <summary>
        /// 用户自定义数据
        /// </summary>
        object UserData { get; set; }

        /// <summary>
        /// 下载后的字符串数据
        /// </summary>
        string ResponseString { get; set; }

        /// <summary>
        /// 下载后的byte数据
        /// </summary>
        byte[] ResponseData { get; set; }

        /// <summary>
        /// 下载进度
        /// </summary>
        float DownloadProgress { get; set; }
    }
}