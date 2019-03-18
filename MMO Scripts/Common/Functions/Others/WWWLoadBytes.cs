// // ================================================================
// // FileName:HKSpriteManager.cs
// // User: Baron-Fisher
// // CreateTime:2017 0930 22:51
// // Description:工具类，继承Monobehaviour
// // Copyright (c) 2016 Greg.Co.Ltd. All rights reserved.
// // ================================================================

using System;
using System.Collections;
using Mono;
using UnityEngine;

namespace HKLibrary
{
    public class WWWLoadBytes : HKSingletonMonoBehaviour<WWWLoadBytes>
    {
        /// <summary>
        /// 加载字节
        /// </summary>
        /// <param name="_filePath"></param>
        /// <param name="_completeCallback"></param>
        public void LoadBytes(string _filePath, Action<string, byte[], string> _completeCallback)
        {
            StartCoroutine(LoadBytesCo(_filePath, _completeCallback));
        }


        /// <summary>
        /// 用WWW的形式读取一个二进制文件
        /// </summary>
        /// <param name="_fileUri"></param>
        /// <param name="_completeCallback"></param>
        /// <returns></returns>
        private IEnumerator LoadBytesCo(string _fileUri, Action<string, byte[], string> _completeCallback)
        {
            WWW www = new WWW(_fileUri);
            yield return www;

            byte[] bytes = www.bytes;
            string errorMessage = www.error;
            www.Dispose();

            if (_completeCallback != null)
            {
                _completeCallback(_fileUri, bytes, errorMessage);
            }
        }
    }
}