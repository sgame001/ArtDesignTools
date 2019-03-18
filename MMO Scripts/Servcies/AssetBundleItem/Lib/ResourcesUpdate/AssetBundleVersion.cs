// // ================================================================
// // FileName:AssetBundleVersion.cs
// // User: Baron
// // CreateTime:1/29/2018
// // Description: Asset Bundle Version 文件
// // ================================================================

using System.Collections.Generic;

/// <summary>
/// Asset Path
/// </summary>
public enum AssetPathType
{
    persisent = 0, // 放在云端服务器，需要下载到本地的
    streamAssets, // 打包时就放在本地
    gameLoad // 放在云端，但是属于边玩边下的资源
}

[System.Serializable]
public class AssetBundleVersion
{
    /// <summary>
    /// 文件个数
    /// </summary>
    public int Count;

    /// <summary>
    /// 版本号
    /// </summary>
    public string VersionNubmer;

    /// <summary>
    /// asset bundle version
    /// </summary>
    public List<AssetBundleVersionItem> AssetBundleVersionItems = new List<AssetBundleVersionItem>();
    
}