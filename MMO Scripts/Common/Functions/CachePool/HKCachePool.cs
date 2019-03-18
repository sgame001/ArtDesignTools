// // // ================================================================
// // // FileName:HKCachePool.cs
// // // User: Baron
// // // CreateTime:2017-09-09-17:27
// // // Description:缓存池对象
// // // 
// // // Copyright (c) 2016 Heeking.Co.Ltd. All rights reserved.
// // // ================================================================

using System.Collections.Generic;

namespace HKLibrary
{
    public class HKCachePool<T> : ICachePool<T> where T : class, ICanbeCache, new()
    {
        /// <summary>
        /// 缓存列表
        /// </summary>
        private readonly Stack<T> stackPool = new Stack<T>();


        /// <summary>
        /// 最大数量
        /// </summary>
        private readonly int maxCount;

        public int MaxCount
        {
            get { return maxCount; }
        }

        /// <summary>
        /// 带有最大数量的构造
        /// </summary>
        /// <param name="_maxCount"></param>
        public HKCachePool(int _maxCount)
        {
            maxCount = _maxCount;
        }

        /// <summary>
        /// 获取当前cache item数量
        /// </summary>
        /// <returns></returns>
        public int GetCacheCount()
        {
            return stackPool.Count;
        }

        /// <summary>
        /// 获取空闲的
        /// </summary>
        /// <returns></returns>
        public T GetFree()
        {
            if (stackPool.Count > 0)
            {
                return stackPool.Pop();
            }

            var t = new T();
            t.Init();
            return t;
        }

        /// <summary>
        /// 回收
        /// </summary>
        /// <param name="_t"></param>
        public void Recover(T _t)
        {
            if (stackPool.Count < MaxCount)
            {
                if (null != _t && false == stackPool.Contains(_t))
                {
                    _t.Reset();
                    stackPool.Push(_t);
                }
            }
            else
            {
                _t = null;
            }
        }

        /// <summary>
        /// 清理空闲的缓存
        /// </summary>
        public void CleraFree()
        {
            stackPool.Clear();
        }
    }
}