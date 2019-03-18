// // // ================================================================
// // // FileName:IFileSystem.cs
// // // User: Baron
// // // CreateTime:2017-09-28-14:16
// // // Description:
// // // 
// // // Copyright (c) 2016 Heeking.Co.Ltd. All rights reserved.
// // // ================================================================

using System.Collections.Generic;
using CatLib.API.FileSystem;

namespace HKLibrary
{
    public interface IFileDiskSystem
    {
        /// <summary>
        /// 获取可读写目录
        /// </summary>
        /// <returns></returns>
        string GetDiskPath();

        /// <summary>
        /// 获取文件名称,包含后缀名
        /// </summary>
        /// <param name="_path">文件的相对路径</param>
        /// <returns></returns>
        string GetFileName(string _path);

        /// <summary>
        /// 获取一个路径名称,不包含后缀名
        /// </summary>
        /// <param name="_path"></param>
        /// <returns></returns>
        string GetFileNameWithoutSuffix(string _path);
        
        /// <summary>
        /// 将文件以byte的形式读取出来
        /// </summary>
        /// <param name="_filePath">文件相对路径</param>
        /// <returns></returns>
        byte[] ReadFile(string _filePath);

        /// <summary>
        /// 判断一个文件路径是否存在
        /// </summary>
        /// <param name="_filePath">文件相对路径</param>
        /// <returns></returns>
        bool FileExists(string _filePath);

        /// <summary>
        /// 将byte写入到文件中
        /// 如果存在会进行覆盖
        /// </summary>
        /// <param name="_targetPath"></param>
        /// <param name="_contents"></param>
        void CreateFile(string _targetPath, byte[] _contents);

        /// <summary>
        /// 创建一个文件夹
        /// </summary>
        /// <param name="_dirPath"></param>
        void MakeDir(string _dirPath);
        
        /// <summary>
        /// 删除一个文件
        /// </summary>
        /// <param name="_filePath">文件相对路径</param>
        /// <returns></returns>
        bool DeleteFile(string _filePath);

        /// <summary>
        /// 移动文件
        /// </summary>
        /// <param name="_sourceFilePath">源相对路径</param>
        /// <param name="_targetFilePath">目标相对路径</param>
        void MoveFile(string _sourceFilePath, string _targetFilePath);

        /// <summary>
        /// 拷贝文件
        /// </summary>
        /// <param name="_sourceFilePath">源相对路径</param>
        /// <param name="_targetFilePath">目标相对路径</param>
        void CopyFile(string _sourceFilePath, string _targetFilePath);

        /// <summary>
        /// 获取文件尺寸
        /// </summary>
        /// <param name="_filePath"></param>
        /// <returns></returns>
        long GetFileSize(string _filePath);

        
        /// <summary>
        /// 获取第一级目录下的所有文件
        /// </summary>
        /// <returns></returns>
        IHandler[] GetFirstLayerSubList(string  _path);

    }
}