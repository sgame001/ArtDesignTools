// // ================================================================
// // FileName:ResourcesMgrServiceProvider.cs
// // User: Baron
// // CreateTime:2/1/2018
// // Description: 资源管理器的Service Provdier
// // ================================================================
using CatLib;

namespace HKLibrary
{
    public class ResourcesMgrServiceProvider : IServiceProvider
    {
        public void Init()
        {
            App.Make<ResourcesMgr>().Init();
        }

        public void Register()
        {
            App.Singleton<ResourcesMgr>().Alias<IResourcesMgr>();
        }
    }
}