// // ================================================================
// // FileName:SceneRootNode.cs
// // User: Baron
// // CreateTime:2018/5/26
// // Description: scene root node
// // ================================================================

using UnityEngine;

namespace HKLibrary
{
    public class SceneRootNode
    {
        /// <summary>
        /// 场景的根节点
        /// Scene_C01_Root
        /// </summary>
        private Transform rooTransform;
        public Transform RootTransform
        {
            get { return rooTransform; }
        }

        /// <summary>
        /// 静态物体节点
        /// 归为到一个节点下，可以方便进行static mesh combo
        /// </summary>
        private Transform staticItemNode;

        public Transform StaticItemNode
        {
            get { return staticItemNode; }
        }

        /// <summary>
        /// 带有半透物体的节点
        /// 方便处理所有半透物体隐藏
        /// </summary>
        private Transform transparentNode;

        public Transform TransparentNode
        {
            get { return transparentNode; }
        }

        /// <summary>
        /// 特效节点
        /// </summary>
        private Transform fxNode;

        public Transform FxNode
        {
            get { return fxNode; }
        }

        /// <summary>
        /// 构造函数，创建多个节点
        /// </summary>
        /// <param name="_sceneName"></param>
        public SceneRootNode(string _sceneName)
        {
            if (true == string.IsNullOrEmpty(_sceneName))
            {
                _sceneName = "none";
            }

            // root
            rooTransform = new GameObject(string.Format("SCENE_{0}_ROOT", _sceneName)).transform;
            Object.DontDestroyOnLoad(RootTransform);
            
            // static node
            staticItemNode = new GameObject("STATIC_NODE").transform;
            staticItemNode.parent = rooTransform;
            
            // transparent
            transparentNode = new GameObject("TRANSPARENT_NODE").transform;
            transparentNode.parent = rooTransform;
            
            // fx
            fxNode = new GameObject("FX_NODE").transform;
            fxNode.parent = RootTransform;

        }


        public void SwithPerfromance(HKCommonDefine.DevicePerformance _devicePerformance)
        {
            switch (_devicePerformance)
            {
                    case HKCommonDefine.DevicePerformance.HIGHT:
                        staticItemNode.gameObject.SetActive(true);
                        transparentNode.gameObject.SetActive(true);
                        fxNode.gameObject.SetActive(true);
                        break;
                    case HKCommonDefine.DevicePerformance.MIDDLE:
                        staticItemNode.gameObject.SetActive(true);
                        transparentNode.gameObject.SetActive(true);
                        fxNode.gameObject.SetActive(false);
                        break;
                    case HKCommonDefine.DevicePerformance.LOW:
                        staticItemNode.gameObject.SetActive(true);
                        transparentNode.gameObject.SetActive(false);
                        fxNode.gameObject.SetActive(false);
                        break;
            }
        }
        

        /// <summary>
        /// 释放
        /// </summary>
        public void Dispose()
        {
            Object.Destroy(rooTransform.gameObject);
        }
        

    }
}