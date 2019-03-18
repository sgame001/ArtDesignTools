// // ================================================================
// // FileName:IFraming.cs
// // User: Baron-Fisher
// // CreateTime:2018 0525 21:08
// // Description:
// // Copyright (c) 2017 Greg.Co.Ltd. All rights reserved.
// // ================================================================

using System;
using System.Collections.Generic;

namespace HKLibrary
{
    public interface IFraming
    {

        /// <summary>
        /// init
        /// </summary>
        void Init();
        
        /// <summary>
        /// 分帧执行
        /// </summary>
        /// <param name="_count">执行的总次数</param>
        /// <param name="_executeCountPerFrame">平分多少帧内完成</param>
        /// <param name="_perAction">每次执行逻辑的回调</param>
        /// <param name="_complete">完成事件</param>
        FramingHandler FramingExecute(int _count, int _executeCountPerFrame, Action<int> _perAction, Action _complete = null);


        /// <summary>
        /// 分帧执行
        /// </summary>
        /// <param name="_count"></param>
        /// <param name="_needFrameCount"></param>
        /// <param name="_perAction"></param>
        /// <param name="_complete"></param>
        /// <returns></returns>
        FramingHandler FramingExecuteByFrameCount(int _count, int _needFrameCount, Action<int> _perAction, Action _complete = null);

        
        /// <summary>
        /// 每帧执行，泛型版本
        /// </summary>
        /// <param name="_list"></param>
        /// <param name="_executeCountPerFrame"></param>
        /// <param name="_perAction"></param>
        /// <param name="_complete"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        FramingHandler FramingExecuteArgs<T>(List<T> _list, int _executeCountPerFrame, Action<int, T> _perAction, Action _complete = null);

        
        /// <summary>
        /// 分帧执行 泛型版本（可以指定在多少帧内完成）
        /// </summary>
        /// <param name="_list"></param>
        /// <param name="_needFrameCount"></param>
        /// <param name="_perAction"></param>
        /// <param name="_complete"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        FramingHandler FramingExecuteByFrameCountArgs<T>(List<T> _list, int _needFrameCount, Action<int, T> _perAction, Action _complete = null);

        
        /// <summary>
        /// 分帧执行（可以指定在多少秒内完成）
        /// </summary>
        /// <param name="_count"></param>
        /// <param name="_duractionTime"></param>
        /// <param name="_perAction"></param>
        /// <param name="_complete"></param>
        /// <returns></returns>
        FramingHandler FramingExecuteByTime(int _count, float _duractionTime, Action<int> _perAction, Action _complete = null);

        /// <summary>
        /// 分帧执行（可以指定在多少时间内完成，泛型版本）
        /// </summary>
        /// <param name="_list"></param>
        /// <param name="_durationTime"></param>
        /// <param name="_perAction"></param>
        /// <param name="_complete"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        FramingHandler FramingExecuteByTimeArgs<T>(List<T> _list, float _durationTime, Action<int, T> _perAction, Action _complete = null);


    }
}