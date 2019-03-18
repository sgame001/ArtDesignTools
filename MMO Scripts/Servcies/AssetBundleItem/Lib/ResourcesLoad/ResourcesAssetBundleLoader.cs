// // ================================================================
// // FileName:ResourcesAssetBundleLoader.cs
// // User: Baron
// // CreateTime:1/30/2018
// // Description: 通过AssetBundle的形式加载资源
// // ================================================================

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using AdvancedCoroutines;
using CatLib;
using CatLib.API.Timer;
using FairyGUI;
using HKLibrary;
using UnityEngine;
using UnityEngine.SceneManagement;
using Application = UnityEngine.Application;
using AsyncOperation = UnityEngine.AsyncOperation;
using Object = UnityEngine.Object;

public partial class ResourcesAssetBundleLoader : IResourcesLoad
{
    /// <summary>
    /// 资源对象
    /// </summary>
    private readonly Dictionary<string, ResResponse> resResponseCache = new Dictionary<string, ResResponse>();

    /// <summary>
    /// AssetBundle依赖关系
    /// </summary>
    private readonly Dictionary<string, string[]> dependentAssetBundles = new Dictionary<string, string[]>();

    /// <summary>
    /// Asset bundle cache
    /// </summary>
    private readonly Dictionary<string, AssetBundle> assetBundleCache = new Dictionary<string, AssetBundle>();

    /// <summary>
    /// 名字和asset bundle的映射
    /// 通过名字知道应该加载哪个AssetBundle
    /// </summary>
    private readonly Dictionary<string, string> assetToAssetBundlePath = new Dictionary<string, string>();

    /// <summary>
    /// 正在异步加载的asset bundle资源
    /// 如果在异步加载的过程中，又有重新申请资源加载的，会被缓存到一个回调池中
    /// 待资源加载完成，会通知回调池中的回调接口
    /// </summary>
    private readonly List<string> asyncLoadingAssetBundleNames = new List<string>();

    /// <summary>
    /// 等待资源加载完成
    /// </summary>
    private readonly Dictionary<string, List<Action<IResResponse>>> waitAsyncLoadActions = new Dictionary<string, List<Action<IResResponse>>>();

    /// <summary>
    /// 需要释放的临时列表
    /// 防止每次反复new
    /// </summary>
    private List<string> tempUnloadAssetList = new List<string>();


    /// <summary>
    /// init
    /// </summary>
    public void Init()
    {
        AppEvent.OnEvent(AssetBundleDefine.EVENT_UPDATE_SUCCESS_ON_GAMESTART, UpdateResSuccessOnGameStart);

        // 启动一个定时，每30s自动释放一次0引用的Gameobject
//        App.Make<ITimerManager>().Make(AutoCheckUnloadTask).Interval(30f);
    }


    /// <summary>
    /// 更新资源完成
    /// </summary>
    /// <param name="o"></param>
    private void UpdateResSuccessOnGameStart(object o)
    {
        MappingRes();
    }

    /// <summary>
    /// AssetBundle下映射资源
    /// 1.处理资源名字和资源路径的映射
    /// 2.处理ab包和其他ab包的映射
    /// </summary>
    public void MappingRes()
    {
        // 组织AssetBundle依赖关系（manifest.bytes）
        AssetBundleDependent();

        // 映射资源和AssetBundle的关系（version.bytes）
        MappingAssetToAssetBundle(() =>
        {
            // 通知ui层更新完成
            AppEvent.BroadCastEvent(AssetBundleDefine.EVENT_UI_UPDATE_COMPOLETE, null);
        });
    }


