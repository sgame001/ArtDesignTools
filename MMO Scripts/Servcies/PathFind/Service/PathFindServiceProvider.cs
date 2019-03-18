// // ================================================================
// // FileName:PathFindServiceProvider.cs
// // User: Baron-Fisher
// // CreateTime:2018 0202 0:46
// // Description:寻路服务
// // Copyright (c) 2016 Greg.Co.Ltd. All rights reserved.
// // ================================================================

using CatLib;

namespace GameCoreLib
{
    public class PathFindServiceProvider : IServiceProvider
    {
        
        public void Init()
        {
            App.Make<IPathFind>().Init();
        }

        public void Register()
        {
            App.Singleton<AStarHelper>().Alias<IPathFind>();
        }
    }
}