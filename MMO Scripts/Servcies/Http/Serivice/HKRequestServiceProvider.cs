// // ================================================================
// // FileName:HKSpriteManager.cs
// // User: Baron-Fisher
// // CreateTime:2017 0910 12:23
// // Description:Http消息请求的服务器提供
// // Copyright (c) 2016 Greg.Co.Ltd. All rights reserved.
// // ================================================================

using CatLib;

namespace HKLibrary
{
    public class HKRequestServiceProvider : IServiceProvider
    {
        public void Init()
        {
        }

        
        /// <summary>
        /// 绑定服务
        /// </summary>
        public void Register()
        {
            App.Singleton<HKRequestTaskQueue>().Alias<IRequestTaskQueue>();

            App.Singleton<HKHttpRequestComponent>().Alias<IHttpRequest>();
        }
    }
}