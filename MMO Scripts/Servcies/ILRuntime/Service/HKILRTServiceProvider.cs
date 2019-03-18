// // // ================================================================
// // // FileName:HKILRTServiceProvider.cs
// // // User: Baron
// // // CreateTime:2017-09-08-19:02
// // // Description:ILRT的服务器提供商
// // // 
// // // Copyright (c) 2016 Heeking.Co.Ltd. All rights reserved.
// // // ================================================================

using CatLib;

namespace HKLibrary
{
    public class HKILRTServiceProvider : IServiceProvider
    {
        
        /// <summary>
        /// init
        /// </summary>
        public void Init()
        {
        }
        
        /// <summary>
        /// 注册
        /// </summary>
        public void Register()
        {
            App.Singleton<HKILModuleComponent>().Alias<IILRuntimeComponent>().OnResolving((data, o) =>
            {
                HKILModuleComponent ilrtModule = (HKILModuleComponent) o;
                ilrtModule.Init();
                return ilrtModule;
            });
        }
    }
}