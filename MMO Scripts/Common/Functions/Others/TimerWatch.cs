using System;
using System.Diagnostics;
using UnityEngine.Profiling;

namespace HKLibrary
{
    [Flags]
    public enum TimerProfileType
    {
        TimerWatch = 1 << 1, // 使用StopWatcher
        Profile    = 1 << 2  // 使用Profile
    }

    public class TimerWatch : IDisposable
    {
        private Stopwatch sp = null;
        
        private string key;

        private TimerProfileType Type;
        
        public TimerWatch(string _key, TimerProfileType _timerProfileType)
        {
            key = _key;
            Type = _timerProfileType;
            
            if ( (Type & TimerProfileType.TimerWatch) != 0)
            {
                sp = new Stopwatch();
                sp.Start();
            }

            if ((Type & TimerProfileType.Profile) != 0)
            {
                Profiler.BeginSample(_key);
            }
        }

        public void Dispose()
        {
            if (null != sp)
            {
                sp.Stop();
                this.Warr(string.Format("{0} use time = {1}ms", key, sp.ElapsedMilliseconds));
            }

            if ((Type & TimerProfileType.Profile) != 0)
            {
                Profiler.EndSample();
            }
  
        }
    }
}