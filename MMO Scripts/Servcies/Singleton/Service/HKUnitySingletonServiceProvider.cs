using System;
using CatLib;
using UnityEngine;
using IServiceProvider = CatLib.IServiceProvider;

namespace HKLibrary.Service
{
    public class HKUnitySingletonServiceProvider : MonoBehaviour, IServiceProviderType, IServiceProvider
    {
        private HKSingletonServiceProvider singletonServiceProvider;

        private void Awake()
        {
            singletonServiceProvider = new HKSingletonServiceProvider();
        }

        public Type BaseType
        {
            get { return singletonServiceProvider.GetType(); }
            private set { }
        }

        public void Init()
        {
            if (null != singletonServiceProvider)
            {
                singletonServiceProvider.Init();
            }
        }

        public void Register()
        {
            if (null != singletonServiceProvider)
            {
                singletonServiceProvider.Register();
            }
        }
    }
}