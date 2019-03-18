// // // ================================================================
// // // FileName:HKUnityILRTServiceProvider.cs
// // // User: Baron
// // // CreateTime:2017-09-08-19:08
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
    public class HKUnityILRTServiceProvider : MonoBehaviour, IServiceProvider, IServiceProviderType
    {
        /// <summary>
        /// ilruntime service provider
        /// </summary>
        private HKILRTServiceProvider ilrtServiceProvider;

        /// <summary>
        ///  unity启动调用
        /// </summary>
        private void Awake()
        {
            ilrtServiceProvider = new HKILRTServiceProvider();
        }

        /// <summary>
        /// init
        /// </summary>
        public void Init()
        {
#if ILRT
            if (null != ilrtServiceProvider)
            {
                ilrtServiceProvider.Init();
            }
#endif
        }

        /// <summary>
        /// reigster
        /// </summary>
        public void Register()
        {
#if ILRT
            if (null != ilrtServiceProvider)
            {
                ilrtServiceProvider.Register();
            }   
#endif
        }

        /// <summary>
        /// 返回type
        /// </summary>
        public Type BaseType
        {
            get { return ilrtServiceProvider.GetType(); }
        }
    }
}