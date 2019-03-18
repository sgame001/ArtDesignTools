// // ================================================================
// // FileName:AssetBundleDownloadInfo.cs
// // User: Baron
// // CreateTime:1/29/2018
// // Description: 下载信息
// // ================================================================

using System.Collections.Generic;

public class AssetBundleDownloadInfo
{
    /// <summary>
    /// 资源下载列表
    /// </summary>
    private readonly Dictionary<AssetPathType, List<AssetBundleVersionItem>> DownloadItemsDic = new Dictionary<AssetPathType, List<AssetBundleVersionItem>>();

    private static readonly List<AssetBundleVersionItem> emptyList = new List<AssetBundleVersionItem>();
    
    /// <summary>
    /// 下载总大小
    /// </summary>
    private long downloadSizeCount = 0;
    public long DownloadSizeCount {
        get { return downloadSizeCount; }
        set
        {
            downloadSizeCount = value;
            DownloadSizeCountDes = CalResTotalSize(downloadSizeCount);
        } }
    
    /// <summary>
    /// 下载size描述
    /// </summary>
    public string DownloadSizeCountDes { get; set; }


    /// <summary>
    /// 设置资源下载
    /// </summary>
    /// <param name="_type"></param>
    /// <param name="_list"></param>
    public void SetDownloadList(AssetPathType _type, List<AssetBundleVersionItem> _list)
    {
        if (null == _list || _list.Count <= 0)
        {
            return;
        }

        if (false == DownloadItemsDic.ContainsKey(_type))
        {
            DownloadItemsDic.Add(_type, _list);
        }
        else
        {
            DownloadItemsDic[_type] = _list;
        }
    }

    
    /// <summary>
    /// 获取下载队列
    /// </summary>
    /// <param name="_type"></param>
    public List<AssetBundleVersionItem> GetDownloadList(AssetPathType _type)
    {
        if (false == DownloadItemsDic.ContainsKey(_type))
        {
            emptyList.Clear();
            return emptyList;
        }
        return DownloadItemsDic[_type];
    }
    
    
    /// <summary>
    /// 计算资源大小
    /// </summary>
    /// <returns></returns>
    private string CalResTotalSize(long _count)
    {
        long totalBytes = _count;
 
        if (totalBytes > 1024 * 1024)
        {
            return (totalBytes / 1024 / 1024) + "M";
        }else if (totalBytes > 1024)
        {
            return (totalBytes / 1024) + "k";
        }
        else
        {
            return totalBytes + "B";
        }
        return "0B";
    }
    
    
}