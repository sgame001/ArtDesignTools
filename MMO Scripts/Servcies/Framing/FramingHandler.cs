// // ================================================================
// // FileName:IFramingHandler.cs
// // User: Baron-Fisher
// // CreateTime:2018 0525 21:29
// // Description:
// // Copyright (c) 2017 Greg.Co.Ltd. All rights reserved.
// // ================================================================

using CatLib;
using CatLib.API.Timer;

namespace HKLibrary
{
    public class FramingHandler
    {
        /// <summary>
        /// 每次执行的timer句柄
        /// </summary>
        public ITimer PerTimerHandler { get; set; }
        
        /// <summary>
        /// 完成的句柄
        /// </summary>
        public ITimer CompleteHandler { get; set; }

        public void Clear()
        {
            if (null != PerTimerHandler)
            {
                App.Make<ITimerManager>().Cancel(PerTimerHandler.Queue);
                PerTimerHandler = null;
            }

            if (null != CompleteHandler)
            {
                App.Make<ITimerManager>().Cancel(CompleteHandler.Queue);
                CompleteHandler = null;
            }
        }
    }
}