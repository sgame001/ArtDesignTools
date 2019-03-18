// // // ================================================================
// // // FileName:HKTimer.cs
// // // User: Baron
// // // CreateTime:2017-09-25-18:52
// // // Description:定时器使用 内部使用的是SuperInvoke,这个框架使用很长时间,表现稳定
// // // 
// // // Copyright (c) 2016 Heeking.Co.Ltd. All rights reserved.
// // // ================================================================

using System;
using CatLib;
using CatLib.API.Timer;
using JacobGames.SuperInvoke;

namespace HKLibrary
{
    public class HKTimer : IGameTimer
    {
        /// <summary>
        /// 延迟时间
        /// </summary>
        private float delayTime = 0;

        /// <summary>
        /// 延迟帧数
        /// </summary>
        private int delayFrame = 0;
        
        /// <summary>
        /// 结束事件
        /// 如果是单次延迟效果,则延迟后调用这个函数
        /// 如果是多次执行效果,则在所有的执行时间结束后调用
        /// </summary>
        private Action<object> endEvent = null;

        /// <summary>
        /// 开始事件
        /// </summary>
        private Action<object> startEvent = null;

        /// <summary>
        /// 间隔事件调用
        /// </summary>
        private Action<int, object> intervalEvent = null;

        /// <summary>
        /// 循环事件
        /// </summary>
        private Action<object> loopEvent = null;

        /// <summary>
        /// 参数数据
        /// </summary>
        private object userData = null;

        /// <summary>
        /// 事件句柄
        /// </summary>
        private SuperInvokeTag tag = null;

        /// <summary>
        /// 间隔时间
        /// </summary>
        private float intervalTime = 0;

        /// <summary>
        /// 循环次数
        /// </summary>
        private int repeatCount = 1;

        /// <summary>
        /// 当前的索引
        /// </summary>
        private int currentIndex = 0;

        /// <summary>
        /// 循环时间
        /// </summary>
        private float loopingDuration = 0;

        /// <summary>
        /// loop timer handler
        /// </summary>
        private ITimer loopTimerHandler = null;

        /// <summary>
        /// 单词帧延迟句柄
        /// </summary>
        private ITimer frameDelayTimerHandler = null;

        /// <summary>
        /// 启动定时器
        /// </summary>
        /// <returns></returns>
        public IGameTimer Send()
        {
            if (null == tag)
            {
                tag = SuperInvoke.CreateTag();
            }
            
            if (null != startEvent)
            {
                startEvent(userData);
            }

            if (loopingDuration > 0)
            {
                var timerManager = App.Make<ITimerManager>();
                if (null != timerManager)
                {
                    loopTimerHandler = timerManager.Make(() =>
                    {
                        if (null != loopEvent)
                        {
                            loopEvent(userData);
                        }
                    }).Loop(loopingDuration);
                    
                    // 执行延迟结束事件
                    SuperInvoke.Run(loopingDuration, tag, internalEndEvent);
                }
                return this;
            }


            if (repeatCount == 1) // 单次事件
            {
                if (delayFrame == 0) // 时间延迟
                {
                    SuperInvoke.Run(delayTime, tag, internalEndEvent);
                }
                else
                {
                    // 帧数延迟
                    frameDelayTimerHandler = App.Make<ITimerManager>().Make(() =>
                    {
                        if (null != endEvent)
                        {
                            endEvent(userData);
                        }
                    }).DelayFrame(delayFrame);
                }
            }
            else
            {
                // 多次事件
                SuperInvoke.RunRepeat(delayTime, intervalTime, repeatCount, tag, () =>
                {
                    intervalEvent(currentIndex, userData);
                    currentIndex++;
                });
                if (null != endEvent)
                {
                    SuperInvoke.Run(delayTime + repeatCount * intervalTime, tag, internalEndEvent);
                }
            }
            return this;
        }

        /// <summary>
        /// 停止计时器
        /// </summary>
        public void Stop()
        {
            if (null != tag)
            {
                SuperInvoke.Kill(tag);
            }

            if (null != loopTimerHandler)
            {
                App.Make<ITimerManager>().Cancel(loopTimerHandler.Queue);
            }

            if (null != frameDelayTimerHandler)
            {
                App.Make<ITimerManager>().Cancel(frameDelayTimerHandler.Queue);
            }
            
            //  回收到内存池中
            App.Make<IGameTimerManager>().Recover(this);
        }

        public void Pause()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 设置延迟时间
        /// </summary>
        /// <param name="_delay"></param>
        /// <returns></returns>
        public IGameTimer SetDelay(float _delay)
        {
            delayTime = _delay;
            return this;
        }

        /// <summary>
        /// 延迟固定帧数
        /// </summary>
        /// <param name="_frame"></param>
        /// <returns></returns>
        public IGameTimer SetDelayFrame(int _frame)
        {
            delayFrame = _frame;
            return this;
        }

        /// <summary>
        /// 开始事件
        /// </summary>
        /// <param name="_startEvent"></param>
        /// <returns></returns>
        public IGameTimer SetStartEvent(Action<object> _startEvent)
        {
            startEvent = _startEvent;
            return this;
        }

        /// <inheritdoc />
        /// <summary>
        /// 设置循环次数
        /// </summary>
        /// <param name="_repeatCount"></param>
        /// <param name="_interval"></param>
        /// <param name="_intervalEvent"></param>
        /// <returns></returns>
        public IGameTimer SetRepeat(int _repeatCount, float _interval, Action<int, object> _intervalEvent)
        {
            if (null == _intervalEvent)
            {
                return this;
            }

            repeatCount = _repeatCount;
            intervalTime = _interval;

            intervalEvent = _intervalEvent;

            return this;
        }

        /// <summary>
        /// 设置携带的参数
        /// </summary>
        /// <param name="_userData"></param>
        /// <returns></returns>
        public IGameTimer SetArgsData(object _userData)
        {
            userData = _userData;
            return this;
        }

        /// <summary>
        /// 设置结束事件
        /// </summary>
        /// <param name="_endEvent"></param>
        /// <returns></returns>
        public IGameTimer SetEndEvent(Action<object> _endEvent)
        {
            if (null != _endEvent)
            {
                endEvent = _endEvent;
            }
            return this;
        }


        /// <summary>
        /// Looping
        /// </summary>
        /// <param name="_duration"></param>
        /// <param name="_loopEvent"></param>
        /// <returns></returns>
        public IGameTimer SetLooping(float _duration, Action<object> _loopEvent)
        {
            loopingDuration = _duration;
            loopEvent = _loopEvent;
            return this;
        }


        /// <summary>
        /// 内部的結束事件调用
        /// </summary>
        private void internalEndEvent()
        {
            if (null != endEvent)
            {
                endEvent(userData);
            }

            // 回收到内存池中
            App.Make<IGameTimerManager>().Recover(this);
        }

        public void Init()
        {
        }

        /// <summary>
        /// reset
        /// </summary>
        public void Reset()
        {
            delayTime = 0;
            startEvent = null;
            intervalEvent = null;
            endEvent = null;
            userData = null;
            tag = null;
            intervalTime = 0;
            repeatCount = 1;
            currentIndex = 0;
            loopingDuration = 0;
            loopTimerHandler = null;
            frameDelayTimerHandler = null;
            delayFrame = 0;
        }
    }
}