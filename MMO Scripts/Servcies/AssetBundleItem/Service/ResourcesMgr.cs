// // ================================================================
// // FileName:ResourcesMgr.cs
// // User: Baron-Fisher
// // CreateTime:2018 0131 9:55
// // Description: 资源管理类，包含游戏的更新，加载，释放
// // Copyright (c) 2016 Greg.Co.Ltd. All rights reserved.
// // ================================================================

using System;
using System.Collections.Generic;
using FairyGUI;
using GameCoreLib.AssetBundleItem.ResourcesLoad;
using UnityEngine;

namespace HKLibrary
{
    public class ResourcesMgr : IResourcesMgr
    {
        /// <summary>
        /// 资源更新
        /// </summary>
        private AssetBundleResUpdate assetBundleResUpdate;

        /// <summary>
        /// 资源加载
        /// </summary>
        private IResourcesLoad resourcesLoad;


        /// <summary>
        /// 显示的初始化
        /// </summary>
        public void Init()
        {
            // 资源加载
            if (LoggerQ.GetResourceLoadType() == ResourceLoadType.AssetBundle)
            {
                // 资源更新模块
                assetBundleResUpdate = new AssetBundleResUpdate();
                assetBundleResUpdate.Init();
                
                // 资源加载模块
                resourcesLoad = new ResourcesAssetBundleLoader();
            }
            else 
            {
                resourcesLoad = new ResourcesEditorLoader(); // 默认使用Editor            
            }
            resourcesLoad.Init();
#if UNITY_EDITOR
            if (true == resourcesLoad is ResourcesAssetBundleLoader)
            {
                this.Error("目前在EDITOR环境下使用AssetBundle进行测试，资源全部来自于资源服务器！！！");
            }
#endif
        }

        /// <summary>
        /// 加载资源
        /// </summary>
        /// <param name="_assetName"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public IResResponse LoadAsset(string _assetName)
        {
            return resourcesLoad.LoadAsset(_assetName);
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        /// <param name="_assetNmae"></param>
        public void UnloadAsset(string _assetNmae)
        {
            resourcesLoad.UnloadAsset(_assetNmae);
        }


        /// <summary>
        /// 加载FairyGUI的Package
        /// </summary>
        /// <param name="_assetName"></param>
        /// <returns></returns>
        public UIPackage LoadFairyGUIPcakge(string _assetName)
        {
            return resourcesLoad.LoadFairyGUIAssetBundle(_assetName);
        }

        /// <summary>
        /// 异步加载一个资源
        /// </summary>
        /// <param name="_assetName"></param>
        /// <param name="_resultCallback"></param>
        public void LoadAssetAsync(string _assetName, Action<IResResponse> _resultCallback)
        {
            resourcesLoad.LoadAssetAsync(_assetName, _resultCallback);
        }

        /// <summary>
        /// 异步加载一个fairy gui 的 package
        /// </summary>
        /// <param name="_assetName"></param>
        /// <param name="_package"></param>
        public void LoadFairyGUIPackageAsync(string _assetName, Action<UIPackage> _package)
        {
            resourcesLoad.LoadFairyGUIAssetAsync(_assetName, _package);
        }

        /// <summary>
        /// 移除fairy gui资源
        /// </summary>
        /// <param name="_asset"></param>
        public void RemoveFairyGUIPckage(string _asset)
        {
            resourcesLoad.UnloadFairyGUIAsset(_asset);
        }

        /// <summary>
        /// 启动更新资源
        /// </summary>
        /// <param name="_updateProgrsssInfo"></param>
        public void StartGameUpdateResources(Action<HKDownloadUpdateInfo> _updateProgrsssInfo)
        {
            if (null != _updateProgrsssInfo)
            {
                assetBundleResUpdate.DownloadProgressCallback = _updateProgrsssInfo;
                assetBundleResUpdate.UpdateGameResources(); // 包含版本对比，以及游戏起始资源下载
            }
        }

        /// <summary>
        /// 游戏中更新资源
        /// </summary>
        /// <param name="_updateProgrsssInfo"></param>
        public void GamingUpdateResources(Action<HKDownloadUpdateInfo> _updateProgrsssInfo)
        {
            if (null != _updateProgrsssInfo)
            {
                assetBundleResUpdate.DownloadProgressCallback = _updateProgrsssInfo;
                assetBundleResUpdate.GameingDownload();
            }
        }


        /// <summary>
        /// 游戏中暂停更新资源
        /// </summary>
        public void StopGamingUpdate()
        {
            assetBundleResUpdate.StopDownload(AssetPathType.gameLoad);
            assetBundleResUpdate.DownloadProgressCallback = null; // 将进度回调置null, 防止重复回调
        }

        /// <summary>
        /// 获取热更新数据
        /// </summary>
        /// <param name="_assetName"></param>
        /// <returns></returns>
        public byte[] LoadHotfixBytes(string _assetName)
        {
            return resourcesLoad.LoadHotfixBytes(_assetName);
        }

        /// <summary>
        /// 获取data base
        /// </summary>
        /// <returns></returns>
        public string GetDatabasePath()
        {
            return resourcesLoad.GetDatabasePath();
        }

        /// <summary>
        /// 加载当前场景中所有的prefab
        /// </summary>
        /// <param name="_sceneName"></param>
        /// <param name="_resultDictCallback"></param>
        public void LoadScenePrefabItems(string _sceneName, Action<Dictionary<string, GameObject>> _resultDictCallback)
        {
            if (true == string.IsNullOrEmpty(_sceneName))
            {
                return;
            }   
            resourcesLoad.LoadScenePrefabItems(_sceneName, _resultDictCallback);
        }

        /// <summary>
        /// 释放场景资源
        /// </summary>
        /// <param name="_sceneName"></param>
        public void ReleaseSceneItems(string _sceneName)
        {
            if (true == string.IsNullOrEmpty(_sceneName))
            {
                resourcesLoad.ReleaseScenePrefabItems(_sceneName);
            }
        }
    }
}