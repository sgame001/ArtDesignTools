// // // ================================================================
// // // FileName:HKFileSystem.cs
// // // User: Baron
// // // CreateTime:2017-09-28-14:16
// // // Description:文件管理系统
// // // 
// // // Copyright (c) 2016 Heeking.Co.Ltd. All rights reserved.
// // // ================================================================

using CatLib;
using CatLib.API.FileSystem;

namespace HKLibrary
{
    public class HKFileSystem : IFileDiskSystem
    {
        /// <summary>
        /// 文件管理器
        /// </summary>
        private readonly IFileSystem fileSystem;


        public HKFileSystem()
        {
            if (null == fileSystem)
            {
                fileSystem = App.Make<IFileSystemManager>().Disk();
            }
        }

        /// <summary>
        /// 获取读写目录
        /// </summary>
        /// <returns></returns>
        public string GetDiskPath()
        {
            return UnityEngine.Application.persistentDataPath;
        }


        /// <summary>
        /// 获取本地的一个文件
        /// </summary>
        /// <param name="_path"></param>
        /// <returns></returns>
        public string GetFileName(string _path)
        {
            if (false == CheckNullPath(_path))
            {
                return null;
            }
            return System.IO.Path.GetFileName(_path);
        }

        /// <summary>
        /// 获取不带后缀名的文件名称
        /// </summary>
        /// <param name="_path"></param>
        /// <returns></returns>
        public string GetFileNameWithoutSuffix(string _path)
        {
            if (false == CheckNullPath(_path))
            {
                return null;
            }

            return System.IO.Path.GetFileNameWithoutExtension(_path);
        }

        /// <summary>
        /// 读取一个文件
        /// </summary>
        /// <param name="_filePath"></param>
        /// <returns></returns>
        public byte[] ReadFile(string _filePath)
        {
            if (false == CheckNullPath(_filePath))
            {
                return null;
            }
            return fileSystem.Read(_filePath);
        }

        /// <summary>
        /// 检测文件是否存在
        /// </summary>
        /// <param name="_filePath"></param>
        /// <returns></returns>
        public bool FileExists(string _filePath)
        {
            if (false == CheckNullPath(_filePath))
            {
                return false;
            }

            return fileSystem.Exists(_filePath);
        }

        /// <summary>
        /// 创建文件
        /// </summary>
        /// <param name="_targetPath"></param>
        /// <param name="_contents"></param>
        public void CreateFile(string _targetPath, byte[] _contents)
        {
            if (false == CheckNullPath(_targetPath))
            {
                return;
            }

            if (null == _contents || _contents.Length <= 0)
            {
                this.Warr("write file contents length must be than 0");
                return;
            }
            fileSystem.Write(_targetPath, _contents);
        }

        /// <summary>
        /// 创建文件夹
        /// </summary>
        /// <param name="_dirPath"></param>
        public void MakeDir(string _dirPath)
        {
            if (false == CheckNullPath(_dirPath))
            {
                return;
            }

            fileSystem.MakeDir(_dirPath);
        }

        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="_filePath"></param>
        /// <returns></returns>
        public bool DeleteFile(string _filePath)
        {
            if (false == CheckNullPath(_filePath))
            {
                return false;
            }

            fileSystem.Delete(_filePath);
            return true;
        }

        /// <summary>
        /// 移动文件
        /// </summary>
        /// <param name="_sourceFilePath"></param>
        /// <param name="_targetFilePath"></param>
        public void MoveFile(string _sourceFilePath, string _targetFilePath)
        {
            if (false == CheckNullPath(_sourceFilePath) || false == CheckNullPath(_targetFilePath))
            {
                return;
            }

            fileSystem.Move(_sourceFilePath, _targetFilePath);
        }

        /// <summary>
        /// 拷贝文件
        /// </summary>
        /// <param name="_sourceFilePath"></param>
        /// <param name="_targetFilePath"></param>
        public void CopyFile(string _sourceFilePath, string _targetFilePath)
        {
            if (false == CheckNullPath(_sourceFilePath) || false == CheckNullPath(_targetFilePath))
            {
                return;
            }

            fileSystem.Copy(_sourceFilePath, _targetFilePath);
        }


        /// <summary>
        /// 获取文件尺寸
        /// </summary>
        /// <param name="_filePath"></param>
        /// <returns></returns>
        public long GetFileSize(string _filePath)
        {
            if (false == CheckNullPath(_filePath))
            {
                return 0;
            }

            return fileSystem.GetSize(_filePath);
        }

        /// <summary>
        /// 获取第一级目录下的所有文件
        /// </summary>
        /// <returns></returns>
        public IHandler[] GetFirstLayerSubList(string _path)
        {
            if (true == string.IsNullOrEmpty(_path))
            {
                return null;
            }

            if (false == fileSystem.Exists(_path))
            {
                this.Debug("Folder does not exist", _path);
                return null;
            }

            return fileSystem.GetList(_path);
        }


        /// <summary>
        /// 检测路径
        /// </summary>
        /// <param name="_path"></param>
        /// <returns></returns>
        private bool CheckNullPath(string _path)
        {
            if (true == string.IsNullOrEmpty(_path))
            {
                this.Warr("path is null, don't get file name ");
                return false;
            }
            return true;
        }
    }
}