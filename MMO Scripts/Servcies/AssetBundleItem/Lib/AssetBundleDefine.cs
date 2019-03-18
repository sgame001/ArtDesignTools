// // ================================================================
// // FileName:AssetBundleDefine.cs
// // User: Baron
// // CreateTime:1/29/2018
// // Description: AssetBundle的定义
// // ================================================================

using System.CodeDom;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class AssetBundleDefine
{
    /// <summary>
    /// 下载路径
    /// </summary>
    public static string SERVER_RES_PATH = "http://115.159.147.94:8086/datelog/Zren/AssetBundlesTarget";

    /// <summary>
    /// 版本文件名称
    /// </summary>
    public static string VERSION_FILE = "version.bytes";

    
    /// <summary>
    /// 只读文件夹名称
    /// </summary>
    public static string READ_ONLY_DIR = "Packaged";
    
    
    /// <summary>
    /// 本地文件列表
    /// </summary>
    public static string VERSION_LOCAL_FILE = "local_version.bytes";
    
    /// <summary>
    /// manifest bytes
    /// </summary>
    public static string MANIFEST_FILE = "manifest.bytes";
    
    /// <summary>
    /// Asset Bundles文件夹
    /// </summary>
    public static string AssetBundlesDir = "AssetBundles";
    
    /// <summary>
    /// 游戏中的资源下载完成
    /// </summary>
    public const string ABDownloadSuccess = "abDownloadSuccess";

    /// <summary>
    /// 游戏中数据下载完成
    /// </summary>
    public const string EVENT_UPDATESUCCESSONGAMING = "EVENT_UPDATESUCCESSONGAMING";

    /// <summary>
    /// 游戏开始更新资源完成
    /// </summary>
    public const string EVENT_UPDATE_SUCCESS_ON_GAMESTART = "EVENT_UPDATE_SUCCESS_ON_GAMESTART";

    /// <summary>
    /// 通知UI层资源更新完成
    /// </summary>
    public const string EVENT_UI_UPDATE_COMPOLETE = "Event_UI_Update_Complete";
    
    /// <summary>
    /// 询问是否更新
    /// </summary>
    public const string EVENT_ASK_UPDATE_RESOURCES = "event_ask_updateresoruces";

    /// <summary>
    /// 游戏开始时的资源更新
    /// </summary>
    public const string EVENT_GAME_START_RESOURCES_UPDATE = "event_game_start_resources_update";
    
    
    public enum AssetBundleType
    {
        
    }
    
    
    /// <summary>
    /// 需要映射路径的资源后缀
    /// </summary>
    public static List<string> MappingAssetTypes = new List<string>()
    {
        ".asset",
        ".prefab",
        ".json",
        ".controller",
        ".bytes"
    };
    
    
    /// <summary>
    /// 获取打包平台对应的名字
    /// </summary>
    /// <returns></returns>
    public static string RuntimePlatformName()
    {
#if UNITY_EDITOR
        if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.Android)
        {
            return "Android";
        }else if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.iOS)
        {
            return "iOS";
        }
        else
        {
            return "Windows";
        }
#else
        if (Application.platform == RuntimePlatform.Android)
        {
            return "Android";
        }else if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            return "iOS";
        }
        else
        {
            return "Windows";
        }
#endif
    }


    /// <summary>
    /// 获取只读路径
    /// </summary>
    /// <returns></returns>
    public static string GetReadOnlyPathByWWW()
    {
        // 获取路径
        var readOnlyFilePath = string.Format("{0}/{1}/{2}", Application.streamingAssetsPath,
            READ_ONLY_DIR, RuntimePlatformName());

        if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.OSXEditor) // Androd下的路径不需要 "file://"
        {
            readOnlyFilePath = "file://" + readOnlyFilePath;
        }

        return readOnlyFilePath;
    }


    /// <summary>
    /// 获取可读写的目录
    /// Application.persistentDataPath
    /// </summary>
    /// <returns></returns>
    public static string GetReadWritePath()
    {
        return string.Format("{0}/{1}/{2}", AssetBundlesDir, RuntimePlatformName(), VERSION_FILE);
    }
    
}