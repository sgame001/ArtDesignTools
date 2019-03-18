// // ================================================================
// // FileName:HKSpriteManager.cs
// // User: Baron-Fisher
// // CreateTime:2017 0910 21:26
// // Description:POOL的具体实现
// // Copyright (c) 2016 Greg.Co.Ltd. All rights reserved.
// // ================================================================

using System;
using System.Collections.Generic;
using CatLib;
using CatLib.API.Timer;
using UnityEngine;
using Object = UnityEngine.Object;

namespace HKLibrary
{
    /// <summary>
    /// 回收类型
    /// </summary>
    public enum RecoverEventType
    {
        Deactive, // 隐藏
        MovePos   // 移动到屏幕外
    }

    public class PoolGroup : IPoolGroup
    {
        /// <summary>
        /// 使用中的队列
        /// </summary>
        public QuickList<ItemEntity> UsePools;

        /// <summary>
        /// 空闲队列
        /// </summary>
        public QuickList<ItemEntity> FreePools;

        /// <summary>
        /// 是否正在运行
        /// </summary>
        private bool isRunning = false;

        /// <summary>
        /// 过期的对象
        /// </summary>
        private readonly List<ItemEntity> expireItems = new List<ItemEntity>();

        /// <summary>
        /// 回收类型
        /// </summary>
        private RecoverEventType recoverType = RecoverEventType.Deactive;


        /// <summary>
        /// 源数据名称
        /// </summary>
        public string RawDataName { get; set; }

        /// <summary>
        /// 回收类型
        /// </summary>
        public RecoverEventType RecoverType
        {
            get { return recoverType; }
        }

        /// <summary>
        /// 设置回收类型
        /// </summary>
        /// <param name="_value"></param>
        /// <returns></returns>
        public IPoolGroup SetRecoverType(RecoverEventType _value)
        {
            recoverType = _value;
            return this;
        }

        /// <summary>
        /// 源对象数据
        /// </summary>
        private readonly IResResponse rawResResponse;

        /// <summary>
        /// raw object 缓存的名称
        /// </summary>
        private readonly string rawObjectName;

        /// <summary>
        /// 预加载数量
        /// </summary>
        protected int preloadNumber;


        /// <summary>
        /// 预加载数量
        /// </summary>
        public int PreloadNumber
        {
            get { return preloadNumber; }
        }

        public IPoolGroup SetPreloadNumber(int _value)
        {
            preloadNumber = _value;
            if (preloadNumber > maxCount)
            {
                SetMaxCount(preloadNumber);
            }

            return this;
        }

        /// <summary>
        /// 组的GameObject节点
        /// </summary>
        private Transform groupNodeTransform;


        /// <summary>
        /// 最大数量
        /// </summary>
        protected int maxCount;

        public int MaxCount
        {
            get { return maxCount; }
        }

        public IPoolGroup SetMaxCount(int _value)
        {
            maxCount = _value;
            return this;
        }

        /// <summary>
        /// 过期时间
        /// </summary>
        protected int expiredTime;

        public int ExpiredTime
        {
            get { return expiredTime; }
        }

        public IPoolGroup SetExpiredTime(int _value)
        {
            expiredTime = _value;
            return this;
        }

        /// <summary>
        /// 清理间隔检测时间
        /// </summary>
        protected int clearIntervalTime;

        public int CleraIntervalTime
        {
            get { return clearIntervalTime; }
        }

        /// <summary>
        /// 轮询时间
        /// </summary>
        private ITimer intervalTimerHandler;

