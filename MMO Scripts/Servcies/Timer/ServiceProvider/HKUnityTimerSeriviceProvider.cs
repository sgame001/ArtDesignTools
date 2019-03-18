// // // ================================================================
// // // FileName:HKUnityTimerSeriviceProvider.cs
// // // User: Baron
// // // CreateTime:2017-09-26-11:09
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
    public class HKUnityTimerSeriviceProvider : MonoBehaviour, IServiceProvider, IServiceProviderType
    {
        private HKTimerServiceProvider timerServiceProvider;

        private void Awake()
        {
            timerServiceProvider = new HKTimerServiceProvider();
        }

        public void Init()
        {
            if (null != timerServiceProvider)
            {
                timerServiceProvider.Init();
            }
        }

        public void Register()
        {
            if (null != timerServiceProvider)
            {
                timerServiceProvider.Register();
            }
        }

        public Type BaseType
        {
            get { return timerServiceProvider.GetType(); }
        }
    }
}