// // // ================================================================
// // // FileName:HKFileServiceProvider.cs
// // // User: Baron
// // // CreateTime:2017-09-28-14:59
// // // Description:
// // // 
// // // Copyright (c) 2016 Heeking.Co.Ltd. All rights reserved.
// // // ================================================================

using CatLib;

namespace HKLibrary
{
    public class HKFileServiceProvider:IServiceProvider
    {
        public void Init()
        {
            
        }

        public void Register()
        {
            App.Singleton<HKFileSystem>().Alias<IFileDiskSystem>();
        }
    }
}