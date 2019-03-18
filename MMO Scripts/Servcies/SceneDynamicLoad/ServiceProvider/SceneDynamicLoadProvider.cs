// // ================================================================
// // FileName:SceneDynamicLoadProvider.cs
// // User: Baron-Fisher
// // CreateTime:2018 0526 2:31
// // Description:
// // Copyright (c) 2017 Greg.Co.Ltd. All rights reserved.
// // ================================================================

using CatLib;

namespace HKLibrary
{
    public class SceneDynamicLoadProvider : IServiceProvider 
    {
        public void Init()
        {
            
        }

        public void Register()
        {
            App.Singleton<SceneDynamicLoadMgr>().Alias<ISceneDynamicLoad>();
        }
    }
}