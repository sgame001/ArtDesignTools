// // // ================================================================
// // // FileName:ICachePool.cs
// // // User: Baron
// // // CreateTime:2017-09-09-17:29
// // // Description: 缓存池结构接口
// // // 
// // // Copyright (c) 2016 Heeking.Co.Ltd. All rights reserved.
// // // ================================================================

using System.Security.Policy;

namespace HKLibrary
{
    public interface ICachePool<T>
    {
        /// <summary>
        /// 最大数量
        /// 超过最大数量后,将不会再回收到内存池中,则直接释放掉
        /// </summary>
        int MaxCount { get;}

        /// <summary>
        /// 获取缓存中的数量
        /// </summary>
        /// <returns></returns>
        int GetCacheCount();
        
        /// <summary>
        /// 获取一个空闲的
        /// </summary>
        /// <returns></returns>
        T GetFree();

        /// <summary>
        /// 回收
        /// </summary>
        /// <param name="_t"></param>
        void Recover(T _t);

        /// <summary>
        /// 清空所有空闲数据
        /// </summary>
        void CleraFree();
    }
}