    /// <summary>
    /// 同步加载一个Asset
    /// </summary>
    /// <param name="_assetName"></param>
    /// <returns></returns>
    public IResResponse LoadAsset(string _assetName)
    {
        if (true == string.IsNullOrEmpty(_assetName))
        {
            return null;
        }

        if (true == resResponseCache.ContainsKey(_assetName))
        {
            return resResponseCache[_assetName];
        }
        else
        {
            if (true == asyncLoadingAssetBundleNames.Contains(_assetName))
            {
                this.Error("异步资源正在加载，不允许同步调用");
                return null;
            }
        }

        AssetBundle assetBundle = LoadAssetBundle(_assetName);

        if (null != assetBundle)
        {
            var assetObject = assetBundle.LoadAsset(_assetName);
            if (null != assetObject)
            {
                var resResponse = CreateResResponse(_assetName, assetObject);
                return resResponse;
            }
        }

        return null;
    }

    /// <summary>
    /// 异步加载一个资源
    /// </summary>
    /// <param name="_assetName"></param>
    /// <param name="_callback"></param>
    public void LoadAssetAsync(string _assetName, Action<IResResponse> _callback)
    {
        if (false == string.IsNullOrEmpty(_assetName))
        {
            if (true == resResponseCache.ContainsKey(_assetName))
            {
                if (null != _callback)
                {
                    _callback(resResponseCache[_assetName]);
                }
            }
            else
            {
                if (true == asyncLoadingAssetBundleNames.Contains(_assetName))
                {
                    List<Action<IResResponse>> waitList = null;
                    if (false == waitAsyncLoadActions.ContainsKey(_assetName))
                    {
                        waitList = new List<Action<IResResponse>>();
                        waitAsyncLoadActions.Add(_assetName, waitList);
                    }

                    if (null != waitList)
                    {
                        waitList.Add(_callback);
                    }
                    return;
                }
                
                // 调用携程加载资源
                CoroutineManager.StartStandaloneCoroutine(LoadAssetBundleAsync(_assetName, bundle =>
                {
                    if (null != bundle)
                    {
                        var assetObject = bundle.LoadAsset(_assetName);
                        if (null != assetObject)
                        {
                            var resResponse = CreateResResponse(_assetName, assetObject);
                            if (null != _callback)
                            {
                                _callback(resResponse);
                            }

                            // 处理正在加载中的情况
                            if (true == waitAsyncLoadActions.ContainsKey(_assetName))
                            {
                                var waitList = waitAsyncLoadActions[_assetName];
                                foreach (var action in waitList)
                                {
                                    if (null != action)
                                    {
                                        action(resResponse);
                                    }
                                }
                                waitAsyncLoadActions.Remove(_assetName);
                            }
                        }
                    }
                }));
            }
        }
    }

    /// <summary>
    /// 加载场景对应的prefab
    /// </summary>
    /// <param name="_sceneName"></param>
    /// <param name="_sourceDictCallback"></param>
    public void LoadScenePrefabItems(string _sceneName, Action<Dictionary<string, GameObject>> _sourceDictCallback)
    {
    }

    
    /// <summary>
    /// 释放场景信息资源
    /// </summary>
    /// <param name="_sceneName"></param>
    public void ReleaseScenePrefabItems(string _sceneName)
    {
        
    }

    /// <summary>
    /// 创建一个ResResponse
    /// </summary>
    /// <param name="_assetName"></param>
    /// <param name="_assetObject"></param>
    /// <returns></returns>
    private ResResponse CreateResResponse(string _assetName, Object _assetObject)
    {
        if (null != _assetObject)
        {
#if UNITY_EDITOR
            if (true == _assetObject is GameObject)
            {
                RefershMaterial((GameObject) _assetObject); // 刷新材质球
            }
#endif
            var resResponse = new ResResponse(_assetObject)
            {
                SourceDataName = _assetName
            };
            if (false == resResponseCache.ContainsKey(_assetName))
            {
                resResponseCache.Add(_assetName, resResponse);
            }

            return resResponse;
        }
        return null;
    }
    
    

