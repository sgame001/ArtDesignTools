// // ================================================================
// // FileName:AStarHelper.cs
// // User: Baron
// // CreateTime:12/21/2017
// // Description: A* 寻路的辅助类，主要是用来加载A*信息
// // ================================================================

using System;
using CatLib;
using HKLibrary;
using ILRuntime.Runtime;
using Pathfinding;
using UnityEngine;
using Object = UnityEngine.Object;

namespace GameCoreLib
{
    public class AStarHelper : IPathFind
    {
        /// <summary>
        /// 单个格子的X
        /// </summary>
        public const float GRID_WIDTH = 1f;

        /// <summary>
        /// 单个格子的Z
        /// </summary>
        public const float GRID_HEIGHT = 1f;

        /// <summary>
        /// A* 自带的一个常量，一个寻路的插值
        /// </summary>
        private float forwardLook = 1;

        /// <summary>
        /// 当前正在激活的scene name
        /// </summary>
        private string currSceneName;

        /// <summary>
        /// A* loader
        /// </summary>
        private AStarLoader currAstartLoader = null;

        /// <summary>
        /// a*信息缓存
        /// 目前只缓存最多10个场景的
        /// </summary>
        private readonly LruCache<string, AStarLoader> astarLoaderCache = new LruCache<string, AStarLoader>(10);

        /// <summary>
        /// a star root
        /// </summary>
        private GameObject aStarRoot;
        
        /// <summary>
        /// init
        /// </summary>
        public void Init()
        {
            astarLoaderCache.OnRemoveLeastUsed += AStarLoaderCacheOnOnRemoveLeastUsed;
            if (null == aStarRoot)
            {
                aStarRoot = GameObject.Find("AStarRoot");
            }
        }

        /// <summary>
        /// 当a* start被动从cache中移被除时
        /// </summary>
        /// <param name="_sceneName"></param>
        /// <param name="_aStarLoader"></param>
        private void AStarLoaderCacheOnOnRemoveLeastUsed(string _sceneName, AStarLoader _aStarLoader)
        {
            if (null != _aStarLoader)
            {
                _aStarLoader.Dispose();
            }
        }

        
        /// <summary>
        /// 加载当前场景的A*格子信息
        /// </summary>
        public void LoadCurrentSceneAStarInfo(string _sceneName, Action _complete)
        {
            if (currSceneName == _sceneName)
            {
                if (null != _complete)
                {
                    _complete();
                    return;
                }
            }
            currSceneName = _sceneName;
            var loader = astarLoaderCache.Get(_sceneName, null);
            if (null == loader)
            {
                loader = new AStarLoader(_sceneName, aStarRoot);
                loader.LoadAStartInfo(_complete);
                astarLoaderCache.Add(_sceneName, loader);
            }
            else
            {
                if (null != _complete)
                {
                    _complete();
                }
            }

            // 不需要等待加载完成，直接赋值
            currAstartLoader = loader;
        }

        /// <summary>
        /// 返回格子的索引
        /// </summary>
        /// <param name="_position"></param>
        /// <returns></returns>
        public int GetGridIndexByPosition(Vector3 _position)
        {
            Vector2 coordinate = GetGridCoordinateByPosition(_position);
            if (coordinate.x == -1 && coordinate.y == -1)
            {
                return -1;
            }

            var nodeIndex = (int) coordinate.y * currAstartLoader.CurrentMapGridInfo.size_width + (int) coordinate.x;
            //            this.LogMessageError("x = ", (int)coordinate.x, " y = ", coordinate.y);
            return nodeIndex;
        }

        /// <summary>
        /// 根据坐标获取格子的索引位置
        /// </summary>
        /// <param name="_position"></param>
        /// <returns></returns>
        public Vector2 GetGridCoordinateByPosition(Vector3 _position)
        {
            if (null == currAstartLoader.CurrentMapGridInfo)
            {
                return new Vector2(-1, -1); // 负数是无效格子，格子是从(0,0)坐标系分布在第一象限的
            }

            double x = _position.x - currAstartLoader.CurrentMapGridInfo.source_x;
            double z = _position.z - currAstartLoader.CurrentMapGridInfo.source_z;

            var int_grid_x = ConverMapGridIndex(x / GRID_WIDTH);

            var int_grid_z = ConverMapGridIndex(z / GRID_HEIGHT);

            return new Vector2(int_grid_x, int_grid_z);
        }


        /// <summary>
        /// 转换xy到grid index
        /// </summary>
        /// <returns></returns>
        public int GetIndexByCoordinate(int x, int y)
        {
            var      gridIndex = y * currAstartLoader.CurrentMapGridInfo.size_width + x;
            Vector3? target    = GetPositionByGridIndex(gridIndex);
            if (null == target)
            {
                return -1;
            }
            else
            {
                return gridIndex;
            }
        }

