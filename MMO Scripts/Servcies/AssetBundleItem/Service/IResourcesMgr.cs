// // ================================================================
// // FileName:IResourcesMgr.cs
// // User: Baron-Fisher
// // CreateTime:2018 0131 9:42
// // Description:资源管理对外接口，会包含
// // Copyright (c) 2016 Greg.Co.Ltd. All rights reserved.
// // ================================================================

using System;
using System.Collections.Generic;
using FairyGUI;
using UnityEngine;

namespace HKLibrary
{
    public interface IResourcesMgr
    {
        /// <summary>
        /// init
        /// </summary>
        void Init();
        
        /// <summary>
        /// 加载资源
        /// </summary>
        /// <param name="_assetName"></param>
        /// <returns></returns>
        IResResponse LoadAsset(string _assetName);

        /// <summary>
        /// 卸载资源
        /// </summary>
        /// <param name="_assetNmae"></param>
        void UnloadAsset(string _assetNmae);

        /// <summary>
        /// 加载FairyGUIPackage
        /// </summary>
        /// <param name="_assetName"></param>
        /// <returns></returns>
        UIPackage LoadFairyGUIPcakge(string _assetName);

        /// <summary>
        /// 异步加载资源
        /// </summary>
        /// <param name="_assetName"></param>
        /// <param name="_resultCallback"></param>
        void LoadAssetAsync(string _assetName, Action<IResResponse> _resultCallback);

        
        /// <summary>
        /// 异步加载fairy gui资源
        /// </summary>
        /// <param name="_assetName"></param>
        /// <param name="_package"></param>
        void LoadFairyGUIPackageAsync(string _assetName, Action<UIPackage> _package);

        /// <summary>
        /// 移除fairygui资源
        /// </summary>
        /// <param name="_asset"></param>
        void RemoveFairyGUIPckage(string _asset);
        
        /// <summary>
        /// 启动更新资源
        /// </summary>
        /// <param name="_updateProgrsssInfo"></param>
        void StartGameUpdateResources(Action<HKDownloadUpdateInfo> _updateProgrsssInfo);

        /// <summary>
        /// 游戏中更新资源
        /// </summary>
        /// <param name="_updateProgrsssInfo"></param>
        void GamingUpdateResources(Action<HKDownloadUpdateInfo> _updateProgrsssInfo);
        
        
        /// <summary>
        /// 游戏过程中，停止游戏更新
        /// 如果要继续更新，则需要重新启动更新流程
        /// </summary>
        void StopGamingUpdate();

        /// <summary>
        /// 获取热更新数据
        /// </summary>
        /// <param name="_assetName"></param>
        /// <returns></returns>
        byte[] LoadHotfixBytes(string _assetName);

        /// <summary>
        /// 获取数据库位置
        /// </summary>
        /// <returns></returns>
        string GetDatabasePath();

        /// <summary>
        /// 加载当前场景中所有的prefab
        /// </summary>
        /// <param name="_sceneName"></param>
        /// <param name="_resultDictCallback"></param>
        void LoadScenePrefabItems(string _sceneName, Action<Dictionary<string, GameObject>> _resultDictCallback);

        /// <summary>
        /// 释放场景中物品的源数据，直接 unload(true)
        /// </summary>
        /// <param name="_sceneName"></param>
        void ReleaseSceneItems(string _sceneName);

    }
}