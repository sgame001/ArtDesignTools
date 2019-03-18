// // ================================================================
// // FileName:ISceneStaticItemLoader.cs
// // User: Baron
// // CreateTime:2018/5/22
// // Description: 
// // ================================================================

using System;
using Mono;
using UnityEngine;

namespace HKLibrary
{
    public interface ISceneStaticItemLoader
    {
        /// <summary>
        /// 开始分帧加载
        /// </summary>
        void StartFramingInstatiate(Vector3 _centerPoint, Action _completeCalback = null);

        /// <summary>
        /// 释放
        /// 因为采用了分帧卸载，所以提供完成的回调接口
        /// </summary>
        /// <param name="_complete"></param>
        void Dispose(Action _complete = null);
    }
}