    /// <summary>
    /// 卸载资源
    /// </summary>
    /// <param name="_assetName"></param>
    public void UnloadAsset(string _assetName)
    {
        if (true == string.IsNullOrEmpty(_assetName))
        {
            return;
        }

        // 卸载resResponseCache中的
        if (true == resResponseCache.ContainsKey(_assetName))
        {
            var resResponse = resResponseCache[_assetName];
            if (true == resResponse.IsGamobject)
            {
                if (resResponse.ReferencesCount > 0)
                {
                    this.Error("当前有未被正确释放的对象, 强制卸载后可能会引起显示异常 = " + _assetName);
                }
            }

            resResponse.UnLoad();
            resResponseCache.Remove(_assetName);
        }

        // 卸载Asset Bundle
        if (true == assetBundleCache.ContainsKey(_assetName))
        {
            var assetBundle = assetBundleCache[_assetName];
            if (null != assetBundle)
            {
                this.Debug("释放资源 = " + _assetName);
                assetBundle.Unload(true);
            }

            assetBundleCache.Remove(_assetName);
        }
    }

    /// <summary>
    /// 释放没有的资源
    /// </summary>
    public void UnloadUnUseAsset()
    {
        Resources.UnloadUnusedAssets();
    }

    /// <summary>
    /// 异步加载场景
    /// </summary>
    /// <param name="_sceneName"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public AsyncOperation LoadSceneAsync(string _sceneName)
    {
        return SceneManager.LoadSceneAsync(_sceneName);
    }


    /// <summary>
    /// 加载FairyGUI的AssetBundle
    /// 加载完之后，交给FairyGUI托管
    /// </summary>
    /// <param name="_bundleName"></param>
    /// <returns></returns>
    public UIPackage LoadFairyGUIAssetBundle(string _bundleName)
    {
        if (true == string.IsNullOrEmpty(_bundleName))
        {
            return null;
        }

        AssetBundle assetBundle = LoadAssetBundle(_bundleName);
        return UIPackage.AddPackage(assetBundle, assetBundle);
    }


    /// <summary>
    /// 异步加载fairy gui 资源
    /// </summary>
    /// <param name="_assetName"></param>
    /// <param name="_callback"></param>
    /// <exception cref="NotImplementedException"></exception>
    public void LoadFairyGUIAssetAsync(string _assetName, Action<UIPackage> _callback)
    {
        if (true == string.IsNullOrEmpty(_assetName))
        {
            if (null != _callback)
            {
                _callback(null);
            }
        }
        else
        {
            // 调用携程加载资源
            CoroutineManager.StartStandaloneCoroutine(LoadAssetBundleAsync(_assetName, bundle =>
            {
                if (null != bundle)
                {
                    var package = UIPackage.AddPackage(bundle, bundle);
                    if (null != _callback)
                    {
                        _callback(package);
                    }
                }
            }));
        }
    }

    
    /// <summary>
    /// 卸载一个FairyGUI资源
    /// </summary>
    /// <param name="_assetName"></param>
    public void UnloadFairyGUIAsset(string _assetName)
    {
        if (true == string.IsNullOrEmpty(_assetName))
        {
            return;
        }

        // 卸载fairy gui package
        UIPackage.RemovePackage(_assetName);
        
        // 卸载asset bundle
        UnloadAsset(_assetName);
    }

    /// <summary>
    /// 获取数据库路径
    /// </summary>
    /// <returns></returns>
    public string GetDatabasePath()
    {
        return Application.persistentDataPath + "database.dat";
    }


    /// <summary>
    /// 热更新数据
    /// </summary>
    /// <param name="_assetName"></param>
    /// <returns></returns>
    public byte[] LoadHotfixBytes(string _assetName)
    {
//        var path = _assetName + ".dat";
//        if (true == FileSystemFacade.Instance.FileExists(path))
//        {
//            return FileSystemFacade.Instance.ReadFile(_assetName + ".dat"); // 直接通过文件系统读取
//        }
        var assetResponse = LoadAsset(_assetName);
        if (null != assetResponse && null != assetResponse.Data)
        {
            if (true == assetResponse.Data is TextAsset)
            {
                return ((TextAsset) assetResponse.Data).bytes;
            }
        }

        return null;
    }


