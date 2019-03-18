// // ================================================================
// // FileName:HKSpriteManager.cs
// // User: Baron-Fisher
// // CreateTime:2017 0909 9:39
// // Description:Http请求接口
// // Copyright (c) 2016 Greg.Co.Ltd. All rights reserved.
// // ================================================================

using System;
using UnityEngine;
using System.Collections;
using System.Net;

public interface IHttpRequest
{
    /// <summary>
    /// 获取一个string字符串
    /// 默认不处理进度更新
    /// </summary>
    /// <param name="_url">要加载的路径</param>
    /// <param name="_callback">回调</param>
    void GetString(string _url, Action<string> _callback);

    /// <summary>
    /// 获取一个Asset Bundle对象
    /// </summary>
    /// <param name="_url">加载的路径</param>
    /// <param name="_compolete">下载结果</param>
    /// <param name="_progrss">进度</param>
    void GetAssetBundle(string _url, Action<AssetBundle> _compolete = null, Action<int> _progrss = null);


    /// <summary>
    /// 获取一个bytes数组对象
    /// </summary>
    /// <param name="_url">远程路径</param>
    /// <param name="_successEvent">成功回调</param>
    /// <param name="_failEvent"></param>
    /// <param name="_progress">进度回调</param>
    void GetBytes(string _url, Action<byte[]> _successEvent = null, Action<HttpStatusCode> _failEvent = null, Action<int> _progress = null);
}