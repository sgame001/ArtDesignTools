// // // ================================================================
// // // FileName:HKGameTimerManager.cs
// // // User: Baron
// // // CreateTime:2017-09-26-10:40
// // // Description:
// // // 
// // // Copyright (c) 2016 Heeking.Co.Ltd. All rights reserved.
// // // ================================================================

using System;

namespace HKLibrary
{
    public class HKGameTimerManager : IGameTimerManager
    {
        /// <summary>
        /// timer的缓存池
        /// </summary>
        private HKCachePool<HKTimer> cachePool = new HKCachePool<HKTimer>(int.MaxValue);

        /// <summary>
        /// 创建一个Timer
        /// 通过内存池处理,会有缓存机制
        /// </summary>
        /// <returns></returns>
        public IGameTimer CreateTimer()
        {
            var timer = cachePool.GetFree();
            return timer;
        }

        /// <summary>
        /// 创建一个延迟的timer
        /// 并且自动开始
        /// </summary>
        /// <param name="_delayTime"></param>
        /// <param name="_complete"></param>
        /// <returns></returns>
        public IGameTimer CreateTimer(float _delayTime, Action<object> _complete)
        {
            var timer = cachePool.GetFree();
            if (null != timer)
            {
                timer.SetDelay(_delayTime).SetEndEvent(_complete).Send();
            }
            return timer;
        }

        /// <summary>
        /// 延迟固定帧数
        /// </summary>
        /// <param name="_delayFrame"></param>
        /// <param name="_complete"></param>
        /// <returns></returns>
        public IGameTimer CreateTimerDelayFrame(int _delayFrame, Action<object> _complete)
        {
            var timer = cachePool.GetFree();
            if (null != timer)
            {
                timer.SetDelayFrame(_delayFrame).SetEndEvent(_complete).Send();
            }

            return timer;
        }

        /// <summary>
        /// 回收
        /// 在执行End事件后,会自动调用
        /// </summary>
        /// <param name="_timer"></param>
        public void Recover(IGameTimer _timer)
        {
            if (null != _timer)
            {
                cachePool.Recover((HKTimer) _timer);
            }
        }
    }
}