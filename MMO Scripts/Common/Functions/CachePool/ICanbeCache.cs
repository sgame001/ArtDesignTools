// // // ================================================================
// // // FileName:ICanbeCache.cs
// // // User: Baron
// // // CreateTime:2017-09-09-17:26
// // // Description:可以被缓存,重复利用的接口
// // // 
// // // Copyright (c) 2016 Heeking.Co.Ltd. All rights reserved.
// // // ================================================================
namespace HKLibrary
{
    public interface ICanbeCache
    {
        /// <summary>
        /// 初始化
        /// </summary>
        void Init();
        
        /// <summary>
        /// reset回收
        /// 这里处理内容重置
        /// </summary>
        void Reset();
    }
}