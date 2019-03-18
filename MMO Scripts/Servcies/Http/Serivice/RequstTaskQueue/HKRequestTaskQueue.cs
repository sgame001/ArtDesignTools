// // // ================================================================
// // // FileName:HKRequsetTaskQueue.cs
// // // User: Baron
// // // CreateTime:2017-09-09-18:56
// // // Description:用于处理队列下载
// // // 
// // // Copyright (c) 2016 Heeking.Co.Ltd. All rights reserved.
// // // ================================================================

using System;
using System.Collections.Generic;
using CatLib;
using CI.HttpClient;
using UnityEngine;

namespace HKLibrary
{
    public class HKRequestTaskQueue : IRequestTaskQueue, ICanbeCache
    {
        /// <summary>
        /// http请求
        /// </summary>
        private HttpClient httpClient;

        /// <summary>
        /// 用来缓存byte池
        /// 创建一个内存池子反复使用，这种种情况只适合逐个下载的情况
        /// </summary>
        private readonly List<byte> cacheBytes = new List<byte>();


        /// <summary>
        /// 下载任务队列
        /// </summary>
        private readonly SortSet<HKDownloadTaskItem, int> downloadSortSet = new SortSet<HKDownloadTaskItem, int>();


        /// <summary>
        /// 错误信息缓存
        /// </summary>
        private StringListCacche errorInfoCache = null;

        /// <summary>
        /// 队列下载完成后的回调方法ScriptDBuild
        /// string 是错误信息 如果是成功下载，则不包含任何错误信息
        /// </summary>
        private Action<string> downloadQueueCompleteAction = null;

        /// <summary>
        /// 下载进度信息回调
        /// </summary>
        private Action<HKDownloadUpdateInfo> downloadUpateInfoAction = null;

        /// <summary>
        /// 取消下载的进度回调
        /// </summary>
        private Action<string> stopDownloadAction = null;
        
        /// <summary>
        /// 下载时的更新信息
        /// </summary>
        private HKDownloadUpdateInfo updateInfo = new HKDownloadUpdateInfo();

        public HKDownloadUpdateInfo UpdateInfo
        {
            get { return updateInfo; }
        }


        /// <summary>
        /// 构造函数
        /// </summary>
        public HKRequestTaskQueue()
        {
            Init();
        }


        /// <summary>
        /// init
        /// </summary>
        public void Init()
        {
            httpClient = new HttpClient();
        }


        /// <summary>
        /// 设置相关回调
        /// </summary>
        /// <param name="_downloadUpdateInfo"></param>
        /// <param name="_downloadComplete"></param>
        /// <param name="_stopDownload"></param>
        public IRequestTaskQueue SetCallBack(Action<HKDownloadUpdateInfo> _downloadUpdateInfo,
            Action<string> _downloadComplete, Action<string> _stopDownload = null)
        {
            downloadUpateInfoAction = _downloadUpdateInfo;
            downloadQueueCompleteAction = _downloadComplete;
            stopDownloadAction = _stopDownload;
            return this;
        }


        /// <summary>
        /// 向任务队列中,添加一个下载任务
        /// </summary>
        /// <param name="_url">地址</param>
        /// <param name="_resType">资源类型</param>
        /// <param name="_complete">回调接口</param>
        /// <param name="_userData"></param>
        public IRequestTaskQueue AddTask(string _url, DownloadResType _resType, Action<string, object, object> _complete, object _userData)
        {
            if (false == HKRequestTools.IsCheckUrl(_url))
            {
                return this;
            }

            var item = HKDownloadTaskItem.downloadItemCache.GetFree();
            if (null == item)
            {
                return this;
            }

            item.Url = _url;
            item.DownloadResType = _resType;
            item.downloadPriority = 0;
            item.CompleteAction = _complete;
            item.UserData = _userData;
            
            // 添加到待下载队列中
            downloadSortSet.Add(item, item.downloadPriority);
            return this;
        }

        /// <summary>
        /// 开始下载准备好的队列内容
        /// </summary>
        public void StartDownload()
        {
            UpdateInfo.TaskQueueCount = downloadSortSet.Count;
            ExecuteDownload();
        }

        
        /// <summary>
        /// 停止下载
        /// 取消当前正在下载的资源
        /// 清空整个队列
        /// </summary>
        public void StopDownload()
        {
            if (null != httpClient)
            {
                httpClient.Abort();
            }
            downloadSortSet.Clear();
        }


        /// <summary>
        /// 返回下载队列的数量
        /// </summary>
        /// <returns></returns>
        public int TaskCount()
        {
            if (null != downloadSortSet)
            {
                return downloadSortSet.Count;
            }

            return 0;
        }


        /// <summary>
        /// 执行下载
        /// 因为会在多处调用，所以单独封装起来
        /// </summary>
        private void ExecuteDownload()
        {
            if (downloadSortSet.Count > 0)
            {
                var item = downloadSortSet.Pop(); // 从末尾弹出一个，因为末尾的是优先级最高的
                if (null != item)
                {
                    // 更新一下当前数量，以及文件名称
                    UpdateInfo.CurrentIndex = updateInfo.TaskQueueCount - downloadSortSet.Count; // 总数减去剩余的
                    UpdateInfo.FileName = item.FileName;

                    // 创建uri并进行下载
                    Uri uri = new Uri(item.Url);
                    if (item.DownloadResType == DownloadResType.StringContent)
                    {
                        httpClient.GetString(uri, _response =>
                        {
                            if (true == _response.IsSuccessStatusCode)
                            {
                                DownloadSuccessExecute(item, _response.Data);
                            }
                            else
                            {
                                DownloadFailExecute(item, (int) _response.StatusCode);
                            }
                        });
                    }
                    else if (item.DownloadResType == DownloadResType.AssetBundle ||
                             item.DownloadResType == DownloadResType.ByteArray)
                    {
                        cacheBytes.Clear();
                        httpClient.GetByteArray(uri, HttpCompletionOption.StreamResponseContent, _response =>
                        {

                            if (false == _response.IsSuccessStatusCode)
                            {
                                DownloadFailExecute(item, (int) _response.StatusCode);
                            }
                            else
                            {
                                cacheBytes.AddRange(_response.Data);
                                if (true == _response.PercentageComplete >= 100)
                                {
                                    if (true == _response.IsSuccessStatusCode)
                                    {
                                        DownloadSuccessExecute(item, cacheBytes.ToArray());
                                    }
                                }
                                else
                                {
                                    DownloadProgressExecute(item, _response.PercentageComplete, _response.Data.Length);
                                }
                            }
                        });
                    }
                }
            }
        }


