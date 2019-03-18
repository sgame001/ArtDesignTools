// // ================================================================
// // FileName:HKSpriteManager.cs
// // User: Baron-Fisher
// // CreateTime:2018 0219 13:50
// // Description:
// // Copyright (c) 2016 Greg.Co.Ltd. All rights reserved.
// // ================================================================

using System;
using System.Collections;
using HKLibrary;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GameCoreLib
{
    public class SceneMgr : HKSingletonMonoBehaviour<SceneMgr>
    {
        /// <summary>
        /// logger
        /// </summary>
        private static Type logger = typeof(SceneMgr);

        /// <summary>
        /// 场景进入
        /// </summary>
        /// <param name="_sceneName"></param>
        /// <param name="_params"></param>
        public delegate void SceneEneter(string _sceneName, object _params);

        /// <summary>
        /// 场景离开
        /// </summary>
        /// <param name="_sceneName"></param>
        public delegate void SceneExit(string _sceneName);

        /// <summary>
        /// 异步加载进度
        /// </summary>
        private static AsyncOperation asyncOperation;

        /// <summary>
        /// 当前真实的进度
        /// </summary>
        private float currentRealProgress = 0;

        /// <summary>
        /// 当前模拟的进度
        /// </summary>
        private float currentSimlateProgress = 0;

        /// <summary>
        /// 异步加载回调
        /// </summary>
        private static Action<float> asyncLoadCallback = null;

        /// <summary>
        /// scene enter
        /// </summary>
        private static SceneEneter sceneEnter = null;

        /// <summary>
        /// 当前加载的场景名称
        /// </summary>
        private static string currentSceneName = null;

        /// <summary>
        /// 附加信息
        /// </summary>
        private static object userObject = null;

        /// <summary>
        /// 同步加载一个场景
        /// </summary>
        /// <param name="_sceneName">场景名称</param>
        /// <param name="_userObject">user object</param>
        /// <param name="_sceneEnter">场景进入的委托回调</param>
        public void LoadScene(string _sceneName, object _userObject, SceneEneter _sceneEnter)
        {
            if (true == string.IsNullOrEmpty(_sceneName))
            {
                return;
            }

            currentSceneName = _sceneName;
            userObject       = _userObject;
            SceneManager.LoadScene(_sceneName);
            if (null != _sceneEnter)
            {
                _sceneEnter(_sceneName, userObject);
            }
        }


        /// <summary>
        /// 异步加载场景
        /// </summary>
        /// <param name="_sceneName">名字</param>
        /// <param name="_userObject"></param>
        /// <param name="_loadCallback">加载回调</param>
        /// <param name="_sceneEnter"></param>
        public void LoadSceneAsync(string _sceneName, object _userObject, Action<float> _loadCallback, SceneEneter _sceneEnter)
        {
            if (true == string.IsNullOrEmpty(_sceneName))
            {
                return;
            }

            currentSceneName  = _sceneName;
            userObject        = _userObject;
            asyncOperation    = SceneManager.LoadSceneAsync(_sceneName); // 开始异步加载
            asyncLoadCallback = _loadCallback;
            sceneEnter        = _sceneEnter;
            currentSimlateProgress = 0;
            
            if (null != asyncLoadCallback)
            {
                asyncLoadCallback(0); // 默认0
            }
        }


        /// <summary>
        /// update
        /// </summary>
        private void Update()
        {
            if (null == asyncOperation)
            {
                return;
            }

            currentRealProgress = asyncOperation.progress;
            if (currentSimlateProgress < currentRealProgress)
            {
                currentSimlateProgress = currentRealProgress; // += 0.05f;
                if (null != asyncLoadCallback)
                {
                    if (currentSimlateProgress >= 1)
                    {
                        if (null != sceneEnter)
                        {
                            asyncLoadCallback(1);
                            sceneEnter(currentSceneName, userObject);
                            userObject = null;
                            sceneEnter = null;
                        }

                        asyncOperation    = null;
                        asyncLoadCallback = null;
                    }
                    else
                    {
                        asyncLoadCallback(currentSimlateProgress);
                    }
                }
            }
        }
    }
}