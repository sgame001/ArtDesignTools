// // ================================================================
// // FileName:SceneDynamicServiceUnityProvider.cs
// // User: Baron-Fisher
// // CreateTime:2018 0526 2:32
// // Description:
// // Copyright (c) 2017 Greg.Co.Ltd. All rights reserved.
// // ================================================================

using System;
using CatLib;
using UnityEngine;
using IServiceProvider = CatLib.IServiceProvider;

namespace HKLibrary
{
    public class SceneDynamicServiceUnityProvider :  MonoBehaviour, IServiceProvider, IServiceProviderType
    {
        private SceneDynamicLoadProvider dynamicLoadProvider;
        private void Awake()
        {
            dynamicLoadProvider = new SceneDynamicLoadProvider();
        }

        public void Init()
        {
            if (null != dynamicLoadProvider)
            {
                dynamicLoadProvider.Init();
            }
        }

        public void Register()
        {
            if (null != dynamicLoadProvider)
            {
                dynamicLoadProvider.Register();
            }
        }

        public Type BaseType
        {
            get { return dynamicLoadProvider.GetType(); }
        }
    }
}