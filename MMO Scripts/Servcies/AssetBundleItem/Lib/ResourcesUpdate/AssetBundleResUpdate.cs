// // ================================================================
// // FileName:AssetBundleResUpdate.cs
// // User: Baron
// // CreateTime:1/29/2018
// // Description: 资源更新
// // ================================================================

using System;
using System.Collections.Generic;
using CatLib;
using HKLibrary;
using UnityEngine;

public class AssetBundleResUpdate
{
	/// <summary>
	/// 下载信息
	/// </summary>
	private static readonly AssetBundleDownloadInfo downloadInfo = new AssetBundleDownloadInfo();

	/// <summary>
	/// 服务器版本内容
	/// </summary>
	private AssetBundleVersion serverVersion = null;
	
	/// <summary>
	/// 总前总计的流量
	/// </summary>
	private long allBytesSize = 0;

	/// <summary>
	/// 开始下载的时间
	/// </summary>
	private float startDownloadTime = 0;

	/// <summary>
	/// 开始下载的句柄
	/// </summary>
	private IRequestTaskQueue startDownloadHandler = null;
	
	/// <summary>
	/// 游戏中下载的句柄
	/// </summary>
	private IRequestTaskQueue gameingDownloadHandler = null;
	
	/// <summary>
	/// 资源下载进度回调
	/// </summary>
	public Action<HKDownloadUpdateInfo> DownloadProgressCallback { get; set; }

	
	/// <summary>
	/// 资源更新初始化
	/// 可以在这里做一些路径定义获取等工作
	/// </summary>
	public void Init()
	{
		// 添加监听事件，通知开始下载
		AppEvent.OnEvent(AssetBundleDefine.EVENT_GAME_START_RESOURCES_UPDATE, o => { GameStartDownload(); });
	}
	
