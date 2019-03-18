// // ================================================================
// // FileName:CoroutinesServiceProvider.cs
// // User: Baron
// // CreateTime:3/16/2018
// // Description: 使用携程分布加载的
// // ================================================================

using CatLib;

namespace HKLibrary
{
    public class CoroutinesServiceProvider : IServiceProvider
    {
        public void Init()
        {
            
        }

        public void Register()
        {
            // 绑定一个单例
            App.Singleton<CoroutinesService>().Alias<ICoroutinesService>();
        }
    }
}