using System;
using CatLib;
using UnityEngine;
using IServiceProvider = CatLib.IServiceProvider;

namespace HKLibrary
{
    public class CoroutinesUnityServiceProvider : MonoBehaviour, IServiceProvider, IServiceProviderType
    {
        private CoroutinesServiceProvider coroutinesService = null;

        private void Awake()
        {
            coroutinesService = new CoroutinesServiceProvider();
        }

        public void Init()
        {
            if (null != coroutinesService)
            {
                coroutinesService.Init();
            }
        }

        public void Register()
        {
            if (null != coroutinesService)
            {
                coroutinesService.Register();
            }            
        }

        public Type BaseType
        {
            get { return typeof(CoroutinesServiceProvider); }
        }
    }
}