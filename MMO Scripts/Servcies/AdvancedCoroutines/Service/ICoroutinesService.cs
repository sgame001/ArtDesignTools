using System.Collections;
using System.Collections.Generic;
using AdvancedCoroutines;
using CatLib;
using UnityEngine;

namespace HKLibrary
{
    public interface ICoroutinesService
    {
        /// <summary>
        /// 开始一个携程
        /// </summary>
        /// <param name="enumerator"></param>
        Routine Start(IEnumerator enumerator);

        /// <summary>
        /// 结束一个携程
        /// </summary>
        /// <param name="_routine"></param>
        void Stop(Routine _routine);

    }


}
