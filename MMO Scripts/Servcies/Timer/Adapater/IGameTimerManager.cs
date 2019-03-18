// // // ================================================================
// // // FileName:IGameTimerManager.cs
// // // User: Baron
// // // CreateTime:2017-09-26-10:39
// // // Description:计时器管理器的消息接口
// // // 
// // // Copyright (c) 2016 Heeking.Co.Ltd. All rights reserved.
// // // ================================================================

using System;

namespace HKLibrary
{
    public interface IGameTimerManager
    {
        /// <summary>
        /// 创建一个Timer
        /// </summary>
        /// <returns></returns>
        IGameTimer CreateTimer();

        /// <summary>
        /// 快速执行一个延迟事件的构造
        /// </summary>
        /// <param name="_delayTime"></param>
        /// <param name="_complete"></param>
        /// <returns></returns>
        IGameTimer CreateTimer(float _delayTime, Action<object> _complete);

        /// <summary>
        /// 延迟固定帧数
        /// </summary>
        /// <param name="_delayFrame"></param>
        /// <param name="_complete"></param>
        /// <returns></returns>
        IGameTimer CreateTimerDelayFrame(int _delayFrame, Action<object> _complete);
        
        /// <summary>
        /// 回收一个Timer
        /// </summary>
        /// <param name="_timer"></param>
        void Recover(IGameTimer _timer);
    }
}