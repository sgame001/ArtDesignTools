// // ================================================================
// // FileName:HKStringProvider.cs
// // User: Baron-Fisher
// // CreateTime:2017 0807 18:20
// // Description: 字符串缓存的工厂类
// // Copyright (c) 2017 Greg.Co.Ltd. All rights reserved.
// // ================================================================

using System.Collections.Generic;

namespace HKLibrary
{
    public class StringCacheFactory
    {
        /// <summary>
        /// 限制缓存目录
        /// </summary>
        private static Stack<StringListCacche> CacheStack = new Stack<StringListCacche>();


        /// <summary>
        /// 获取一个限制字符串缓存器
        /// </summary>
        /// <returns></returns>
        public static StringListCacche GetFree()
        {
            if (CacheStack.Count > 0)
            {
                StringListCacche cache = CacheStack.Pop();
                return cache;
            }
            return new StringListCacche();
        }


        /// <summary>
        /// 回收字符串缓存
        /// </summary>
        /// <param name="_cache"></param>
        public static void Recover(StringListCacche _cache)
        {
            if (null != _cache)
            {
                _cache.Clear();
            }
            CacheStack.Push(_cache);
        }
    }
}