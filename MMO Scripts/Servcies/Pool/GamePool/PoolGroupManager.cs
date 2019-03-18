// // // ================================================================
// // // FileName:PoolGroupManager.cs
// // // User: Baron
// // // CreateTime:2017-09-11-17:08
// // // Description:内存组管理器
// // // 
// // // Copyright (c) 2016 Heeking.Co.Ltd. All rights reserved.
// // // ================================================================

using System.Collections.Generic;
using CatLib;
using UnityEngine;

namespace HKLibrary
{
    public class PoolGroupManager : IPoolManager
    {
        /// <summary>
        /// pools gorup
        /// </summary>
        private readonly Dictionary<string, IPoolGroup> poolGroups = new Dictionary<string, IPoolGroup>();

        /// <summary>
        /// 别名映射
        /// </summary>
        private readonly Dictionary<string, string> aliasMap = new Dictionary<string, string>();
        
        
        /// <summary>
        /// 加载接口
        /// 如果是使用Name的方式创建或者加载，必须提前给此接口赋值
        /// </summary>
        public HKCommonDefine.LoadRes LoadResAction { get; set; }


        public void Init()
        {
            AppEvent.OnEvent("unload_asset", o =>
            {
                if (true == o is string)
                {
                    var assetName = (string) o;
                    RemovePoolGroup(assetName);
                }
            });
        }

        /// <summary>
        /// 通过源数据创建一个内存池
        /// </summary>
        /// <param name="_rawResResponse"></param>
        /// <returns></returns>
        public IPoolGroup CreatePool(IResResponse _rawResResponse)
        {
            if (null == _rawResResponse)
            {
                return null;
            }
            IPoolGroup group = GetPoolGroup(_rawResResponse.SourceDataName);
            if (null != group)
            {
                return group;
            }
            IPoolGroup resultPool = new PoolGroup(_rawResResponse);
            string rawName = _rawResResponse.SourceDataName;
            if (false == poolGroups.ContainsKey(rawName))
            {
                resultPool.RawDataName = rawName;
                poolGroups.Add(rawName, resultPool);
            }
            return resultPool;
        }

        /// <summary>
        /// 创建对象池 包含预加载数量以及最大数量的版本
        /// </summary>
        /// <param name="_rawResResponse"></param>
        /// <param name="_preloadNumber">预加载数量</param>
        /// <param name="_maxCount">总大小</param>
        /// <returns></returns>
        public IPoolGroup CreatePool(IResResponse _rawResResponse, int _preloadNumber, int _maxCount)
        {
            if (null == _rawResResponse)
            {
                return null;
            }
            IPoolGroup group = GetPoolGroup(_rawResResponse.SourceDataName);
            if (null != group)
            {
                return group;
            }
            group = CreatePool(_rawResResponse);
            group.SetPreloadNumber(_preloadNumber).SetMaxCount(_maxCount);
            return group;
        }


        /// <summary>
        /// 创建一个内存池,并且直接启动
        /// </summary>
        /// <param name="_rawResResponse"></param>
        /// <param name="_preloadNumber">预加载数量</param>
        /// <param name="_maxCount">最大数量</param>
        /// <param name="_intervalCheck">间隔检测时间</param>
        /// <param name="_expireTime">过期时间</param>
        public void CreatePool(IResResponse _rawResResponse, int _preloadNumber, int _maxCount, int _intervalCheck,
            int _expireTime)
        {
            IPoolGroup group = GetPoolGroup(_rawResResponse.SourceDataName);
            if (null != group)
            {
                return;
            }
            group = CreatePool(_rawResResponse, _preloadNumber, _maxCount);
            if (null != group)
            {
                group.SetMaxCount(_maxCount).SetCleraIntervalTime(_intervalCheck).SetExpiredTime(_expireTime);
                group.Run();
            }
        }
        

        /// <summary>
        /// 移除一个缓存池
        /// </summary>
        /// <param name="_name"></param>
        public void RemovePoolGroup(string _name)
        {
            var group = GetPoolGroup(_name);
            if (null != group)
            {
                group.Dispose();
                poolGroups.Remove(_name);
                group = null;
                this.Debug("移除内存池" + _name);
            }
        }

        /// <summary>
        /// 获取一个缓存池
        /// </summary>
        /// <param name="_name"></param>
        /// <returns></returns>
        public IPoolGroup GetPoolGroup(string _name)
        {
            if (true == string.IsNullOrEmpty(_name))
            {
                return null;
            }

            if (true == aliasMap.ContainsKey(_name))
            {
                _name = aliasMap[_name];
            }
            
            if (false == poolGroups.ContainsKey(_name))
            {
//                this.Warr(StringCacheFactoryFacade.Instance.GetFree().Add("无法获取PoolGroup")
//                    .Add(_name));
                return null;
            }

            return poolGroups[_name];
        }

        /// <summary>
        /// 根据名字获取一个空闲对象
        /// </summary>
        /// <param name="_name"></param>
        /// <returns></returns>
        public GameObject GetFree(string _name)
        {
            if (true == string.IsNullOrEmpty(_name))
            {
                return null;
            }

            var poolGroup = GetPoolGroup(_name);
            if (null != poolGroup)
            {
                return poolGroup.GetFree();
            }
            return null;
        }

        /// <summary>
        /// 回收一个对象
        /// </summary>
        /// <param name="_go"></param>
        public void Recover(GameObject _go)
        {
            if (null == _go)
            {
                return;
            }
            var poolEntity = _go.GetComponent<IPoolEntity>();
            if (null != poolEntity)
            {
                var poolGroup = GetPoolGroup(poolEntity.ParentGroupName);
                if (null != poolGroup)
                {
                    poolGroup.Recover(_go);
                    return;
                }
            }
            Object.Destroy(_go);
        }

        /// <summary>
        /// 设置别名
        /// </summary>
        /// <param name="_sourceName"></param>
        /// <param name="_aliasName"></param>
        public void SetAlias(string _sourceName, string _aliasName)
        {
            if (true == string.IsNullOrEmpty(_sourceName) || true == string.IsNullOrEmpty(_aliasName))
            {
                return;
            }

            if (false == poolGroups.ContainsKey(_sourceName))
            {
                this.Error(string.Format("不包含 {0} 的缓存池, 无法设置别名", _sourceName));
                return;
            }

            if (true == aliasMap.ContainsKey(_sourceName))
            {
                aliasMap[_sourceName] = _aliasName;
            }
            else
            {
                aliasMap.Add(_sourceName, _aliasName);
            }
        }

#if UNITY_EDITOR
        
        /// <summary>
        /// 获取所有的内存池
        /// 这个方法是为了重绘数据使用的
        /// 在实际运行时，并不需要调用这个方法
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, IPoolGroup> GetPoolGroups()
        {
            return poolGroups;
        }
#endif
    }
}