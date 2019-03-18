using CatLib;

namespace HKLibrary.Service
{
    public class HKSingletonServiceProvider : IServiceProvider
    {
        public void Init()
        {
        }

        public void Register()
        {
            App.Singleton<AppSingleton>().Alias<IHKSingletonManager>();
        }
    }
}