        public IPoolGroup SetCleraIntervalTime(int _value)
        {
            clearIntervalTime = _value;
            return this;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public PoolGroup(IResResponse rawGameResResponse)
        {
            rawResResponse = rawGameResResponse;
            if (null != rawResResponse && null != rawGameResResponse.Data)
            {
                rawObjectName = rawResResponse.Data.name;
            }

            Init();
        }


        /// <summary>
        /// 初始化节点
        /// </summary>
        private void CreateGoNode()
        {
            var rootGameobject = GameObject.Find("PoolRootNode");
            if (null == rootGameobject)
            {
                return;
            }

            var parentNode = rootGameobject.transform;
            if (null != parentNode)
            {
                if (null != rawResResponse && null == groupNodeTransform)
                {
                    // 创建一个当前的节点
                    groupNodeTransform = new GameObject(string.Format("[{0}]", rawObjectName)).transform;

                    // 设置在主节点下面
                    groupNodeTransform.transform.parent = parentNode;
                }
            }
        }

        /// <summary>
        /// init
        /// group 初始化操作
        /// </summary>
        public void Init()
        {
            // 对数据进行初始化
            preloadNumber     = 3;
            maxCount          = 20;
            expiredTime       = 20;                                     // 过期时间 单位秒
            clearIntervalTime = (10 + UnityEngine.Random.Range(0, 10)); // 清理检测时间，单位秒 这里使用一个随机值，防止同一个批次的内存池同时清理，造成卡顿现象

            UsePools  = new QuickList<ItemEntity>();
            FreePools = new QuickList<ItemEntity>();

//            this.Info(string.Format(
//                "创建内存池 = {0} preload number = {1}  max count = {2} expired time ={3} clear interval time = {4}",
//                rawObjectName, preloadNumber, maxCount, expiredTime, clearIntervalTime));
        }

        /// <summary>
        /// Setup
        /// 启动池子，在这之前，可以对池子进行各种参数设置
        /// </summary>
        public void Run()
        {
            if (true == isRunning)
            {
                return;
            }

            // 创建节点相关的内容
            CreateGoNode();

            // 处理预加载数据
            PreloadDatas();

            // 启动轮询定时器
            if (null == intervalTimerHandler)
            {
                intervalTimerHandler = App.Make<ITimerManager>().Make(IntervalEvent).Interval(clearIntervalTime);
            }

            isRunning = true;
        }

        /// <summary>
        /// 加载原始数据
        /// </summary>
        private void PreloadDatas()
        {
            // 预加载数据
            if (null != rawResResponse)
            {
                if (PreloadNumber > 0)
                {
                    for (int index = 0; index < PreloadNumber; index++) // 预加载资源
                    {
                        var preloadGameObj = CreateNewItem();
                        FreePools.Push(preloadGameObj);
                        if (null != preloadGameObj)
                        {
                            OnRecoverAction(preloadGameObj);
                        }
                    }
                }
            }
            else
            {
                this.Error("Pool Group 源数据为空");
            }
        }


        /// <summary>
        /// 获取一个空闲对象
        /// </summary>
        /// <returns></returns>
        public GameObject GetFree()
        {
            if (null == rawResResponse)
            {
                return null;
            }

            ItemEntity result = null;

            if (FreePools.Count <= 0)
            {
                result = CreateNewItem();
                if (null != result)
                {
                    result.transform.parent = groupNodeTransform; // 默认放在内存池节点下
                }
            }
            else
            {
                // 先从尾部pop出去
                result = FreePools.Pop();
                if (null == result) // 如果是回收时的对象已经是空对象，被Destroy掉了，那么取出来的时候，仍然可能是空的
                {
                    result = CreateNewItem(); // new的时候已经插入到链表头部了
                }
            }

            if (null != result)
            {
                result.PoolInitEvent();
                result.gameObject.SetActive(true); // 默认显示出来
                result.hideFlags = HideFlags.None;
                UsePools.Push(result); // 添加到使用队列中
                return result.gameObject;
            }

            return null;
        }


        /// <summary>
        /// 获取一个空闲的对象
        /// </summary>
        public void GetFreeAsync(Action<GameObject> _callback)
        {
            if (null == _callback)
            {
                return;
            }
        }


        /// <summary>
        /// 创建一个新的对象
        /// </summary>
        /// <returns></returns>
        private ItemEntity CreateNewItem()
        {
            if (null == rawResResponse)
            {
                return null;
            }

            ItemEntity result = null;
            var        go     = rawResResponse.Instantiate();
            if (null != go)
            {
                go.transform.position  = HKCommonDefine.OUT_SPACE;
                result                 = go.gameObject.AddComponent<ItemEntity>();
                result.ParentGroupName = rawObjectName; // 设置Group节点
            }
            else
            {
                this.Error("raw response Instantiate 创建失败");
            }

            return result;
        }


        /// <summary>
        /// 回收
        /// </summary>
        /// <param name="_t"></param>
        public void Recover(GameObject _t)
        {
            if (null == _t)
            {
                this.Warr("回收的对象为空，放弃回收");
                return;
            }

            ItemEntity poolEntity = _t.GetComponent<ItemEntity>();
            if (null == poolEntity)
            {
                this.Error("pool entity组件为空，无法回收");
                return;
            }

            if (FreePools.Count < MaxCount)
            {
                poolEntity.PoolRecoverEvent(); // 回收之前执行的事件
                // 执行回收事件
                OnRecoverAction(poolEntity);

                // push到Free队列中
                FreePools.Push(poolEntity);
            }
            else
            {
                poolEntity.IsDestroyTag = true; // 做一个标记，判断是否是非法销毁
                if (null != rawResResponse)
                {
                    rawResResponse.Destroy(_t);
                }
            }

            // 从使用队列中移除
            UsePools.Remove(poolEntity);
        }

        /// <summary>
        /// 销毁
        /// </summary>
        public void Dispose()
        {
            Clear(); // 清空空闲队列

            // 停止过期检测定时器
            if (null != intervalTimerHandler)
            {
                App.Make<ITimerManager>().Cancel(intervalTimerHandler.Queue);
            }

            Object.Destroy(groupNodeTransform.gameObject); // 删除当前节点对象

            isRunning = false;
        }


        /// <summary>
        /// 清空缓存池
        /// </summary>
        /// <param name="_force"></param>
        public void Clear(bool _force = false)
        {
            // 销毁空闲队列
            if (FreePools.Count > 0)
            {
                foreach (var freeItem in FreePools)
                {
                    var poolItem = freeItem.GetComponent<ItemEntity>();
                    poolItem.PoolDisposeEvent();
                    poolItem.IsDestroyTag = true; // 标记为将要释放掉
                    rawResResponse.Destroy(poolItem.gameObject);
                }

                FreePools.Clear();
            }

            // 销毁使用队列
            if (UsePools.Count > 0)
            {
                foreach (var useItem in UsePools)
                {
                    var poolItem = useItem.GetComponent<ItemEntity>();
                    poolItem.PoolDisposeEvent();
                    poolItem.IsDestroyTag = true; // 标记为将要释放掉
                    rawResResponse.Destroy(poolItem.gameObject);
                }

                this.Warr("使用队列不为空，强制清空  " + rawObjectName + "  count = " + UsePools.Count);
            }
        }

        /// <summary>
        /// 轮询时要处理的事件
        /// </summary>
        public void IntervalEvent()
        {
            // 统计已经过期的对象
            expireItems.Clear();

            for (int index = 0; index < FreePools.Count; index++)
            {
                var poolEntity = FreePools[index];
                if (null == poolEntity)
                {
                    continue;
                }

                if (Time.time - poolEntity.LastRecoverTime > expiredTime) // 已经过期
                {
                    expireItems.Add(poolEntity);
                }
            }

            // 清理已经过期的对象，并从队列中删除
            if (expireItems.Count > 0)
            {
                for (int index = 0; index < expireItems.Count; index++)
                {
                    var expireItem = expireItems[index];
                    expireItem.PoolDisposeEvent(); // 发送Dispose事件
                    expireItem.IsDestroyTag = true;
                    FreePools.Remove(expireItem);                  // 从队列中移除
                    rawResResponse.Destroy(expireItem.gameObject); // 销毁
                }

                expireItems.Clear();
            }
        }

        /// <summary>
        /// 回收
        /// </summary>
        /// <param name="_poolEntity"></param>
        private void OnRecoverAction(ItemEntity _poolEntity)
        {
            if (null == _poolEntity)
            {
                return;
            }

//            this.Debug("group node transform = " + groupNodeTransform);
            // 恢复到默认节点下
            if (null != groupNodeTransform)
            {
                _poolEntity.cacheTransform.parent = groupNodeTransform;
            }

            // 修改回收后的属性
            if (recoverType == RecoverEventType.Deactive)
            {
                _poolEntity.gameObject.SetActive(false);
            }
            else if (recoverType == RecoverEventType.MovePos)
            {
                _poolEntity.cacheTransform.localPosition = HKCommonDefine.OUT_SPACE;
            }

            _poolEntity.cacheTransform.localEulerAngles = Vector3.zero;

            // 修改为在Hierarchy中不显示,会自定义相关界面
//            _go.hideFlags = HideFlags.HideInHierarchy;

            // 如果有过期事件,则记录一个日期
            _poolEntity.LastRecoverTime = Time.time;
        }
    }
}