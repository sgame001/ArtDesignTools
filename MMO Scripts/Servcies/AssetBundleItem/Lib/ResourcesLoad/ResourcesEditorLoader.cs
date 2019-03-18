// // ================================================================
// // FileName:ResourcesDatabaseLoader.cs
// // User: Baron
// // CreateTime:1/30/2018
// // Description: 处理Dev时通过AssetDataBase加载资源
// // ================================================================

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using FairyGUI;
using HKLibrary;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace GameCoreLib.AssetBundleItem.ResourcesLoad
{
    public class ResourcesEditorLoader : IResourcesLoad
    {
        /// <summary>
        /// 映射的资源路径
        /// </summary>
        private readonly Dictionary<string, string> assetPathMapping = new Dictionary<string, string>();


        /// <summary>
        /// 资源映射缓存路径
        /// </summary>
        private readonly Dictionary<string, IResResponse> resResponseDic = new Dictionary<string, IResResponse>();


        /// <summary>
        /// init
        /// </summary>
        public void Init()
        {
            MappingRes();

            // 发送消息，模拟更新完成
            AppEvent.BroadCastEvent(AssetBundleDefine.EVENT_UI_UPDATE_COMPOLETE);
        }

        /// <summary>
        /// Editor下的资源映射主要处理
        /// 1.资源名字和路径的映射
        /// </summary>
        public void MappingRes()
        {
            TextAsset textAsset = Resources.Load("Config/ResourcesMap") as TextAsset;
            if (null == textAsset)
            {
                this.Error("无法加载配置文件");
                return;
            }

            string content = textAsset.text;
            if (true == string.IsNullOrEmpty(content))
            {
                return;
            }

            var maps = JsonUtility.FromJson<HKSerialization<HKResourcesMapField>>(content).ToList();

            for (int index = 0; index < maps.Count; index++)
            {
                HKResourcesMapField mapField = maps[index];
                if (null == mapField)
                {
                    continue;
                }

                //editor 状态会多次执行。避免 samekey
                if (assetPathMapping.ContainsKey(mapField.fileName) == false)
                    assetPathMapping.Add(mapField.fileName, mapField.filePath);
            }

            this.Info("资源映射文件加载成功 总计 = " + assetPathMapping.Count);
        }


        /// <summary>
        /// 同步加载资源
        /// </summary>
        /// <param name="_assetName"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public IResResponse LoadAsset(string _assetName)
        {
            string path = GetAssetPathByName(_assetName);
            if (true == string.IsNullOrEmpty(path))
            {
                return null;
            }

            if (true == resResponseDic.ContainsKey(_assetName))
            {
                return resResponseDic[_assetName];
            }

            UnityEngine.Object obj = null;
#if UNITY_EDITOR
            obj = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(path);
#endif
            if (null != obj)
            {
                IResResponse resResponse = new ResResponse(obj)
                {
                    SourceDataName = _assetName
                };
                resResponseDic.Add(_assetName, resResponse);
                return resResponse;
            }

            return null;
        }


        /// <summary>
        /// 异步加载
        /// 在ediotr下表现是同步的
        /// </summary>
        /// <param name="_assetName"></param>
        /// <param name="_callback"></param>
        public void LoadAssetAsync(string _assetName, Action<IResResponse> _callback)
        {
            var iResponse = LoadAsset(_assetName);
            if (null != _callback)
            {
                _callback(iResponse);
            }
        }

        /// <summary>
        /// 根据场景名字加载所有的场景静态物件
        /// </summary>
        /// <param name="_sceneName"></param>
        /// <param name="_sourceDictCallback"></param>
        public void LoadScenePrefabItems(string _sceneName, Action<Dictionary<string, GameObject>> _sourceDictCallback)
        {
            if (true == string.IsNullOrEmpty(_sceneName))
            {
                return;
            }

            Dictionary<string, GameObject> dictResult = new Dictionary<string, GameObject>();
#if UNITY_EDITOR
            string        loadUrl = string.Format(Application.dataPath + "/ClientResources/ScenePrefabs/{0}", _sceneName);
            DirectoryInfo dirInfo = new DirectoryInfo(loadUrl);
            FileInfo[]    files   = dirInfo.GetFiles("*.prefab");
            List<FileInfo> fileInfoList = new List<FileInfo>();
            foreach (var fileInfo in files)
            {
                fileInfoList.Add(fileInfo);
            }
            
            FramingFacade.Instance.FramingExecuteByFrameCountArgs(fileInfoList, 15/**15帧内完成*/, (_index, _info) =>
            {
                var    fileName = Path.GetFileNameWithoutExtension(_info.Name);
                string path     = GetAssetPathByName(fileName);
                var    go       = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                if (null != go)
                {
                    dictResult.Add(fileName, go);
                }
            }, () =>
            {
                if (null != _sourceDictCallback)
                {
                    // 完成后执行回调
                    _sourceDictCallback(dictResult);
                }
            });
#endif
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="_sceneName"></param>
        public void ReleaseScenePrefabItems(string _sceneName)
        {
#if UNITY_EDITOR
            Resources.UnloadUnusedAssets();
#endif
        }


        /// <summary>
        /// 释放资源
        /// </summary>
        /// <param name="_assetName"></param>
        public void UnloadAsset(string _assetName)
        {
            if (true == resResponseDic.ContainsKey(_assetName))
            {
                IResResponse resResponse = resResponseDic[_assetName];
                //if (null != asset)
                //{
                //    Resources.UnloadAsset(asset);
                //}
                resResponseDic.Remove(_assetName);
                resResponse.UnLoad();
            }
        }

        /// <summary>
        /// 释放已经不使用的资源
        /// </summary>
        public void UnloadUnUseAsset()
        {
            Resources.UnloadUnusedAssets();
        }


        /// <summary>
        /// 加载场景
        /// </summary>
        /// <param name="_sceneName"></param>
        /// <returns></returns>
        public AsyncOperation LoadSceneAsync(string _sceneName)
        {
            if (true == string.IsNullOrEmpty(_sceneName))
            {
                return null;
            }

            return SceneManager.LoadSceneAsync(_sceneName);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="_bundleName"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public UIPackage LoadFairyGUIAssetBundle(string _bundleName)
        {
            if (true == string.IsNullOrEmpty(_bundleName))
            {
                return null;
            }

            var bundlePath = GetAssetPathByName(_bundleName);
            if (false == string.IsNullOrEmpty(bundlePath))
            {
                bundlePath = bundlePath.Replace(".bytes", "");
            }

            var package = UIPackage.AddPackage(bundlePath, (_name, _extension, _type) =>
            {
                UnityEngine.Object assetObj = null;
#if UNITY_EDITOR
                assetObj = AssetDatabase.LoadAssetAtPath(_name + _extension, _type);
#endif
                return assetObj;
            });

            return package;
        }


        /// <summary>
        /// 异步加载FairyGUI
        /// </summary>
        /// <param name="_assetName"></param>
        /// <param name="_callback"></param>
        /// <exception cref="NotImplementedException"></exception>
        public void LoadFairyGUIAssetAsync(string _assetName, Action<UIPackage> _callback)
        {
            UIPackage package = LoadFairyGUIAssetBundle(_assetName);
            if (null == package)
            {
                this.Error("ui 资源加载失败 = " + _assetName);
            }

            if (null != _callback)
            {
                _callback(package);
            }
        }

        /// <summary>
        /// 卸载一个fairy gui 资源
        /// </summary>
        /// <param name="_assetName"></param>
        public void UnloadFairyGUIAsset(string _assetName)
        {
            if (true == string.IsNullOrEmpty(_assetName))
            {
                return;
            }

            UIPackage.RemovePackage(_assetName); // 卸载package
//            UnloadAsset(_assetName); // 卸载资源 由于没有通过LoadAsset加载，所以无需在此处进行调用
        }

        public string GetDatabasePath()
        {
            return Application.dataPath + "/ClientResources/Database/all_data.db";
        }

        /// <summary>
        /// 获取热更新文件的bytes
        /// </summary>
        /// <param name="_assetName"></param>
        /// <returns></returns>
        public byte[] LoadHotfixBytes(string _assetName)
        {
            var textAssetResponse = LoadAsset(_assetName);

            if (null != textAssetResponse && null != textAssetResponse.Data)
            {
                if (true == textAssetResponse.Data is TextAsset)
                {
                    return ((TextAsset) textAssetResponse.Data).bytes;
                }
            }

            return null;
        }

        /// <summary>
        /// 根据名字返回加载路径
        /// 这个方法是在本地加载过程中使用
        /// </summary>
        /// <param name="_assetName"></param>
        /// <returns></returns>
        private string GetAssetPathByName(string _assetName)
        {
            if (true == string.IsNullOrEmpty(_assetName))
            {
                return null;
            }

            if (assetPathMapping.Count <= 0)
            {
                this.Error("资源映射文件数量为0");
            }

            string result = null;
            if (true == assetPathMapping.TryGetValue(_assetName, out result))
            {
                return result;
            }

            return _assetName;
        }
    }
}