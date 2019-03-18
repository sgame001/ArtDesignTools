// // ================================================================
// // FileName:DeviceInfo.cs
// // User: Baron
// // CreateTime:2018/5/26
// // Description: 设备相关信息
// // ================================================================

using UnityEngine;

namespace GameCoreLib
{
    public class DeviceInfo
    {
        /// <summary>
        /// 获取内存
        /// 直接返回G的单位
        /// </summary>
        /// <returns></returns>
        public static int GetMemorySize()
        {
            return SystemInfo.systemMemorySize / 1000;  // 这里之所以没有除1024，是因为不同系统上，可能换算单位不一致，采用1000更精准
        }
    }
}