    /// <summary>
    /// 根据bundle name名字，加载AssetBundle名称
    /// </summary>
    /// <param name="_assetName"></param>
    /// <param name="_isCache">是否加入到缓存cache中,特殊情况也可能不加入缓存(FairyGUI是需要独立托管AB的所以不需要Cache处理)</param>
    /// <returns></returns>
    private AssetBundle LoadAssetBundle(string _assetName, bool _isCache = true)
    {
        AssetBundle resultAssetBundle = null;
        if (true == assetBundleCache.ContainsKey(_assetName))
        {
            resultAssetBundle = assetBundleCache[_assetName];
            return resultAssetBundle;
        }

        var bundlePath = GetAssetBundlePath(_assetName);
        if (true == string.IsNullOrEmpty(bundlePath))
        {
            return null;
        }

        // 遍历加载AssetBundle部分
        string[] dps = null;
        if (true == dependentAssetBundles.TryGetValue(_assetName, out dps))
        {
            for (int index = 0; index < dps.Length; index++)
            {
                var dpAssetBundleName = dps[index];
                if (false == string.IsNullOrEmpty(dpAssetBundleName) &&
                    false == assetBundleCache.ContainsKey(dpAssetBundleName))
                {
                    LoadAssetBundle(dpAssetBundleName);
                }
            }
        }

        // 加载具体的AssetBundle
        resultAssetBundle = AssetBundle.LoadFromFile(bundlePath); //  同步加载
        if (null != resultAssetBundle)
        {
            if (true == _isCache && false == assetBundleCache.ContainsKey(_assetName))
            {
                assetBundleCache.Add(_assetName, resultAssetBundle);
            }
        }

        return resultAssetBundle;
    }


    /// <summary>
    /// 异步加载AssetBundle
    /// </summary>
    /// <param name="_assetName"></param>
    /// <param name="_assetBunleCallback">回调</param>
    /// <param name="_isCache"></param>
    /// <returns></returns>
    private IEnumerator LoadAssetBundleAsync(string _assetName, Action<AssetBundle> _assetBunleCallback, bool _isCache = true)
    {
        AssetBundle resultAssetBundle = null;
        if (true == assetBundleCache.ContainsKey(_assetName))
        {
            resultAssetBundle = assetBundleCache[_assetName];
            if (null != _assetBunleCallback)
            {
                _assetBunleCallback(resultAssetBundle);
            }
        }
        else
        {
            List<string> loadAssetNames = new List<string>();
            StatisticsAsyncLoadAssetBundles(_assetName, loadAssetNames); // 统计所有要加载的asset bundle
            if (loadAssetNames.Count > 0)
            {
                asyncLoadingAssetBundleNames.AddRange(loadAssetNames);
            }

            for (int index = 0; index < loadAssetNames.Count; index++)
            {
                var loadAssetBundleName = loadAssetNames[index];
                var path                = GetAssetBundlePath(loadAssetBundleName);
                if (true == string.IsNullOrEmpty(path) || true == assetBundleCache.ContainsKey(loadAssetBundleName))
                {
                    continue;
                }

                var myLoadedAssetBundle = AssetBundle.LoadFromFileAsync(path);
                yield return myLoadedAssetBundle;
                if (null != myLoadedAssetBundle.assetBundle)
                {
                    assetBundleCache.Add(_assetName, myLoadedAssetBundle.assetBundle);
                    if (loadAssetBundleName == _assetName)
                    {
                        resultAssetBundle = myLoadedAssetBundle.assetBundle;
                    }

                    if (asyncLoadingAssetBundleNames.Contains(_assetName))
                    {
                        asyncLoadingAssetBundleNames.Remove(_assetName); // 从加载队列中移除
                    }
                }
            }

            if (null != _assetBunleCallback)
            {
                _assetBunleCallback(resultAssetBundle);
            }
        }
    }


