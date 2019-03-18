// // // ================================================================
// // // FileName:HKCommonDefine.cs
// // // User: Baron
// // // CreateTime:2017-09-08-18:44
// // // Description:通用定义
// // // 
// // // Copyright (c) 2016 Heeking.Co.Ltd. All rights reserved.
// // // ================================================================

using UnityEngine;

namespace HKLibrary
{
    public class HKCommonDefine
    {
        /// <summary>
        /// 设备性能
        /// </summary>
        public enum DevicePerformance
        {
            LOW    = 0, // 高
            MIDDLE = 1, // 中
            HIGHT  = 2  // 低
        }

        /// <summary>
        /// 本地设置中，性能key
        /// </summary>
        public const string KEY_PERFORMANCE_LEVEL = "performance_level";

        /// <summary>
        /// 切换质量
        /// </summary>
        public const string EVENT_PERFORMANCE_SWITH = "performance_swith";

        /// <summary>
        /// 屏幕外的一个坐标
        /// </summary>
        public static Vector3 OUT_SPACE = new Vector3(-9999, -9999, -9999);

        /// <summary>
        /// 无效空间位置
        /// </summary>
        public static Vector3 IN_VAILD_VECTOR3 = new Vector3(-999, -999, -999);

        /// <summary>
        /// 加载资源
        /// </summary>
        /// <param name="_name"></param>
        /// <param name="_bundleName"></param>
        /// <typeparam name="T"></typeparam>
        public delegate Object LoadRes(string _name, string _bundleName);

        /// <summary>
        /// 应用程序是否退出
        /// </summary>
        public static bool IsApplicationQuit = false;

        /// <summary>
        /// 是否是生产环境
        /// </summary>
        public static bool IsMobileDevice
        {
            get { return Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer; }
        }
    }
}