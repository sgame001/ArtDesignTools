// // ================================================================
// // FileName:HKSpriteManager.cs
// // User: Baron-Fisher
// // CreateTime:2017 0910 12:39
// // Description:
// // Copyright (c) 2016 Greg.Co.Ltd. All rights reserved.
// // ================================================================

using System;
using CatLib;
using UnityEngine;
using IServiceProvider = CatLib.IServiceProvider;

namespace HKLibrary
{
    public class HKUnityRequestServiceProvider:MonoBehaviour,IServiceProvider, IServiceProviderType
    {

        /// <summary>
        /// serice provider
        /// </summary>
        private HKRequestServiceProvider requestServiceProvider;

        /// <summary>
        /// 下载的基础路径
        /// </summary>
        public string DownloadBaseUrl = "";
        
        void Awake()
        {
            requestServiceProvider = new HKRequestServiceProvider();
            HKRequestTools.DownloadBaseUrl = DownloadBaseUrl;
        }
        
        public void Init()
        {
            if (null != requestServiceProvider)
            {
                requestServiceProvider.Init();
            }
        }

        public void Register()
        {
            if (null != requestServiceProvider)
            {
                requestServiceProvider.Register();
            }
        }

        public Type BaseType
        {
            get { return requestServiceProvider.GetType(); }
        }
    }
}