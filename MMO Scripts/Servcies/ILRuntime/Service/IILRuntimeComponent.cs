// // // ================================================================
// // // FileName:IILRuntimeComponent.cs
// // // User: Baron
// // // CreateTime:2017-09-08-18:12
// // // Description:ILRuntime组件
// // // 
// // // Copyright (c) 2016 Heeking.Co.Ltd. All rights reserved.
// // // ================================================================

using System;
using ILRuntime.CLR.Method;
using ILRuntime.Runtime.Intepreter;

namespace HKLibrary
{
    public interface IILRuntimeComponent
    {
        /// <summary>
        /// ilrt 环境对象
        /// </summary>
        ILRuntime.Runtime.Enviorment.AppDomain ILAppDomain { get; set; }

        /// <summary>
        /// 初始化
        /// </summary>
        void Init();

        /// <summary>
        /// 注册Action以及Delegate
        /// </summary>
        void RegisterAdapter();

        /// <summary>
        /// 执行一个静态函数
        /// </summary>
        /// <param name="_clazzFullName"></param>
        /// <param name="_methodName"></param>
        /// <param name="_params"></param>
        /// <returns></returns>
        object InvokeStatic(string _clazzFullName, string _methodName, params object[] _params);

        /// <summary>
        /// 根据类型全名,返回一个ILRuntime的对象
        /// </summary>
        /// <param name="_clazzName"></param>
        /// <returns></returns>
        ILTypeInstance GetILRTInstance(string _clazzName);

        /// <summary>
        /// 获取方法
        /// </summary>
        /// <returns></returns>
        IMethod GetMethod(string _typeName, string _methodName);
        
        /// <summary>
        /// 根据名字执行一个成员函数
        /// </summary>
        /// <param name="_clazzFullName"></param>
        /// <param name="_methodName"></param>
        /// <param name="_params"></param>
        /// <returns></returns>
        object Invoke(string _clazzFullName, string _methodName, params object[] _params);

        /// <summary>
        /// 根据一个IL对象, 执行一个执行名字的函数
        /// </summary>
        /// <param name="_instance"></param>
        /// <param name="_methodName"></param>
        /// <param name="_params"></param>
        /// <returns></returns>
        object Invoke(ILTypeInstance _instance, string _methodName, params object[] _params);

        /// <summary>
        /// 获取所有的类型
        /// </summary>
        Type[] GetAllILRTTypes();

        #if UNITY_EDITOR

        void CheckGenericType(ILRuntime.CLR.TypeSystem.CLRType clrType, Type[] args);

        #endif
    }
}