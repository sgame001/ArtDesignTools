using System;
using CatLib;
using UnityEngine;
using IServiceProvider = CatLib.IServiceProvider;

namespace HKLibrary.ServiceProvider
{
    public class HKUnityConfigServiceProvider : MonoBehaviour, IServiceProviderType, IServiceProvider
    {
        private HKConfigServiceProvider serviceProvider;

        public Type BaseType
        {
            get
            {
                return serviceProvider.GetType();
            }
        }

        private void Awake()
        {
            serviceProvider = new HKConfigServiceProvider();            
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
    }
}