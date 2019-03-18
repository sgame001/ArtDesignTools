// // ================================================================
// // FileName:FramingServiceProvider.cs
// // User: Baron-Fisher
// // CreateTime:2018 0525 22:19
// // Description: 分帧Provider
// // Copyright (c) 2017 Greg.Co.Ltd. All rights reserved.
// // ================================================================

using CatLib;

namespace HKLibrary
{
    public class FramingServiceProvider : IServiceProvider
    {
        public void Init()
        {
            App.Make<IFraming>().Init();
        }

        public void Register()
        {
            App.Singleton<Framing>().Alias<IFraming>();
        }
    }
}