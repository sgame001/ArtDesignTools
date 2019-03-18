// // ================================================================
// // FileName:SceneStaticItemLoader.cs
// // User: Baron
// // CreateTime:2018/5/22
// // Description: 场景加载item
// // ================================================================

using System;
using System.Collections.Generic;
using AdvancedCoroutines;
using UnityEngine;
using Object = UnityEngine.Object;

namespace HKLibrary
{
    public class SceneStaticItemLoader : ISceneStaticItemLoader
    {
        /// <summary>
        /// Scene name
        /// </summary>
        private string sceneName = null;

        /// <summary>
        /// 序列化
        /// </summary>
        private SceneSerializedInfo serializedInfoConfig = null;

        /// <summary>
        /// static item source data
        /// </summary>
        Dictionary<string, GameObject> sourceDatasDict = new Dictionary<string, GameObject>();

        /// <summary>
        /// 缓存池
        /// </summary>
        readonly List<GameObject> allCacheStaticGameObjects = new List<GameObject>();

        /// <summary>
        /// routine
        /// </summary>
        private Routine routine;

        /// <summary>
        /// 当前机器性能等级
        /// </summary>
        private HKCommonDefine.DevicePerformance devicePerformance;

        /// <summary>
        /// scene相关的root node
        /// </summary>
        private SceneRootNode sceneRootNode;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="_sceneName"></param>
        /// <param name="_serializedInfo"></param>
        public SceneStaticItemLoader(string _sceneName, SceneSerializedInfo _serializedInfo)
        {
            sceneName            = _sceneName;
            serializedInfoConfig = _serializedInfo;
            devicePerformance    = (HKCommonDefine.DevicePerformance) ConfigSystemFacade.Instance.Get(HKCommonDefine.KEY_PERFORMANCE_LEVEL, (int) HKCommonDefine.DevicePerformance.MIDDLE);
#if UNITY_EDITOR
            // editor下默认最高品质
            devicePerformance = HKCommonDefine.DevicePerformance.HIGHT;
#endif


            // 创建父节点
            sceneRootNode = new SceneRootNode(_sceneName);

            // 切换质量设置
            AppEvent.OnEvent(HKCommonDefine.EVENT_PERFORMANCE_SWITH, EventPerformanceSwith);
        }

        /// <summary>
        /// 切换质量回调
        /// </summary>
        /// <param name="_obj"></param>
        private void EventPerformanceSwith(object _obj)
        {
            devicePerformance = (HKCommonDefine.DevicePerformance) (int) _obj;

            if (null != sceneRootNode)
            {
                sceneRootNode.SwithPerfromance(devicePerformance);
            }
        }

        /// <summary>
        /// 根据配加载一个场景物体
        /// </summary>
        /// <param name="serializedInfo"></param>
        private void FramingInstanceItem(SceneItemSerialized serializedInfo)
        {
            GameObject sourceGo = null;
            if (true == sourceDatasDict.ContainsKey(serializedInfo.PrefabName))
            {
                sourceGo = sourceDatasDict[serializedInfo.PrefabName];
            }
            else
            {
                this.Debug("不包含 key = " + serializedInfo.PrefabName);
            }

            var noLightLayer = LayerMask.NameToLayer("NO_LIGHT");
            if (null != sourceGo)
            {
                var go          = Object.Instantiate(sourceGo);
                var goTransform = go.transform;
                goTransform.localPosition    = serializedInfo.Position;
                goTransform.localEulerAngles = serializedInfo.LocalEulerAngles;
                goTransform.localScale       = serializedInfo.LocalScale;
                var meshRenders = go.GetComponentsInChildren<MeshRenderer>();
                for (var meshRenderIndex = 0; meshRenderIndex < meshRenders.Length; meshRenderIndex++)
                {
                    var meshRenderer = meshRenders[meshRenderIndex];
                    if (serializedInfo.LightMapInfos.Count > meshRenderIndex)
                    {
                        meshRenderer.lightmapIndex       = serializedInfo.LightMapInfos[meshRenderIndex].LightMapIndex;
                        meshRenderer.lightmapScaleOffset = serializedInfo.LightMapInfos[meshRenderIndex].LightMapScaleOffset;
                    }

                    if (true == serializedInfo.IsTransparent || true == serializedInfo.IsFX)
                    {
                        meshRenderer.gameObject.layer = noLightLayer; // 因为first访问不到，所以只能写LayerMask转换了
                    }
                }

                allCacheStaticGameObjects.Add(go); // 添加到all队列中

                if (true == serializedInfo.IsFX)
                {
                    // 特效节点
                    goTransform.parent = sceneRootNode.FxNode;
                }
                else if (true == serializedInfo.IsTransparent)
                {
                    // 特效之外的半透物体节点
                    goTransform.parent = sceneRootNode.TransparentNode;
                }
                else
                {
                    // 静态物体节点
                    goTransform.parent = sceneRootNode.StaticItemNode;
                }
            }
            else
            {
                this.Debug("无法加载prefab = " + serializedInfo.PrefabName);
            }
        }