	/// <summary>
	/// 资源更新入口
	/// </summary>
	public void UpdateGameResources()
	{
		string versionFile = string.Format("{0}/{1}/{2}", AssetBundleDefine.SERVER_RES_PATH, AssetBundleDefine.RuntimePlatformName(), AssetBundleDefine.VERSION_FILE);
		this.Info("version file path = " + versionFile);
		App.Make<IHttpRequest>().GetBytes(versionFile, bytes =>
		{
			if (null == bytes || bytes.Length <= 0)
			{
				this.Error("Version文件下载失败");
				return;
			}

			var decompressBytes = GameFramework.Utility.Zip.Decompress(bytes);
			string versionContent = System.Text.Encoding.UTF8.GetString(decompressBytes);
			if (true == string.IsNullOrEmpty(versionContent))
			{
				this.Error("Version文件解析失败");
				return;
			}

			// 存入本地，并不是做对比使用，而是name和path映射做准备
			App.Make<IFileDiskSystem>()
				.CreateFile(string.Format("{0}/{1}/{2}", AssetBundleDefine.AssetBundlesDir, AssetBundleDefine.RuntimePlatformName(), AssetBundleDefine.VERSION_FILE),
					decompressBytes);
			
			serverVersion = JsonUtility.FromJson<AssetBundleVersion>(versionContent);
			if (null == serverVersion)
			{
				this.Error("version文件转换失败");
				return;
			}

			var downloadList = CheckDownloadList(serverVersion.AssetBundleVersionItems, AssetPathType.persisent);
			downloadInfo.SetDownloadList(AssetPathType.persisent, downloadList);

			if (downloadList.Count > 0)
			{
				this.Debug("总计下载文件个数 = " + downloadList.Count + "  大小 = " + downloadInfo.DownloadSizeCountDes);
				// 这里要根据网络情况，判断是否直接下载

				if (UnityEngine.Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork) // 移动网络
				{
					// 开始下载
					AppEvent.BroadCastEvent(AssetBundleDefine.EVENT_GAME_START_RESOURCES_UPDATE);
				}else if (UnityEngine.Application.internetReachability == NetworkReachability.ReachableViaCarrierDataNetwork) // WIFI网络
				{
					// 询问玩家
					AppEvent.BroadCastEvent(AssetBundleDefine.EVENT_ASK_UPDATE_RESOURCES, downloadInfo);
				}
			}
			else
			{
				GameStartDownloadComplete(null); // 没有最新资源需要下载
			}
		}, _errorCode =>
		{
			this.Error("Version文件下载失败 error code = " + _errorCode);
		});
	}


	
	/// <summary>
	/// 获取下载队列句柄
	/// </summary>
	/// <param name="_pathType"></param>
	/// <returns></returns>
	private IRequestTaskQueue GetDownloadHandler(AssetPathType _pathType)
	{
		var taskQueue = App.Make<IRequestTaskQueue>();
		if (null != serverVersion)
		{
			var downloadItems = CheckDownloadList(serverVersion.AssetBundleVersionItems, _pathType);
			if (null == downloadItems)
			{
				return null;
			}
			
			downloadInfo.SetDownloadList(_pathType, downloadItems);

			if (downloadItems.Count > 0)
			{
				for (int index = 0; index < downloadItems.Count; index++)
				{
					var downloadItem = downloadItems[index];
					var path = GetDownloadPath(downloadItem);
					if (null == path)
					{
						continue;
					}
					taskQueue.AddTask(path, DownloadResType.ByteArray, SingleAssetComplete, downloadItem);
				}
			}
		}

		return taskQueue;
	}
	
	
	/// <summary>
	/// 游戏开始下载必要数据
	/// </summary>
	private void GameStartDownload()
	{
		startDownloadHandler = GetDownloadHandler(AssetPathType.persisent);
		if (null != startDownloadHandler)
		{
			// 添加Manifest的下载
			startDownloadHandler.AddTask(
				string.Format("{0}/{1}/{2}", AssetBundleDefine.SERVER_RES_PATH, AssetBundleDefine.RuntimePlatformName(), AssetBundleDefine.MANIFEST_FILE),
				DownloadResType.ByteArray, ManifestDlwonloadComplete, null);
			
			startDownloadHandler.SetCallBack(DownloadUpdateInfo, GameStartDownloadComplete);
			startDownloadHandler.StartDownload();
		}
	}

	
	/// <summary>
	/// 游戏中下载非必要性数据
	/// </summary>
	public void GameingDownload()
	{
		gameingDownloadHandler = GetDownloadHandler(AssetPathType.gameLoad);
		if (null != gameingDownloadHandler)
		{
			gameingDownloadHandler.SetCallBack(DownloadUpdateInfo, GameingDownloadComplete);
			gameingDownloadHandler.StartDownload();
		}
	}

	
	/// <summary>
	/// 停止下载
	/// </summary>
	public void StopDownload(AssetPathType _type)
	{
		if (_type == AssetPathType.gameLoad && null != gameingDownloadHandler)
		{
			gameingDownloadHandler.StopDownload();
		}
		DownloadProgressCallback = null;
	}
	
	
	/// <summary>
	/// 所有资源下载完成
	/// </summary>
	/// <param name="s"></param>
	private void GameStartDownloadComplete(string s)
	{
		if (false == string.IsNullOrEmpty(s))
		{
			this.Error("游戏开始下载数据有失败信息 = " + s);
			// 这里如果return可能会导致整个游戏流程中断
		}

		// 游戏启动下载资源完成
		AppEvent.BroadCastEvent(AssetBundleDefine.EVENT_UPDATE_SUCCESS_ON_GAMESTART);
		
		// 统计游戏中需要下载的的资源
		var gameLoaDownloadList = CheckDownloadList(serverVersion.AssetBundleVersionItems, AssetPathType.gameLoad);
		if (null != gameLoaDownloadList && gameLoaDownloadList.Count > 0)
		{
			if (null != downloadInfo)
			{
				downloadInfo.SetDownloadList(AssetPathType.gameLoad, gameLoaDownloadList);
			}
			this.Info("游戏中需要下载资源统计完成 数量 = " + gameLoaDownloadList.Count);
		}
		
	}

	/// <summary>
	/// 游戏中所有资源下载完成
	/// </summary>
	/// <param name="_s"></param>
	private void GameingDownloadComplete(string _s)
	{
		if (false == string.IsNullOrEmpty(_s))
		{
			this.Error("游戏中下载数据有失败信息 = " + _s);
			return;
		}

		// 游戏中资源下载完成
		AppEvent.BroadCastEvent(AssetBundleDefine.EVENT_UPDATESUCCESSONGAMING);
	}

	
	/// <summary>
	/// 资源更新
	/// </summary>
	/// <param name="_downloadUpdateInfo"></param>
	private void DownloadUpdateInfo(HKDownloadUpdateInfo _downloadUpdateInfo)
	{
		allBytesSize += _downloadUpdateInfo.currentFrameSize;

		if (null != DownloadProgressCallback)
		{
			DownloadProgressCallback(_downloadUpdateInfo); // 通知回调
		}
//		this.Debug("_downloadUpdateInfo = " + _downloadUpdateInfo.FileName + "  index = " + _downloadUpdateInfo.CurrentIndex +
//		           "  progress = " + _downloadUpdateInfo.CurrentProgress);
	}


