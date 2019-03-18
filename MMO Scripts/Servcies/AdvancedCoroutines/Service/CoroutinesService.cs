using System.Collections;
using AdvancedCoroutines;

namespace HKLibrary
{
    public class CoroutinesService : ICoroutinesService
    {
        /// <summary>
        /// 开始一个携程
        /// </summary>
        /// <param name="enumerator"></param>
        /// <returns></returns>
        public Routine Start(IEnumerator enumerator)
        {
            return CoroutineManager.StartStandaloneCoroutine(enumerator);

        }

        /// <summary>
        /// 停止一个携程
        /// </summary>
        /// <param name="_routine"></param>
        public void Stop(Routine _routine)
        {
            if (null != _routine)
            {
                CoroutineManager.StopCoroutine(_routine);
            }
        }
    }
}