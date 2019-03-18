// // ================================================================
// // FileName:ResourcesMgrUnityServiceProvider.cs
// // User: Baron
// // CreateTime:2/1/2018
// // Description: 和Unity对接Provider
// // ================================================================
using System;
using CatLib;
using UnityEngine;
using IServiceProvider = CatLib.IServiceProvider;

namespace HKLibrary
{
    public class ResourcesMgrUnityServiceProvider : MonoBehaviour, IServiceProvider, IServiceProviderType
    {
        private ResourcesMgrServiceProvider resourceMgrProvider;

        private void Awake()
        {
            resourceMgrProvider = new ResourcesMgrServiceProvider();
        }


        public void Init()
        {
            if (null != resourceMgrProvider)
            {
                resourceMgrProvider.Init();
            }
        }

        public void Register()
        {
            if (null != resourceMgrProvider)
            {
                resourceMgrProvider.Register();
            }
        }

        public Type BaseType
        {
            get { return resourceMgrProvider.GetType(); }
        }
    }
}