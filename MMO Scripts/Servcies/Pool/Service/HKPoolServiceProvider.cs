// // ================================================================
// // FileName:HKSpriteManager.cs
// // User: Baron-Fisher
// // CreateTime:2017 0913 1:01
// // Description:内存缓存池的服务提供
// // Copyright (c) 2016 Greg.Co.Ltd. All rights reserved.
// // ================================================================

using CatLib;

namespace HKLibrary
{
    public class HKPoolServiceProvider:IServiceProvider
    {
        public void Init()
        {
            
        }

        public void Register()
        {
            App.Singleton<PoolGroupManager>().Alias<IPoolManager>().OnResolving(( data, o) =>
            {
                var poolGroupManager = (PoolGroupManager) o;
                if (null != poolGroupManager)
                {
                    poolGroupManager.Init();
                }
                return poolGroupManager;
            });
        }
    }
}