// // ================================================================
// // FileName:HKSpriteManager.cs
// // User: Baron-Fisher
// // CreateTime:2018 0202 0:38
// // Description:
// // Copyright (c) 2016 Greg.Co.Ltd. All rights reserved.
// // ================================================================

using System;
using Pathfinding;
using UnityEngine;

namespace GameCoreLib
{
    public interface IPathFind
    {
        
        /// <summary>
        /// init
        /// </summary>
        void Init();
        
        /// <summary>
        /// 根据当前场景名字，加载寻路数据
        /// 因为场景名字可以动态获取，所以不需要传参数
        /// </summary>
        void LoadCurrentSceneAStarInfo(string _sceneName, Action _complete);

        /// <summary>
        /// 根据3d世界坐标，获取一个格子索引
        /// </summary>
        /// <param name="_position"></param>
        /// <returns></returns>
        int GetGridIndexByPosition(Vector3 _position);

        /// <summary>
        /// 根据3d世界格子，返回一个2d(x,y)坐标信息
        /// </summary>
        /// <param name="_position"></param>
        /// <returns></returns>
        Vector2 GetGridCoordinateByPosition(Vector3 _position);

        /// <summary>
        /// 根据xy格子，转换为索引
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        int GetIndexByCoordinate(int x, int y);

        /// <summary>
        /// 将获取的索引格子转换为世界的3d坐标
        /// 为ILRT提供的使用方法
        /// ILRT中无法对返回的 Nullable类型进行判断
        /// </summary>
        /// <param name="_gridIndex"></param>
        /// <returns></returns>
        Vector3 GetPositionGridIndex_ILRT(int _gridIndex);

        /// <summary>
        /// 根据索引格子，返回当前世界坐标
        /// </summary>
        /// <param name="_gridIndex"></param>
        /// <returns></returns>
        Vector3? GetPositionByGridIndex(int _gridIndex);

        /// <summary>
        /// 根据2d索引格子，获取一个世界3d坐标
        /// </summary>
        /// <param name="_gridIndex"></param>
        /// <returns></returns>
        Vector3? GetPositionByGridCoordinate(Vector2 _gridIndex);


        /// <summary>
        /// 获取某个世界坐标上的格子详细信息
        /// </summary>
        /// <param name="_position"></param>
        /// <returns></returns>
        GridNode GetPositionNode(Vector3 _position);

        /// <summary>
        /// 获取无效的的位置点
        /// </summary>
        /// <returns></returns>
        Vector3 GetInVaild();
    }
}