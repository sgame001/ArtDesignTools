// // // ================================================================
// // // FileName:HKUnityDBServiceProvider.cs
// // // User: Baron
// // // CreateTime:2017-09-08-17:01
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
    public class HKUnityDBServiceProvider:MonoBehaviour, IServiceProvider, IServiceProviderType
    {
        /// <summary>
        /// 数据库服务
        /// </summary>
        private HKDBServiceProvider dbServiceProvider;

        /// <summary>
        /// 数据连接类型
        /// </summary>
        public SqlDBAdapterDataType SqlType;
        
        private void Awake()
        {
            if (null == dbServiceProvider)
            {
                dbServiceProvider = new HKDBServiceProvider();
            }
        }

        public void Init()
        {
            if (null != dbServiceProvider)
            {
                dbServiceProvider.SqlType = SqlType;
                dbServiceProvider.Init();
            }
        }

        /// <summary>
        /// 注册
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        public void Register()
        {
            if (null != dbServiceProvider)
            {
                dbServiceProvider.Register();
            }
        }

        public Type BaseType
        {
            get { return dbServiceProvider.GetType(); }
        }
    }
}