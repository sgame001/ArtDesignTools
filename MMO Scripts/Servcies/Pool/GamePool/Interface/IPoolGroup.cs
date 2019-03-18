// // ================================================================
// // FileName:HKSpriteManager.cs
// // User: Baron-Fisher
// // CreateTime:2017 0910 19:23
// // Description:Pool Group 内存池的基本单元
// // Copyright (c) 2016 Greg.Co.Ltd. All rights reserved.
// // ================================================================

using HKLibrary;
using UnityEngine;

public interface IPoolGroup
{
    
    /// <summary>
    /// 原始资源名称
    /// </summary>
    string RawDataName { get; set; }
    
    /// <summary>
    /// 回收类型
    /// </summary>
    RecoverEventType RecoverType { get; }

    IPoolGroup SetRecoverType(RecoverEventType _value);


    /// <summary>
    /// 预加载的数量
    /// 创建Pool的时候，就会预加载进来的数量
    /// 一般用在切换场景时，同步加载怪物，预加载一些已知的内容
    /// </summary>
    int PreloadNumber { get; }

    IPoolGroup SetPreloadNumber(int _value);

    /// <summary>
    /// 最大数量
    /// 回收时，如果超过最大数量，则直接销毁，不回收到池子中
    /// 在缓存和内存之间做一个权衡
    /// </summary>
    int MaxCount { get; }

    IPoolGroup SetMaxCount(int _value);


    /// <summary>
    /// 过期时间
    /// </summary>
    int ExpiredTime { get; }

    IPoolGroup SetExpiredTime(int _value);


    /// <summary>
    /// 检测频率
    /// 间隔这么长时间后，检测一次内存中可以可以释放的对象
    /// </summary>
    int CleraIntervalTime { get; }

    IPoolGroup SetCleraIntervalTime(int _value);


    /// <summary>
    /// init
    /// </summary>
    void Init();

    /// <summary>
    /// 启动
    /// 启动后会创建GameObject Node以及Preload Number
    /// </summary>
    void Run();

    /// <summary>
    /// 获取一个空闲的对象
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
   GameObject GetFree();

    /// <summary>
    /// 回收一个对象
    /// </summary>
    /// <param name="_t"></param>
    void Recover(GameObject _t);

    /// <summary>
    /// 销毁
    /// </summary>
    void Dispose();

    /// <summary>
    /// 清理所有的缓存对象
    /// </summary>
    /// <param name="_force">如果强制清除，则当前正在使用的也会被清除，慎用</param>
    void Clear(bool _force = false);

    /// <summary>
    /// 轮询时要处理的事件
    /// </summary>
    void IntervalEvent();
}