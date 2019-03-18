// // ================================================================
// // FileName:HKSpriteManager.cs
// // User: Baron-Fisher
// // CreateTime:2017 0910 19:50
// // Description:Pool Manager 内存池管理器，主要是管理Group
// // Copyright (c) 2016 Greg.Co.Ltd. All rights reserved.
// // ================================================================

using System.Collections.Generic;
using UnityEngine;

namespace HKLibrary
{
    public interface IPoolManager
    {
        /// <summary>
        /// 加载接口
        /// </summary>
        HKCommonDefine.LoadRes LoadResAction { get; set; }

        /// <summary>
        /// init
        /// </summary>
        void Init();
        
        /// <summary>
        /// 创建一个Pool
        /// </summary>
        IPoolGroup CreatePool(IResResponse _rawResResponse);

        /// <summary>
        /// 创建对象池 包含预加载数量以及最大数量的版本
        /// </summary>
        /// <param name="_rawResResponse"></param>
        /// <param name="_preloadNumber">预加载数量</param>
        /// <param name="_maxCount">总大小</param>
        /// <returns></returns>
        IPoolGroup CreatePool(IResResponse _rawResResponse, int _preloadNumber, int _maxCount);

        /// <summary>
        /// 创建一个内存池,并且直接启动
        /// </summary>
        /// <param name="_rawResResponse"></param>
        /// <param name="_preloadNumber">预加载数量</param>
        /// <param name="_maxCount">最大数量</param>
        /// <param name="_intervalCheck">间隔检测时间</param>
        /// <param name="_expireTime">过期时间</param>
        void CreatePool(IResResponse _rawResResponse, int _preloadNumber, int _maxCount, int _intervalCheck,
            int _expireTime);
        
        /// <summary>
        /// Group Name
        /// </summary>
        /// <param name="_name"></param>
        void RemovePoolGroup(string _name);

        /// <summary>
        /// 根据名字获取一个Pool Group对象
        /// </summary>
        /// <param name="_name"></param>
        /// <returns></returns>
        IPoolGroup GetPoolGroup(string _name);


        /// <summary>
        /// 获取一个空闲的对象
        /// </summary>
        /// <param name="_name"></param>
        /// <returns></returns>
        GameObject GetFree(string _name);

        /// <summary>
        /// 释放一个对象
        /// </summary>
        /// <param name="_go"></param>
        void Recover(GameObject _go);


        /// <summary>
        /// 设置别名映射
        /// </summary>
        /// <param name="_sourceName"></param>
        /// <param name="_aliasName"></param>
        void SetAlias(string _sourceName, string _aliasName);
        
        
#if UNITY_EDITOR
        /// <summary>
        /// 获取所有的内存池
        /// 这个方法是为了重绘数据使用的
        /// 在实际运行时，并不需要调用这个方法
        /// </summary>
        /// <returns></returns>
        Dictionary<string, IPoolGroup> GetPoolGroups();
#endif
    }
}