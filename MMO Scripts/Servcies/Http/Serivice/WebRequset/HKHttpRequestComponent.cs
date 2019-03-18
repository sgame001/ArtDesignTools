// // // ================================================================
// // // FileName:HKHttpRequestComponent.cs
// // // User: Baron
// // // CreateTime:2017-09-09-11:36
// // // Description:Http请求组件
// // // 
// // // Copyright (c) 2016 Heeking.Co.Ltd. All rights reserved.
// // // ================================================================

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Security.Policy;
using CatLib;
using CatLib.API.FileSystem;
using CI.HttpClient;
using UnityEngine;

namespace HKLibrary
{
    public class HKHttpRequestComponent : IHttpRequest
    {
        /// <summary>
        /// 用来缓存byte池
        /// </summary>
        private readonly List<byte> cacheBytes = new List<byte>();

        /// <summary>
        /// http请求
        /// </summary>
        private readonly HttpClient httpClient;

        /// <summary>
        /// 构造函数, 执行初始化操作
        /// </summary>
        public HKHttpRequestComponent()
        {
            httpClient = new HttpClient();
        }

        /// <summary>
        /// 通过http请求一个字符串
        /// 请求的可以是一个文本,也会以字符串形式返回
        /// </summary>
        /// <param name="_url"></param>
        /// <param name="_callback"></param>
        public void GetString(string _url, Action<string> _callback)
        {
            if (false == HKRequestTools.IsCheckUrl(_url))
            {
                return;
            }
            httpClient.GetString(new Uri(_url), response =>
            {
                if (null != _callback)
                {
                    if (true == response.IsSuccessStatusCode)
                    {
                        if (null != response.Data)
                        {
                            _callback(response.Data);
                        }
                    }
                    else
                    {
                        this.Error(StringCacheFactory.GetFree().Add("Download fail url = ")
                            .Add(_url).Add("\terror code = ").Add(response.StatusCode));
                        _callback(null);
                    }
                }
            });
        }

        /// <summary>
        /// 加载一个Asset Bundle,加载完后,会自动写入到可读写目录下
        /// 如果存在相同的文件,会被覆盖
        /// </summary>
        /// <param name="_url"></param>
        /// <param name="_progrss"></param>
        /// <param name="_compolete"></param>
        public void GetAssetBundle(string _url, Action<AssetBundle> _compolete = null, Action<int> _progrss = null)
        {
            if (false == HKRequestTools.IsCheckUrl(_url))
            {
                return;
            }
            cacheBytes.Clear();
            httpClient.GetByteArray(new Uri(_url), HttpCompletionOption.StreamResponseContent, response =>
            {
                if (null != _progrss)
                {
                    _progrss(response.PercentageComplete);
                }
                if (null != response.Data)
                {
                    cacheBytes.AddRange(response.Data);
                }
                if (response.PercentageComplete >= 100 && true == response.IsSuccessStatusCode)
                {
                    byte[] byteContent = cacheBytes.ToArray();

                    // 写入本地
                    HKRequestTools.WriteAssetBundleToLcoal(_url, byteContent);

                    //  如果需要回调,则创建一个AssetBundle进行回调
                    if (null != _compolete)
                    {
                        var assetBundle = AssetBundle.LoadFromMemory(byteContent);
                        _compolete(assetBundle);
                    }

                    // 清空队列
                    cacheBytes.Clear();
                }
            });
        }


        /// <summary>
        /// 获取byte数组数据
        /// </summary>
        /// <param name="_url"></param>
        /// <param name="_successEvent"></param>
        /// <param name="_failEvent"></param>
        /// <param name="_progress"></param>
        public void GetBytes(string _url, Action<byte[]> _successEvent = null, Action<HttpStatusCode> _failEvent = null,
            Action<int> _progress = null)
        {
            if (false == HKRequestTools.IsCheckUrl(_url))
            {
                return;
            }
            cacheBytes.Clear();
            httpClient.GetByteArray(new Uri(_url), HttpCompletionOption.StreamResponseContent, response =>
            {
                if (null != _progress)
                {
                    _progress(response.PercentageComplete);
                }
                if (null != response.Data)
                {
                    cacheBytes.AddRange(response.Data);
                }

                if (true == response.IsSuccessStatusCode)
                {
                    if (response.PercentageComplete >= 100)
                    {
                        byte[] byteContent = cacheBytes.ToArray();

                        //  如果需要回调,则创建一个AssetBundle进行回调
                        if (null != _successEvent)
                        {
                            _successEvent(byteContent);
                        }
                        // 清空队列
                        cacheBytes.Clear();
                    }
                }
                else
                {
                    if (null != _failEvent)
                    {
                        _failEvent(response.StatusCode);
                    }
                }
            });
        }
    }
}