    /// <summary>
    /// 统计要加载的AssetBundle Names
    /// </summary>
    /// <param name="_assetName"></param>
    /// <param name="_result"></param>
    private void StatisticsAsyncLoadAssetBundles(string _assetName, List<string> _result)
    {
        string[] dps = null;
        if (true == dependentAssetBundles.TryGetValue(_assetName, out dps))
        {
            for (int index = 0; index < dps.Length; index++)
            {
                var dpAssetBundleName = dps[index];
                if (false == string.IsNullOrEmpty(dpAssetBundleName))
                {
                    StatisticsAsyncLoadAssetBundles(dpAssetBundleName, _result);
                    _result.Add(dpAssetBundleName);
                }
            }
        }

        _result.Add(_assetName); // 加入资源自身
    }


    /// <summary>
    /// asset bundle 依赖关系映射
    /// </summary>
    private void AssetBundleDependent()
    {
        // 读取manifest文件
        string manifestFilePath = string.Format("{0}/{1}/{2}/{3}", App.Make<IFileDiskSystem>().GetDiskPath(),
            AssetBundleDefine.AssetBundlesDir, AssetBundleDefine.RuntimePlatformName(), AssetBundleDefine.MANIFEST_FILE);
        var manifestAssetBundle = AssetBundle.LoadFromFile(manifestFilePath);
        if (null != manifestAssetBundle)
        {
            var manifest = manifestAssetBundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
            if (null != manifest)
            {
                dependentAssetBundles.Clear();                         // 先清空列表
                string[] assetBundles = manifest.GetAllAssetBundles(); // 只有存在依赖关系的，才会再这个数组中，Scene不会
                if (null == assetBundles || assetBundles.Length <= 0)
                {
                    return;
                }

                foreach (string bundle in assetBundles)
                {
                    if (null == bundle)
                    {
                        continue;
                    }

                    string[] dependencies = manifest.GetAllDependencies(bundle);

                    if (true == dependentAssetBundles.ContainsKey(bundle))
                    {
                        dependentAssetBundles.Remove(bundle);
                    }

                    if (dependencies.Length > 0)
                    {
                        dependentAssetBundles.Add(bundle, dependencies); // 如果依赖关系 大于0 ,则添加到依赖关系队列中
                    }
                }
            }

            manifestAssetBundle.Unload(true);
        }
    }


    /// <summary>
    /// 映射asset和asset bundle之间的路径关系
    /// 1. 先映射只读文件夹里的
    /// 2. 再映射读写文件夹里的
    /// 因为只读文件需要使用www读取，所以理论上是异步读取，要通过回调函数返回完成
    /// </summary>
    /// <param name="_complete"></param>
    private void MappingAssetToAssetBundle(Action _complete)
    {
        // 先映射StreamingAssets下的
        var readOnlyVersionFileDir = AssetBundleDefine.GetReadOnlyPathByWWW();
        WWWLoadBytes.Instance.LoadBytes(
            string.Format("{0}/{1}", readOnlyVersionFileDir, AssetBundleDefine.VERSION_LOCAL_FILE),
            (s, bytes, _errorMessage) =>
            {
                if (false == string.IsNullOrEmpty(_errorMessage))
                {
                    this.Error("无法读取StreamingAsset中的文件配置 url : " + s + " error message : " + _errorMessage);
                }

                MappingAssetToAssetBundle(GameFramework.Utility.Zip.Decompress(bytes),
                    string.Format("{0}/{1}/{2}", UnityEngine.Application.streamingAssetsPath,
                        AssetBundleDefine.READ_ONLY_DIR, AssetBundleDefine.RuntimePlatformName()));

                // 再映射可读写目录的
                var versionFileBytes = FileSystemFacade.Instance.ReadFile(AssetBundleDefine.GetReadWritePath());

                MappingAssetToAssetBundle(versionFileBytes /**已经是解压过的*/,
                    string.Format("{0}/{1}/{2}", UnityEngine.Application.persistentDataPath,
                        AssetBundleDefine.AssetBundlesDir, AssetBundleDefine.RuntimePlatformName()));

                if (null != _complete)
                {
                    _complete();
                }
            });
    }


