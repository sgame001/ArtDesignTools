// // ================================================================
// // FileName:HKSpriteManager.cs
// // User: Baron-Fisher
// // CreateTime:2017 0913 1:10
// // Description:Unity关联的的内存缓存池服务提供者
// // Copyright (c) 2016 Greg.Co.Ltd. All rights reserved.
// // ================================================================

using System;
using CatLib;
using UnityEngine;
using IServiceProvider = CatLib.IServiceProvider;

namespace HKLibrary
{
    public class HKUntiyPoolServiceProvider : MonoBehaviour, IServiceProvider, IServiceProviderType
    {
        /// <summary>
        /// 服务提供者
        /// </summary>
        private HKPoolServiceProvider poolServiceProivder;

        private void Awake()
        {
            poolServiceProivder = new HKPoolServiceProvider();
        }

        public void Init()
        {
            if (null != poolServiceProivder)
            {
                poolServiceProivder.Init();
            }
        }

        public void Register()
        {
            if (null != poolServiceProivder)
            {
                poolServiceProivder.Register();
            }
        }

        public Type BaseType
        {
            get { return poolServiceProivder.GetType(); }
        }
    }
}