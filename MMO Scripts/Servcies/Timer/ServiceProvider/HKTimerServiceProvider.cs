// // // ================================================================
// // // FileName:HKTimerServiceProvider.cs
// // // User: Baron
// // // CreateTime:2017-09-26-11:08
// // // Description:
// // // 
// // // Copyright (c) 2016 Heeking.Co.Ltd. All rights reserved.
// // // ================================================================

using CatLib;

namespace HKLibrary
{
    public class HKTimerServiceProvider : IServiceProvider
    {
        public void Init()
        {
        }

        public void Register()
        {
            App.Singleton<HKGameTimerManager>().Alias<IGameTimerManager>();
        }
    }
}