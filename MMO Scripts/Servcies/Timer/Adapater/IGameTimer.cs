using System;

namespace HKLibrary
{
    public interface IGameTimer : ICanbeCache
    {
        /// <summary>
        /// 启动事件
        /// </summary>
        /// <returns></returns>
        IGameTimer Send();

        /// <summary>
        /// 终止事件
        /// </summary>
        void Stop();

        /// <summary>
        /// 暂停事件
        /// </summary>
        void Pause();

        /// <summary>
        /// 设置延迟
        /// </summary>
        /// <param name="_delay"></param>
        IGameTimer SetDelay(float _delay);

        /// <summary>
        /// 设置延迟固定帧数
        /// </summary>
        /// <param name="_frame"></param>
        /// <returns></returns>
        IGameTimer SetDelayFrame(int _frame);
        
        /// <summary>
        /// 执行开始事件
        /// </summary>
        /// <param name="_startEvent"></param>
        /// <returns></returns>
        IGameTimer SetStartEvent(Action<object> _startEvent);

        /// <summary>
        /// 循环事件
        /// </summary>
        /// <param name="_repeatCount">循环次数</param>
        /// <param name="_interval">间隔时间</param>
        /// <param name="_intervalEvent">间隔事件</param>
        /// <returns></returns>
        IGameTimer SetRepeat(int _repeatCount, float _interval, Action<int, object> _intervalEvent);

        /// <summary>
        /// 设置自定义参数
        /// </summary>
        /// <param name="_userData"></param>
        /// <returns></returns>
        IGameTimer SetArgsData(object _userData);

        /// <summary>
        /// 设置结束事件
        /// </summary>
        /// <param name="_endEvent"></param>
        /// <returns></returns>
        IGameTimer SetEndEvent(Action<object> _endEvent);


        /// <summary>
        /// 每帧都执行事件,循环固定时间
        /// </summary>
        /// <param name="_duration"></param>
        /// <param name="_loopEvent"></param>
        /// <returns></returns>
        IGameTimer SetLooping(float _duration, Action<object> _loopEvent);
    }
}