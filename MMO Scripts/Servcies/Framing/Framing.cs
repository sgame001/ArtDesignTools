// // ================================================================
// // FileName:Framing.cs
// // User: Baron-Fisher
// // CreateTime:2018 0525 21:21
// // Description:
// // Copyright (c) 2017 Greg.Co.Ltd. All rights reserved.
// // ================================================================

using System;
using System.Collections.Generic;
using CatLib;
using CatLib.API.Timer;

namespace HKLibrary
{
    public class Framing : IFraming
    {
        /// <summary>
        /// timer manager
        /// </summary>
        private ITimerManager timerManager;


        public void Init()
        {
            timerManager = App.Make<ITimerManager>();
        }

        /// <summary>
        /// 分帧执行事件（可以指定每帧执行的逻辑次数）
        /// </summary>
        /// <param name="_count">总数量</param>
        /// <param name="_executeCountPerFrame">每帧执行的数量</param>
        /// <param name="_perAction">每次逻辑执行的回调（不是每帧，每帧会回调frame count次）</param>
        /// <param name="_complete">完成事件</param>
        public FramingHandler FramingExecute(int _count, int _executeCountPerFrame, Action<int> _perAction, Action _complete = null)
        {
            if (_count <= 0 || _executeCountPerFrame <= 0)
            {
                if (null != _complete)
                {
                    _complete();
                }

                return null;
            }

            var loopFrameCount = _count / _executeCountPerFrame + 1;

            FramingHandler handler = new FramingHandler();

            int executeIndex = 0;

            handler.PerTimerHandler = timerManager.Make(() =>
            {
                for (int index = 0; index < _executeCountPerFrame; index++)
                {
                    executeIndex++;
                    if (executeIndex < _count)
                    {
                        if (null != _perAction)
                        {
                            _perAction(executeIndex);
                        }
                    }
                }
            }).LoopFrame(loopFrameCount);

            handler.CompleteHandler = timerManager.Make(() =>
            {
                if (null != _complete)
                {
                    _complete();
                }

                handler.Clear();
            }).DelayFrame(loopFrameCount + 1); // 这里结束多延迟一帧，防止和最后一个逻辑帧冲突
            return handler;
        }


        /// <summary>
        /// 分帧执行一系列事件（可以指定在多少帧内完成）
        /// </summary>
        /// <param name="_count">执行的总数量</param>
        /// <param name="_needFrameCount">需要执行的帧数</param>
        /// <param name="_perAction">每次执行的回调</param>
        /// <param name="_complete">完成的回调</param>
        /// <returns></returns>
        public FramingHandler FramingExecuteByFrameCount(int _count, int _needFrameCount, Action<int> _perAction, Action _complete = null)
        {
            var executeFramePerCount = _count / _needFrameCount;
            executeFramePerCount = executeFramePerCount < 0 ? 1 : executeFramePerCount; // 保证每帧最少一次执行
            return FramingExecute(_count, executeFramePerCount, _perAction, _complete);
        }

        /// <summary>
        /// 分帧泛型参数执行
        /// </summary>
        /// <param name="_list"></param>
        /// <param name="_executeCountPerFrame"></param>
        /// <param name="_perAction"></param>
        /// <param name="_complete"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public FramingHandler FramingExecuteArgs<T>(List<T> _list, int _executeCountPerFrame, Action<int, T> _perAction, Action _complete = null)
        {
            if (null == _list || _list.Count <= 0)
            {
                if (null != _complete)
                {
                    _complete();
                }
                return null;
            }
            
            var loopFrameCount = _list.Count / _executeCountPerFrame + 1;

            FramingHandler handler = new FramingHandler();

            int executeIndex = 0;

            handler.PerTimerHandler = timerManager.Make(() =>
            {
                for (int index = 0; index < _executeCountPerFrame; index++)
                {
                    if (executeIndex <  _list.Count)
                    {
                        var model = _list[executeIndex];
                        if (null != _perAction)
                        {
                            _perAction(executeIndex, model);
                        }
                    }
                    executeIndex++;

                }
            }).LoopFrame(loopFrameCount);

            handler.CompleteHandler = timerManager.Make(() =>
            {
                if (null != _complete)
                {
                    _complete();
                }

                handler.Clear();
            }).DelayFrame(loopFrameCount + 1); // 这里结束多延迟一帧，防止和最后一个逻辑帧冲突
            return handler;
        }

        /// <summary>
        /// 泛型版本，可以指定在多少帧内完成
        /// </summary>
        /// <param name="_list"></param>
        /// <param name="_needFrameCount"></param>
        /// <param name="_perAction"></param>
        /// <param name="_complete"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public FramingHandler FramingExecuteByFrameCountArgs<T>(List<T> _list, int _needFrameCount, Action<int, T> _perAction, Action _complete = null)
        {
            if (null == _list)
            {
                return null;
            }
            var executeFramePerCount = _list.Count / _needFrameCount;
            executeFramePerCount = executeFramePerCount < 0 ? 1 : executeFramePerCount; // 保证每帧最少一次执行
            return FramingExecuteArgs(_list, executeFramePerCount, _perAction, _complete);
        }

        /// <summary>
        /// 分帧执行，可以指定在多少s内完成
        /// </summary>
        /// <param name="_count"></param>
        /// <param name="_duractionTime"></param>
        /// <param name="_perAction"></param>
        /// <param name="_complete"></param>
        /// <returns></returns>
        public FramingHandler FramingExecuteByTime(int _count, float _duractionTime, Action<int> _perAction, Action _complete = null)
        {
            if (_count <= 0 || _duractionTime <= 0)
            {
                this.Error("参数错误 count or duration 必须大于0");
                return null;
            }
            
            // 先得到每s需要执行多少个单位
            var perSecondCount = (int) (_count / _duractionTime);

            // 计算每帧需要执行多少个单位
            var executeFrameCount = (int)(perSecondCount / UnityEngine.Application.targetFrameRate) + 1; // + 1 是为了结果为0或者是小数进位的情况

            return FramingExecute(_count, executeFrameCount, _perAction, _complete);

        }

        /// <summary>
        /// 分帧执行（可以指定在单位时间内完成，泛型版本）
        /// </summary>
        /// <param name="_list"></param>
        /// <param name="_durationTime"></param>
        /// <param name="_perAction"></param>
        /// <param name="_complete"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public FramingHandler FramingExecuteByTimeArgs<T>(List<T> _list, float _durationTime, Action<int, T> _perAction, Action _complete = null)
        {
            if (null == _list || _list.Count <= 0 || _durationTime <= 0)
            {
                this.Error("参数错误 list or duration 必须大于0");
                return null;
            }
            
            // 先得到每s需要执行多少个单位
            var perSecondCount = (int)(_list.Count / _durationTime);

            // 计算每帧需要执行多少个单位
            var executeFrameCount = (int)(perSecondCount / UnityEngine.Application.targetFrameRate) + 1; // + 1 是为了结果为0或者是小数进位的情况

            return FramingExecuteArgs(_list, executeFrameCount, _perAction, _complete);
        }
    }
}