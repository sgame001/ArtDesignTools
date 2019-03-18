// // // ================================================================
// // // FileName:HKAudioService.cs
// // // User: Baron
// // // CreateTime:2017-09-08-11:57
// // // Description:
// // // 
// // // Copyright (c) 2016 Heeking.Co.Ltd. All rights reserved.
// // // ================================================================

using System;
using CatLib;
using UnityEngine;
using IServiceProvider = CatLib.IServiceProvider;

namespace HKLibrary
{
    public class HKUntiyAudioServiceProvider:MonoBehaviour,IServiceProvider, IServiceProviderType
    {
        private HKAudioServiceProvider audioServiceProvider;

        private void Awake()
        {
            audioServiceProvider = new HKAudioServiceProvider();
        }

        public void Init()
        {
            UnityEngine.Debug.Log("Unity Service Provider Init");
            if (null != audioServiceProvider)
            {
                audioServiceProvider.Init();
            }
        }

        public void Register()
        {
            UnityEngine.Debug.Log("Unity Service Provider Register");
            if (null != audioServiceProvider)
            {
                audioServiceProvider.Register();
            }
        }

        /// <summary>
        /// 获取基础类型
        /// </summary>
        public Type BaseType
        {
            get { return audioServiceProvider.GetType(); }
        }
    }
}