	/// <summary>
	/// 单个资源下载完成
	/// 将文件写入本地
	/// 如果有压缩或者加密，可以在这里进行解析
	/// </summary>
	/// <param name="s"></param>
	/// <param name="o"></param>
	/// <param name="arg3"></param>
	private void SingleAssetComplete(string s, object o, object arg3)
	{
		byte[] bytes = (byte[]) o;
		AssetBundleVersionItem downloadItem = (AssetBundleVersionItem) arg3;

		string localItemPath = GetFileLocalPath(downloadItem);
		
		// 写入到本地
		App.Make<IFileDiskSystem>().CreateFile(localItemPath, bytes);

		if (downloadItem.AssetPathType == AssetPathType.gameLoad)
		{
			// 广播，通知临时资源使用中心来替换临时资源（一般为模型，音效，UI等）
			AppEvent.BroadCastEvent(string.Format("{0}_{1}", AssetBundleDefine.ABDownloadSuccess, localItemPath));
		}
	}


	/// <summary>
	/// 将manifest写入本地
	/// </summary>
	/// <param name="s"></param>
	/// <param name="o"></param>
	/// <param name="arg3"></param>
	private void ManifestDlwonloadComplete(string s, object o, object arg3)
	{
		byte[] bytes = (byte[]) o;
		string localPath = string.Format("{0}/{1}/{2}", AssetBundleDefine.AssetBundlesDir, AssetBundleDefine.RuntimePlatformName(), AssetBundleDefine.MANIFEST_FILE);
		App.Make<IFileDiskSystem>().CreateFile(localPath, bytes);
	}
	
	
	/// <summary>
	/// 检测某个类型的资源下载里列表
	/// </summary>
	/// <returns></returns>
	private List<AssetBundleVersionItem> CheckDownloadList(List<AssetBundleVersionItem> _abItemList, AssetPathType _pathType)
	{
		List<AssetBundleVersionItem> result = new List<AssetBundleVersionItem>();
		if (null == _abItemList || _abItemList.Count <= 0)
		{
			return null;
		}

		long sizeCount = 0;
		for (int index = 0; index < _abItemList.Count; index++)
		{
			var item = _abItemList[index];
			if (null == item)
			{
				continue;
			}

			string fileFullPath = GetFileLocalPath(item);
			if (false == App.Make<IFileDiskSystem>().FileExists(fileFullPath)) // 这里直接检测本地文件
			{
				if (item.AssetPathType == _pathType)
				{
					sizeCount += item.size;
					result.Add(item);
				}
			}
		}

		downloadInfo.DownloadSizeCount = sizeCount;
		return result;
	}


	/// <summary>
	/// 获取一个资源的下载路径
	/// </summary>
	/// <param name="_versionItem"></param>
	/// <returns></returns>
	private string GetDownloadPath(AssetBundleVersionItem _versionItem)
	{
		if (null == _versionItem)
		{
			return null;
		}
		string downloadFullPath =
			string.Format("{0}/{1}/{2}${3}.dat", AssetBundleDefine.SERVER_RES_PATH, AssetBundleDefine.RuntimePlatformName(),  _versionItem.ItemName, _versionItem.MD5);

		return downloadFullPath;
	}


	/// <summary>
	/// 获取本地存储路径
	/// </summary>
	/// <param name="_versionItem"></param>
	/// <returns></returns>
	public static string GetFileLocalPath(AssetBundleVersionItem _versionItem)
	{
		if (null == _versionItem)
		{
			return null;
		}
		
		string fileFullPath = string.Format("{0}/{1}/{2}${3}.dat", AssetBundleDefine.AssetBundlesDir,  AssetBundleDefine.RuntimePlatformName(), _versionItem.ItemName, _versionItem.MD5);
		return fileFullPath;
	}
	
}
