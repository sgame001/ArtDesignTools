// // // ================================================================
// // // FileName:IRequestTaskQueue.cs
// // // User: Baron
// // // CreateTime:2017-09-09-18:54
// // // Description:用于队列下载
// // // 
// // // Copyright (c) 2016 Heeking.Co.Ltd. All rights reserved.
// // // ================================================================

using System;

namespace HKLibrary
{
    public interface IRequestTaskQueue
    {
        /// <summary>
        /// 下载进度更新信息
        /// </summary>
        HKDownloadUpdateInfo UpdateInfo { get; }

        /// <summary>
        /// 设置下载相关的回调
        /// </summary>
        /// <param name="_downloadUpdateInfo"></param>
        /// <param name="_downloadComplete"></param>
        /// <param name="_stopDownload"></param>
        IRequestTaskQueue SetCallBack(Action<HKDownloadUpdateInfo> _downloadUpdateInfo, Action<string> _downloadComplete, Action<string> _stopDownload = null);

        /// <summary>
        /// 向任务队列中,添加一个下载任务
        /// </summary>
        /// <param name="_url">地址</param>
        /// <param name="_resType">资源类型</param>
        /// <param name="_complete">回调接口</param>
        /// <param name="_userData"></param>
        IRequestTaskQueue AddTask(string _url, DownloadResType _resType, Action<string, object, object> _complete/**url, data, user_data*/, object _userData);

        /// <summary>
        /// 开始下载
        /// </summary>
        void StartDownload();

        /// <summary>
        /// 暂停下载
        /// </summary>
        void StopDownload();
        
        /// <summary>
        /// 下载队列中的数量
        /// </summary>
        /// <returns></returns>
        int TaskCount();
    }
}