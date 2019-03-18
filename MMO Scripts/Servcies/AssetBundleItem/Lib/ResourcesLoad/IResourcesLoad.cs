
using System;
using System.Collections.Generic;
using FairyGUI;
using UnityEngine;

public interface IResourcesLoad
{
    /// <summary>
    /// init
    /// </summary>
    void Init();

    /// <summary>
    /// 映射资源
    /// </summary>
    void MappingRes();
    
    
    /// <summary>
    /// 加载资源信息
    /// </summary>
    /// <param name="_assetName"></param>
    /// <returns></returns>
    IResResponse LoadAsset(string _assetName);

    /// <summary>
    /// 异步加载资源
    /// </summary>
    /// <param name="_assetName"></param>
    /// <param name="_callback"></param>
    void LoadAssetAsync(string _assetName, Action<IResResponse> _callback);


    /// <summary>
    /// 根据场景名字，加载所有的场景静态物件（不包含特效部分）
    /// </summary>
    /// <param name="_sceneName"></param>
    /// <param name="_sourceDictCallback"></param>
    void LoadScenePrefabItems(string _sceneName, Action<Dictionary<string, GameObject>> _sourceDictCallback);

    /// <summary>
    /// 释放场景中的资源对象，直接unload (true) 
    /// </summary>
    /// <param name="_sceneName"></param>
    void ReleaseScenePrefabItems(string _sceneName);
    
    /// <summary>
    /// 卸载一个资源
    /// 会调用unload(true)从内存中彻底删除
    /// </summary>
    /// <param name="_assetName"></param>
    void UnloadAsset(string _assetName);

    /// <summary>
    /// 释放未使用的Unity Asset
    /// </summary>
    void UnloadUnUseAsset();

    /// <summary>
    /// 异步加载场景
    /// </summary>
    /// <param name="_sceneName"></param>
    /// <returns></returns>
    UnityEngine.AsyncOperation LoadSceneAsync(string _sceneName);

    /// <summary>
    /// 加载FairyGUI的Asset Bundle
    /// </summary>
    /// <param name="_bundleName"></param>
    /// <returns></returns>
    UIPackage LoadFairyGUIAssetBundle(string _bundleName);
        
    /// <summary>
    /// 异步加载UI资源
    /// </summary>
    /// <param name="_assetName"></param>
    /// <param name="_callback"></param>
    void LoadFairyGUIAssetAsync(string _assetName, Action<UIPackage> _callback);


    /// <summary>
    /// 卸载一个fairy gui的资源
    /// </summary>
    /// <param name="_assetName"></param>
    void UnloadFairyGUIAsset(string _assetName);
    
    /// <summary>
    /// 获取data base path
    /// </summary>
    /// <returns></returns>
    string GetDatabasePath();

    /// <summary>
    /// 获取热更新数据
    /// </summary>
    /// <returns></returns>
    byte[] LoadHotfixBytes(string _assetName);
}