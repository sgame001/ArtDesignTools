// // ================================================================
// // FileName:HKSpriteManager.cs
// // User: Baron-Fisher
// // CreateTime:2017 0910 0:29
// // Description:下载相关的工具类
// // Copyright (c) 2016 Greg.Co.Ltd. All rights reserved.
// // ================================================================

using System;
using System.IO;
using CatLib;
using CatLib.API.FileSystem;

namespace HKLibrary
{
    public class HKRequestTools
    {
        /// <summary>
        /// logger
        /// </summary>
        private static Type logger = typeof(HKRequestTools);

        /// <summary>
        /// 资源下载的基础路径
        /// </summary>
        public static string DownloadBaseUrl = "http://115.159.147.94:8086/datelog/";
        
        /// <summary>
        /// 检测url的合法性
        /// </summary>
        /// <param name="_url"></param>
        /// <returns></returns>
        public static bool IsCheckUrl(string _url)
        {
            if (true == string.IsNullOrEmpty(_url))
            {
                logger.Warr("请求路径不能为空");
                return false;
            }
            return true;
        }

        /// <summary>
        /// 将一个数组，以AssetBundle的形式写入到本地目录中
        /// </summary>
        /// <param name="_url"></param>
        /// <param name="_bytes"></param>
        public static void WriteAssetBundleToLcoal(string _url, byte[] _bytes)
        {
            if (true == string.IsNullOrEmpty(_url))
            {
                logger.Error("写入时，原始路径为空，无法写入");
                return;
            }

            if (null == _bytes || _bytes.Length <= 0)
            {
                logger.Error(StringCacheFactory.GetFree().Add("写入本地时，原始数据为null = ")
                    .Add(_url));
                return;
            }
            
            //  写入本地 如果有重复则会覆盖
            string savePath = Path.GetFileName(_url);
            if (false == string.IsNullOrEmpty(DownloadBaseUrl))
            {
                savePath = _url.Replace(DownloadBaseUrl, "");
            }
            
            // 调用File系统，将字节写入到本地文件中，路径保持和网络格式路径格式相同
            App.Make<IFileSystemManager>().Disk().Write(savePath, _bytes);
        }
    }
}