        /// <summary>
        /// 下载过程中进度更新
        /// </summary>
        /// <param name="_item">当前正在下载的对象</param>
        /// <param name="_progresss">当前下载对象的下载进度，百分比形式 0 - 100的整数值</param>
        /// <param name="_currentFrameByesCount">当前帧的流量</param>
        private void DownloadProgressExecute(HKDownloadTaskItem _item, int _progresss, int _currentFrameByesCount)
        {
            if (null == _item)
            {
                return;
            }
            UpdateInfo.CurrentProgress = _progresss;
            updateInfo.currentFrameSize = _currentFrameByesCount;
            
            if (null != downloadUpateInfoAction)
            {
                downloadUpateInfoAction(UpdateInfo);
            }
        }


        /// <summary>
        /// 下载成功处理
        /// </summary>
        private void DownloadSuccessExecute(HKDownloadTaskItem _item, object _data)
        {
            if (null != _item)
            {
                _item.DownloadResult = DownloadResult.Success;

                updateInfo.CurrentProgress = 100; // 更新一次满进度，主要考虑到UI相关的更新
                if (null != downloadUpateInfoAction)
                {
                    downloadUpateInfoAction(updateInfo);
                }

                if (_item.DownloadResType == DownloadResType.StringContent) // string 类型
                {
                    if (null != _item.CompleteAction)
                    {
                        _item.CompleteAction(_item.Url, _data as string, _item.UserData);
                    }
                }
                else if (_item.DownloadResType == DownloadResType.AssetBundle) // AssetBundle类型
                {
                    if (true == _data is byte[])
                    {
                        // 将数据写入到本地
                        HKRequestTools.WriteAssetBundleToLcoal(_item.Url, (byte[]) _data);

                        // 处理回调
                        if (null != _item.CompleteAction)
                        {
                            var assetBundle = AssetBundle.LoadFromMemory((byte[]) _data);
                            _item.CompleteAction(_item.Url,assetBundle, _item.UserData);
                        }
                    }
                }else if (_item.DownloadResType == DownloadResType.ByteArray) // 二进制类型
                {
                    if (true == _data is byte[])
                    {
                        // 处理回调
                        if (null != _item.CompleteAction)
                        {
                            _item.CompleteAction(_item.Url, _data, _item.UserData);
                        }
                    }
                }

                // 使用完后，就回收到内存池中
                HKDownloadTaskItem.downloadItemCache.Recover(_item);
            }

            // 检测当前剩余下载对象，并继续下载
            CheckQueueComplete();
        }


        /// <summary>
        /// 资源下载失败的处理
        /// 要向资源错误原因中添加一条错误记录
        /// </summary>
        /// <param name="_item"></param>
        /// <param name="_errorCode"></param>
        private void DownloadFailExecute(HKDownloadTaskItem _item, int _errorCode)
        {
            if (null == errorInfoCache)
            {
                errorInfoCache = StringCacheFactory.GetFree();
                errorInfoCache.SetSplit("\n");
            }

            if (_item.CurrentSurplusRetryCount > 0)
            {
                RetryExecute(_item);
                errorInfoCache.Add(_item.Url).Add(" 下载失败，启动第").Add(_item.RetryCount - _item.CurrentSurplusRetryCount)
                    .Add("次重试");
            }
            else
            {
                errorInfoCache.Add(_item.Url).Add(" 下载失败 = ").Add("_").Add(_errorCode);
                // 使用完后，就回收到内存池中
                HKDownloadTaskItem.downloadItemCache.Recover(_item);
            }

            // 检测当前剩余下载对象，并继续下载
            CheckQueueComplete();
        }


        /// <summary>
        /// 重试处理
        /// </summary>
        /// <param name="_item"></param>
        private void RetryExecute(HKDownloadTaskItem _item)
        {
            _item.CurrentSurplusRetryCount--;
            downloadSortSet.Add(_item, _item.downloadPriority); // 重新加入到队列中
        }


        /// <summary>
        /// 检测下载队列是否完成
        /// </summary>
        private void CheckQueueComplete()
        {
            if (downloadSortSet.Count == 0)
            {
                // 下载完成，通知进行回调
                if (null != downloadQueueCompleteAction)
                {
                    string errorMessage = null == errorInfoCache ? null : errorInfoCache.Release();
                    downloadQueueCompleteAction(errorMessage);
                }
            }
            else
            {
                // 继续下载下一个
                ExecuteDownload();
            }
        }


        /// <summary>
        /// Reset
        /// </summary>
        public void Reset()
        {
            cacheBytes.Clear();
            downloadSortSet.Clear();
            if (null != errorInfoCache)
            {
                errorInfoCache.Release();
                errorInfoCache = null;
            }
            UpdateInfo.Reset();
            downloadQueueCompleteAction = null;
            downloadUpateInfoAction = null;
        }
    }
}