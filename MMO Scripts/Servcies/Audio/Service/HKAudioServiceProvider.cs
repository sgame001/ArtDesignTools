// // // ================================================================
// // // FileName:HKAudioServiceProvider.cs
// // // User: Baron
// // // CreateTime:2017-09-08-11:54
// // // Description:声音处理的内容提供者
// // // 
// // // Copyright (c) 2016 Heeking.Co.Ltd. All rights reserved.
// // // ================================================================

using CatLib;

namespace HKLibrary
{
    public class HKAudioServiceProvider:IServiceProvider
    {
        
        /// <summary>
        /// 服务器初始化
        /// </summary>
        public void Init()
        {
            UnityEngine.Debug.Log("Service Provider Init");
        }

        /// <summary>
        /// 注册绑定具体的服务
        /// </summary>
        public void Register()
        {
            UnityEngine.Debug.Log("Service Provider Reigster");
            App.Singleton<HKAudioComponent>().Alias<IAudioComponent>().OnResolving(
                    (data, o) =>
                    {
                        HKAudioComponent audioComponent = (HKAudioComponent) o;
                        if (null != audioComponent)
                        {
                            audioComponent.Init((_name, _bundleName) => App.Make<IResourcesMgr>().LoadAsset(_name).Data);
                        }
                        return audioComponent;
                    });
        }
    }
}