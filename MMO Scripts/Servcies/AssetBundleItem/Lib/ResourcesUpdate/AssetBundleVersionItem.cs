using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AssetBundleVersionItem
{

	/// <summary>
	/// asset bundle名称
	/// </summary>
	public string ItemName;

	/// <summary>
	/// asset bundle md5
	/// </summary>
	public string MD5;

	/// <summary>
	/// type
	/// </summary>
	public AssetPathType AssetPathType;

	/// <summary>
	/// 尺寸
	/// 方便计算下载的总大小
	/// </summary>
	public long size;

	/// <summary>
	/// 包含的资源
	/// </summary>
	public List<string> AssetsList = new List<string>();
	
}