    /// <summary>
    /// 映射资源到AssetBundle
    /// </summary>
    private void MappingAssetToAssetBundle(byte[] _versionFileBinary, string _rootDir)
    {
        if (null == _versionFileBinary)
        {
            return;
        }

        if (true == string.IsNullOrEmpty(_rootDir))
        {
            return;
        }

        var versionFileBytes = _versionFileBinary;

        string versionContent = Encoding.UTF8.GetString(versionFileBytes);
        if (true == string.IsNullOrEmpty(versionContent))
        {
            this.Error("version.bytes bytes => string 失败");
            return;
        }

        // 转换出来了version对象
        AssetBundleVersion version = JsonUtility.FromJson<AssetBundleVersion>(versionContent);
        if (null == version)
        {
            this.Error("version文件反序列化失败");
            return;
        }

        for (int index = 0; index < version.AssetBundleVersionItems.Count; index++)
        {
            var downloadItem = version.AssetBundleVersionItems[index];
            if (null == downloadItem)
            {
                continue;
            }

            string fullPath = string.Format("{0}/{1}${2}.dat", _rootDir, downloadItem.ItemName, downloadItem.MD5); // 完整路径
            for (int assetIndex = 0; assetIndex < downloadItem.AssetsList.Count; assetIndex++)
            {
                var    assetItem = downloadItem.AssetsList[assetIndex];
                string fileName  = System.IO.Path.GetFileNameWithoutExtension(assetItem);
                string extension = System.IO.Path.GetExtension(assetItem);

                if (false == string.IsNullOrEmpty(fileName) && false == string.IsNullOrEmpty(extension))
                {
                    if (true == AssetBundleDefine.MappingAssetTypes.Contains(extension))
                    {
                        if (false == assetToAssetBundlePath.ContainsKey(fileName))
                        {
                            assetToAssetBundlePath.Add(fileName, fullPath);
                        }
                        else
                        {
                            assetToAssetBundlePath[fileName] = fullPath;
                        }
                    }
                }
            }
        }

        this.Info("资源映射本地文件 总计 = " + assetToAssetBundlePath.Count);
    }

    /// <summary>
    /// 自动释放的任务
    /// </summary>
    private void AutoCheckUnloadTask()
    {
        tempUnloadAssetList.Clear();
        foreach (var resResponse in resResponseCache)
        {
            if (null == resResponse.Value)
            {
                continue;
            }

            if (resResponse.Value.ReferencesCount == 0)
            {
                tempUnloadAssetList.Add(resResponse.Value.SourceDataName);
            }
        }

        // 统计完统一释放，防止for-remove的问题
        if (tempUnloadAssetList.Count > 0)
        {
            for (int index = tempUnloadAssetList.Count - 1; index >= 0; index--)
            {
                string assetName = tempUnloadAssetList[index];
                UnloadAsset(assetName); // 自动释放
            }
        }
    }


    /// <summary>
    /// 根据文件名获取AssetBundle路径
    /// </summary>
    /// <param name="_fileName"></param>
    /// <returns></returns>
    private string GetAssetBundlePath(string _fileName)
    {
        if (true == assetToAssetBundlePath.ContainsKey(_fileName))
        {
            return assetToAssetBundlePath[_fileName];
        }

        return null;
    }

    /// <summary>
    /// 刷新材质球
    /// </summary>
    /// <param name="_go"></param>
    private void RefershMaterial(GameObject _go)
    {
        if (null == _go)
        {
            return;
        }

#if UNITY_EDITOR
        var renderers = _go.GetComponentsInChildren<Renderer>();
        if (null != renderers && renderers.Length > 0 )
        {
            for (var index = 0; index < renderers.Length; index++)
            {
                var renderer = renderers[index];
                if (null != renderer)
                {
                    foreach (Material material in renderer.sharedMaterials)
                    {
                        if (null != material)
                        {
                            material.shader = Shader.Find(material.shader.name);
                        }
                    }
                }
            }
        }
#endif
        
    }
    
}