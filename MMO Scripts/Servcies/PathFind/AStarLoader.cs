// // ================================================================
// // FileName:AStarLoader.cs
// // User: Baron
// // CreateTime:2018/5/26
// // Description: AStart Loader 管理单个场景的A*信息，为了缓存而创建
// // ================================================================

using System;
using HKLibrary;
using UnityEngine;
using Object = UnityEngine.Object;

namespace GameCoreLib
{
    public class AStarLoader
    {
        /// <summary>
        /// 场景名字
        /// </summary>
        private string SceneName;

        /// <summary>
        /// 当前的格子信息
        /// </summary>
        public AStartNodeInfo.AStarGrid CurrentMapGridInfo { get; set; }

        /// <summary>
        /// A* 信息
        /// </summary>
        public AstarPath CurrentAStarPath { get; set; }

        /// <summary>
        /// A* gameobject 信息
        /// </summary>
        public GameObject aStarGameoObject { get; set; }

        /// <summary>
        /// root
        /// </summary>
        private GameObject aStarRoot = null;

        public AStarLoader(string _sceneName, GameObject _root)
        {
            if (true == string.IsNullOrEmpty(_sceneName))
            {
                this.Error("场景名字为空，无法加载A*信息");
                return;
            }

            SceneName = _sceneName;
            aStarRoot = _root;
        }


        public void LoadAStartInfo(Action _complete)
        {
            // 加载A*信息
            LoadAStarInfo(SceneName, () =>
            {
                // 统计A*信息
                CurrentMapGridInfo = null;
                if (null == CurrentMapGridInfo && null != aStarGameoObject)
                {
                    CurrentAStarPath = aStarGameoObject.GetComponent<AstarPath>();
                    if (null == CurrentAStarPath)
                    {
                        this.Error(string.Format("无法获取当前地图场景的格子信息 | {0}", SceneName));
                        return;
                    }

                    AStartNodeInfo.AStarGrid aStarGridInfo = new AStartNodeInfo.AStarGrid();
                    var                      gridGraph     = CurrentAStarPath.data.gridGraph;
                    aStarGridInfo.size_width  = gridGraph.width;
                    aStarGridInfo.size_height = gridGraph.depth;

                    var sourceTempX = (-gridGraph.width * 0.5f) + gridGraph.center.x;
                    var sourceTempZ = (-gridGraph.depth * 0.5f) + gridGraph.center.z;

                    // 计算原点
                    aStarGridInfo.source_x = Math.Round(sourceTempX, 2);
                    aStarGridInfo.source_z = Math.Round(sourceTempZ, 2);

                    CurrentMapGridInfo = aStarGridInfo;

                    if (null != _complete)
                    {
                        _complete();
                    }
                }
            });
        }


        /// <summary>
        /// 加载A*资源
        /// A*资源比较特殊
        /// 暂时不做保存，每次重新加载一个
        /// </summary>
        /// <param name="_sceneName"></param>
        /// <param name="_complete"></param>
        private void LoadAStarInfo(string _sceneName, Action _complete)
        {
            if (true == string.IsNullOrEmpty(_sceneName))
            {
                return;
            }

            // 加载新的
            string aStarResName = string.Format("{0}_ASTAR", _sceneName);
            ResourcesMgrFacade.Instance.LoadAssetAsync(aStarResName, response =>
            {
                if (null == response)
                {
                    this.Error("A*信息加载失败 = " + aStarResName);
                    return;
                }

                aStarGameoObject = response.Instantiate();
                if (null != aStarGameoObject)
                {
                    aStarGameoObject.name = aStarResName;

                    if (null != aStarRoot)
                    {
                        aStarGameoObject.transform.parent = aStarRoot.transform;
                    }
                }

                if (null != _complete)
                {
                    _complete();
                }
            });
        }

        /// <summary>
        /// 释放到缓存池中
        /// </summary>
        public void Release()
        {
            if (null != aStarGameoObject)
            {
                aStarGameoObject.SetActive(false);
            }
        }


        /// <summary>
        /// dispose
        /// </summary>
        public void Dispose()
        {
            if (null != aStarGameoObject)
            {
                string aStarResName = string.Format("{0}_ASTAR", SceneName);
                ResourcesMgrFacade.Instance.UnloadAsset(aStarResName);
            }

            aStarRoot = null;
        }
    }
}