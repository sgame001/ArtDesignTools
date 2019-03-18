// // // ================================================================
// // // FileName:HKUnityFileSeriviceProvdier.cs
// // // User: Baron
// // // CreateTime:2017-09-28-15:01
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
    public class HKUnityFileSeriviceProvdier : MonoBehaviour, IServiceProvider, IServiceProviderType
    {
        private HKFileServiceProvider fileServiceProvider;

        private void Awake()
        {
            fileServiceProvider = new HKFileServiceProvider();
        }

        public void Init()
        {
            if (null != fileServiceProvider)
            {
                fileServiceProvider = new HKFileServiceProvider();
            }
        }

        public void Register()
        {
            if (null != fileServiceProvider)
            {
                fileServiceProvider.Register();
            }
        }

        public Type BaseType
        {
            get { return fileServiceProvider.GetType(); }
        }
    }
}