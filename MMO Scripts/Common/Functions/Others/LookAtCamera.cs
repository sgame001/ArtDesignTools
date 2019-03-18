// // ================================================================
// // FileName:HKWorldRotationZero.cs
// // User: Baron-Fisher
// // CreateTime:2018/2/2
// // Description: 始终和Camera保持同样的角度
// // Copyright (c) 2018 Greg.Co.Ltd. All rights reserved.
// // ================================================================
using UnityEngine;
using System.Collections;

namespace HKLibrary
{
    public class LookAtCamera : MonoBehaviour
    {
        private Transform mainCameraTf;
        
        // Use this for initialization
        void Start()
        {
        }

        // Update is called once per frame
        void LateUpdate()
        {
            if (null == mainCameraTf)
            {
                var cameraGo = GameObject.FindWithTag("MainCamera");
                if (null != cameraGo)
                {
                    mainCameraTf = cameraGo.transform;
                }

            }
            if (null != mainCameraTf)
            {
                transform.rotation = mainCameraTf.rotation;
            }
        }
        
        public void OnEnable()
        {
            
        }
    }

}
