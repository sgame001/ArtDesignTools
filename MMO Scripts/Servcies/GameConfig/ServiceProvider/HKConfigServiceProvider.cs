using CatLib;

namespace HKLibrary.ServiceProvider
{
    public class HKConfigServiceProvider : IServiceProvider
    {
        public void Init()
        {
            
        }

        public void Register()
        {
            App.Singleton<HKConfigManager>().Alias<IConfigManager>();
        }
    }
}