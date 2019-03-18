// // ================================================================
// // FileName:ISceneDynamicLoad.cs
// // User: Baron
// // CreateTime:2018/5/22
// // Description: 场景物体动态加载接口
// // ================================================================

using System;
using UnityEngine;

namespace HKLibrary
{
    public interface ISceneDynamicLoad
    {
        /// <summary>
        /// 加载静态物体
        /// </summary>
        /// <param name="_sceneName"></param>
        /// <param name="_centerPoint">中心点，会从这个点附近开始动态加载</param>
        /// <param name="_completeCalback"></param>
        /// <param name="_cacheCurrent">缓存当前</param>
        void LoadStaticItems(string _sceneName, Vector3 _centerPoint, Action _completeCalback = null, bool _cacheCurrent = false);

        /// <summary>
        /// 卸载一个场景
        /// </summary>
        /// <param name="_sceneName"></param>
        /// <param name="_complete"></param>
        void UnloadScene(string _sceneName, Action _complete = null);

    }
}