        /// <summary>
        /// 开始分帧加载
        /// </summary>
        public void StartFramingInstatiate(Vector3 _centerPoint, Action _completeCalback = null)
        {
            // 先进行一次排序
            if (null != serializedInfoConfig && serializedInfoConfig.ItemSerializeds.Count > 0)
            {
                serializedInfoConfig.ItemSerializeds.Sort(( serialized, _other) =>
                {
                    float dis      = Vector3.Distance(serialized.Position, _centerPoint);
                    float otherDis = Vector3.Distance(_other.Position, _centerPoint);
                    return dis.CompareTo(otherDis);
                });
            }

            // 先异步加载资源
            ResourcesMgrFacade.Instance.LoadScenePrefabItems(sceneName, _args =>
            {
                if (null != _args)
                {
                    sourceDatasDict = _args;
                }

                if (allCacheStaticGameObjects.Count > 0)
                {
                    EventPerformanceSwith(devicePerformance); /**加载完成后，默认执行一次质量切换*/
                    if (null != _completeCalback)
                    {
                        _completeCalback();
                    }
                }
                else
                {
                    // 调用分帧加载
                    FramingFacade.Instance.FramingExecuteArgs(serializedInfoConfig.ItemSerializeds, 3, (_i, _serialized) =>
                    {
                        if (null != _serialized)
                        {
                            FramingInstanceItem(_serialized);
                        }
                    }, () =>
                    {
                        // 合并静态物体
                        if (null != sceneRootNode)
                        {
                            StaticBatchingUtility.Combine(sceneRootNode.StaticItemNode.gameObject);  // 合并静态物体
                            StaticBatchingUtility.Combine(sceneRootNode.TransparentNode.gameObject); // 合并透贴物体
                        }


                        EventPerformanceSwith(devicePerformance); /**加载完成后，默认执行一次质量切换*/
                        if (null != _completeCalback)
                        {
                            _completeCalback();
                        }
                    });
                }
            });
        }

        /// <summary>
        /// 停止分帧加载
        /// </summary>
        private void StopInstatiate()
        {
            if (null != routine)
            {
                CoroutineManager.StopCoroutine(routine);
                routine = null;
            }
        }


        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose(Action _complete = null)
        {
            if (null != routine)
            {
                StopInstatiate();
            }

            // 分帧卸载
            FramingFacade.Instance.FramingExecuteByTimeArgs(allCacheStaticGameObjects, 1f /**1s内要卸载完成*/, ( i, _args) => { Object.Destroy(_args); }, () =>
            {
                serializedInfoConfig = null;
                CleraCache();
                sourceDatasDict.Clear();
                ResourcesMgrFacade.Instance.ReleaseSceneItems(sceneName);

                if (null != sceneRootNode)
                {
                    sceneRootNode.Dispose();
                    sceneRootNode = null;
                }

                if (null != _complete)
                {
                    _complete();
                }
            });

            // 不用等待释放结束，开始决定释放的时候，就不能再接收品质切换消息
            AppEvent.RemoveEvent(HKCommonDefine.EVENT_PERFORMANCE_SWITH); // 移除消息监听
        }

        /// <summary>
        /// 清理缓存
        /// </summary>
        private void CleraCache()
        {
            allCacheStaticGameObjects.Clear();
        }
    }
}