        /// <summary>
        /// 为ILRT提供的使用方法
        /// ILRT中无法对返回的 Nullable类型进行判断
        /// </summary>
        /// <param name="_gridIndex"></param>
        /// <returns></returns>
        public Vector3 GetPositionGridIndex_ILRT(int _gridIndex)
        {
            var result = GetPositionByGridIndex(_gridIndex);
            if (null != result)
            {
                return result.Value;
            }

            return HKCommonDefine.IN_VAILD_VECTOR3; // 无效位置
        }


        /// <summary>
        /// 根据格子返回坐标点
        /// </summary>
        /// <param name="_gridIndex"></param>
        /// <returns></returns>
        /// <exception cref="IndexOutOfRangeException"></exception>
        public Vector3? GetPositionByGridIndex(int _gridIndex)
        {
            if (null == currAstartLoader.CurrentAStarPath.data.gridGraph)
            {
                return null;
            }

            if (_gridIndex > currAstartLoader.CurrentAStarPath.data.gridGraph.nodes.Length)
            {
                Debug.LogError("path invalid = " + _gridIndex);
                throw new IndexOutOfRangeException(_gridIndex + "path invalid =");
                return new Vector3(-1, -1, -1);
            }

            var node = currAstartLoader.CurrentAStarPath.data.gridGraph.nodes[_gridIndex];

            return (Vector3) node.position; // Int3 强转为 Vector3
        }


        /// <summary>
        /// 根据格子转换为世界坐标
        /// </summary>
        /// <param name="_gridIndex"></param>
        /// <returns></returns>
        public Vector3? GetPositionByGridCoordinate(Vector2 _gridIndex)
        {
            if (_gridIndex.x < 0 || _gridIndex.y < 0)
            {
                this.Error("格子信息有<0的数据");
                //                return new Vector3(-1, -1, -1);
                return null;
            }

            if (null == currAstartLoader.CurrentAStarPath)
            {
                this.Error("AStarLoader.Path信息为空");
                return null;
            }

            var nodeIndex = (int) _gridIndex.y * currAstartLoader.CurrentMapGridInfo.size_width + (int) _gridIndex.x; /**(int)((_gridIndex.x + 1) * (_gridIndex.y + 1));*/

            //            var node = AStarPath.astarData.gridGraph.nodes[nodeIndex];
            //
            //            return (Vector3)node.position;// Int3 强转为 Vector3
            return GetPositionByGridIndex(nodeIndex);
        }


        /// <summary>
        /// 获取前方某个点的可行走信息
        /// </summary>
        /// <param name="_position"></param>
        /// <returns></returns>
        public GridNode GetPositionNode(Vector3 _position)
        {
            var gridIndex = GetGridCoordinateByPosition(_position);                                             // 先获取格子索引
            var nodeIndex = (int) gridIndex.y * currAstartLoader.CurrentMapGridInfo.size_width + (int) gridIndex.x; // 获取node信息

            if (null == currAstartLoader.CurrentAStarPath)
            {
                return null;
            }

            if (nodeIndex >= currAstartLoader.CurrentAStarPath.data.gridGraph.nodes.Length)
            {
                return null;
            }

            if (nodeIndex > 0 && nodeIndex < currAstartLoader.CurrentAStarPath.data.gridGraph.nodes.Length)
            {
                var node = currAstartLoader.CurrentAStarPath.data.gridGraph.nodes[nodeIndex];
                return node;
            }

            return null;
        }

        /// <summary>
        /// 无效的位置点
        /// </summary>
        /// <returns></returns>
        public Vector3 GetInVaild()
        {
            return HKCommonDefine.IN_VAILD_VECTOR3;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="_f"></param>
        /// <returns></returns>
        private static int ConverMapGridIndex(double _f)
        {
            return (int) _f; // 直接强转，不会四舍五入，如果要四舍五入用Conver.ToInt
        }


        /// <summary>
        /// 貌似是计算直线的相对角度
        /// 暂时没用上
        /// </summary>
        /// <param name="p">当前点</param>
        /// <param name="a">寻路结果的起始点</param>
        /// <param name="b">寻路结果的结束点</param>
        /// <returns></returns>
        public Vector3 CalculateTargetPoint(Vector3 p, Vector3 a, Vector3 b)
        {
            a.y = p.y;
            b.y = p.y;

            float magn = (a - b).magnitude;
            if (magn == 0) return a;

            float   closest  = Mathf.Clamp01(VectorMath.ClosestPointOnLineFactor(a, b, p));
            Vector3 point    = (b - a) * closest + a;
            float   distance = (point - p).magnitude;

            float lookAhead = Mathf.Clamp(forwardLook - distance, 0.0F, forwardLook);

            float offset = lookAhead / magn;
            offset = Mathf.Clamp(offset + closest, 0.0F, 1.0F);
            return (b - a) * offset + a;
        }
    }
}