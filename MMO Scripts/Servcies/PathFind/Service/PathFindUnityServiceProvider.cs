// // ================================================================
// // FileName:PathFindUnityServiceProvider.cs
// // User: Baron-Fisher
// // CreateTime:2018 0202 0:47
// // Description:
// // Copyright (c) 2016 Greg.Co.Ltd. All rights reserved.
// // ================================================================

using System;
using CatLib;
using UnityEngine;
using IServiceProvider = CatLib.IServiceProvider;

namespace GameCoreLib
{
    public class PathFindUnityServiceProvider : MonoBehaviour, IServiceProvider, IServiceProviderType
    {
        private PathFindServiceProvider serviceProvider = null;
        
        private void Awake()
        {
            serviceProvider = new PathFindServiceProvider();
        }

        public void Init()
        {
            if (null != serviceProvider)
            {
                serviceProvider.Init();
            }
        }

        public void Register()
        {
            if (null != serviceProvider)
            {
                serviceProvider.Register();
            }
        }

        public Type BaseType { get{ return typeof(PathFindServiceProvider);} }
    }
}