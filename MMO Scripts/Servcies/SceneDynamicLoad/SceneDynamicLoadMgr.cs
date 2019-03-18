// // ================================================================
// // FileName:SceneDynamicLoadMgr.cs
// // User: Baron
// // CreateTime:2018/5/22
// // Description: 动态的加载场景中物体，场景中物体加载不需要缓存
// // ================================================================

using System;
using System.Collections;
using System.Collections.Generic;
using GameCoreLib;
using UnityEngine;

namespace HKLibrary
{
    public class SceneDynamicLoadMgr : ISceneDynamicLoad
    {
        /// <summary>
        /// 场景信息队列
        /// 因为数据量并不大，所以直接缓存
        /// </summary>
        private readonly Dictionary<string, SceneSerializedInfo> serializedInfosDict = new Dictionary<string, SceneSerializedInfo>();

        /// <summary>
        /// 当前正在显示的场景配置信息
        /// </summary>
        private SceneStaticItemLoader currentShowSceneLoader = null;

        /// <summary>
        /// scene loader队列
        /// 在队列中都是没有释放的
        /// </summary>
        private readonly Dictionary<string, SceneStaticItemLoader> SceneStaticItemLoadersDict = new Dictionary<string, SceneStaticItemLoader>();

        /// <summary>
        /// 加载场景中的静态物体
        /// </summary>
        /// <param name="_sceneName"></param>
        /// <param name="_centerPoint"></param>
        /// <param name="_completeCalback">完成事件回调</param>
        /// <param name="_cacheCurrent">是否缓存当前场景，这是一个高速加载的机制，用空间换时间，在3G内存以上的机器上有效</param>
        public void LoadStaticItems(string _sceneName, Vector3 _centerPoint, Action _completeCalback = null, bool _cacheCurrent = false)
        {
            if (DeviceInfo.GetMemorySize() < 3) // 3G内存下，不考虑缓存
            {
                _cacheCurrent = false;
            }

            LoadSceneStaticConfig(_sceneName, info =>
            {
                if (info.ItemSerializeds.Count > 0)
                {
                    SceneStaticItemLoader sceneLoader = null;
                    if (false == SceneStaticItemLoadersDict.TryGetValue(_sceneName, out sceneLoader))
                    {
                        sceneLoader = new SceneStaticItemLoader(_sceneName, info);
                        SceneStaticItemLoadersDict.Add(_sceneName, sceneLoader);
                    }

                    if (null != sceneLoader)
                    {
                        sceneLoader.StartFramingInstatiate(_centerPoint, _completeCalback);
                        currentShowSceneLoader = sceneLoader;
                    }
                }
                else
                {
                    this.Error("静态物体数量为空");
                }
            });
        }


        /// <summary>
        /// 卸载一个场景
        /// </summary>
        /// <param name="_sceneName"></param>
        /// <param name="_complete"></param>
        public void UnloadScene(string _sceneName, Action _complete = null)
        {
            SceneStaticItemLoader sceneStaticItemLoader = null;
            if (true == SceneStaticItemLoadersDict.TryGetValue(_sceneName, out sceneStaticItemLoader))
            {
                sceneStaticItemLoader.Dispose(() =>
                {
                    this.Debug("场景卸载完成 = " + _sceneName);
                    if (null != _complete)
                    {
                        _complete();
                    }
                });

                SceneStaticItemLoadersDict.Remove(_sceneName);
            }
        }


        /// <summary>
        /// 通过名字加载一个场景静态物体配置
        /// </summary>
        private void LoadSceneStaticConfig(string _sceneName, Action<SceneSerializedInfo> _serializedInfoCallback)
        {
            if (true == string.IsNullOrEmpty(_sceneName))
            {
                return;
            }

            if (true == serializedInfosDict.ContainsKey(_sceneName))
            {
                if (null != _serializedInfoCallback)
                {
                    _serializedInfoCallback(serializedInfosDict[_sceneName]);
                    return;
                }
            }

            var configName = string.Format("{0}_Serialized", _sceneName);


            ResourcesMgrFacade.Instance.LoadAssetAsync(configName, _cfgResponse =>
            {
                if (null == _cfgResponse)
                {
                    this.Error("无法加载场景静态物品配置文件 = " + configName);
                    return;
                }

                if (null != _cfgResponse.Data)
                {
                    SceneSerializedInfo serializedInfo = (SceneSerializedInfo) _cfgResponse.Data;
                    if (null != _serializedInfoCallback)
                    {
                        if (false == serializedInfosDict.ContainsKey(_sceneName))
                        {
                            serializedInfosDict.Add(_sceneName, serializedInfo);
                        }

                        _serializedInfoCallback(serializedInfo);
                    }
                }
                else
                {
                    this.Error("配置文件加载失败 = " + _sceneName);
                }
            });
        }
    }
}