// // ================================================================
// // FileName:FramingUnityServiceProvider.cs
// // User: Baron-Fisher
// // CreateTime:2018 0525 22:20
// // Description:
// // Copyright (c) 2017 Greg.Co.Ltd. All rights reserved.
// // ================================================================

using System;
using CatLib;
using UnityEngine;
using IServiceProvider = CatLib.IServiceProvider;

namespace HKLibrary
{
    public class FramingUnityServiceProvider : MonoBehaviour, IServiceProvider, IServiceProviderType
    {
        private FramingServiceProvider framingServiceProvider;

        private void Awake()
        {
            framingServiceProvider = new FramingServiceProvider();
        }

        public void Init()
        {
            if (null != framingServiceProvider)
            {
                framingServiceProvider.Init();
            }
        }

        public void Register()
        {
            if (null != framingServiceProvider)
            {
                framingServiceProvider.Register();
            }
        }

        public Type BaseType
        {
            get { return framingServiceProvider.GetType(); }
        }
    }
}