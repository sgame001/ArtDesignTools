// // ================================================================
// // FileName:HKSpriteManager.cs
// // User: Baron-Fisher
// // CreateTime:2018 0202 0:32
// // Description:
// // Copyright (c) 2016 Greg.Co.Ltd. All rights reserved.
// // ================================================================

using System;
using CatLib;
using UnityEngine;
using IServiceProvider = CatLib.IServiceProvider;

namespace HKLibrary
{
    public class SMCUnityServiceProvider : MonoBehaviour, IServiceProvider, IServiceProviderType
    {
        private SMCServiceProvider smcProvider;
        private void Awake()
        {
            smcProvider = new SMCServiceProvider();
        }

        public void Init()
        {
            if (null != smcProvider)
            {
                smcProvider.Init();
            }
        }

        public void Register()
        {
            if (null != smcProvider)
            {
                smcProvider.Register();
            }
        }

        public Type BaseType
        {
            get { return typeof(SMCServiceProvider); }
        }
    }
}