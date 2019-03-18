using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

public class HKEditorFileHelper
{
    /// <summary>
    /// 将一个文本信息写入文件中
    /// </summary>
    /// <param name="_content"></param>
    /// <param name="_path"></param>
    public static void WriteTextToFile(string _content, string _path)
    {
        if (true == string.IsNullOrEmpty(_path) || true == string.IsNullOrEmpty(_content))
        {
            return;
        }
        TextWriter tw = new StreamWriter(_path /***/);
        tw.Write(_content);
        tw.Flush();
        tw.Close();
    }


    /// <summary>
    /// 将一个bytes流写入到文件中
    /// </summary>
    /// <param name="_bytes"></param>
    /// <param name="_path"></param>
    /// <param name="_isCompress">是否压缩</param>
    public static void WriteBytesToFile(byte[] _bytes, string _path, bool _isCompress = false)
    {
        if (null == _bytes || _bytes.Length <= 0 || true == string.IsNullOrEmpty(_path))
        {
            return;
        }

        byte[] writeBytes = _bytes;
        if (true == _isCompress)
        {
            writeBytes = GameFramework.Utility.Zip.Compress(_bytes);
        }
        FileStream fileStream = new FileStream(_path, FileMode.Create);
        fileStream.Write(writeBytes, 0, writeBytes.Length);
        fileStream.Flush();
        fileStream.Close();
    }


    /// <summary>
    /// 以追加的形式将数据写入到文件末尾
    /// </summary>
    /// <param name="_bytes"></param>
    /// <param name="_path"></param>
    public static void WriteBytesToFileAppend(byte[] _bytes, string _path)
    {
        if (null == _bytes || _bytes.Length <= 0 || true == string.IsNullOrEmpty(_path))
        {
            return;
        }

        FileStream fs = null;
        if (false == File.Exists(_path))
        {
            fs = File.Create(_path);
        }
        else
        {
            fs = File.Open(_path, FileMode.Open);
        }
        
        try
        {
            fs.Position = fs.Length;
            fs.Write(_bytes, 0, _bytes.Length);
            fs.Flush();
            fs.Close();
            fs.Dispose();
        }
        catch (Exception e)
        {
            UnityEngine.Debug.Log("资源写入异常 = " + e.Message);
        }
        finally
        {
            if (null != fs)
            {
                fs.Close();
            }
        }
    }
    
    

    public static void GetAllDirBySub(FileSystemInfo _info, List<DirectoryInfo> _list)
    {
        if (!_info.Exists)
            return;
        DirectoryInfo dir = _info as DirectoryInfo;
        //不是目录 
        if (dir == null)
            return;
        FileSystemInfo[] files = dir.GetFileSystemInfos();
        for (int i = 0; i < files.Length; i++)
        {
            DirectoryInfo dirInfo = files[i] as DirectoryInfo;

            if (null != dirInfo)
            {
                _list.Add(dirInfo);
            }
        }
    }
    
    
    /// <summary>
    /// 获取一个目录下的所有子文件
    /// </summary>
    /// <param name="info">Info.</param>
    /// <param name="_list">List.</param>
    /// <param name="_isRecursion"></param>
    public static void GetAllFileBySub(FileSystemInfo info, List<FileInfo> _list, bool _isRecursion = true)
    {
        if (!info.Exists)
            return;
        DirectoryInfo dir = info as DirectoryInfo;
        //不是目录 
        if (dir == null)
            return;
        FileSystemInfo[] files = dir.GetFileSystemInfos();
        for (int i = 0; i < files.Length; i++)
        {
            FileInfo fileInfo = files[i] as FileInfo;
            //是文件 
            if (fileInfo != null)
            {
                _list.Add(fileInfo);
            }
            else
            {
                if (true == _isRecursion) // 是否递归
                {
                    DirectoryInfo di = files[i] as DirectoryInfo;
                    if (null != di && di.Attributes != (FileAttributes.Hidden | di.Attributes))
                    {
                        GetAllFileBySub(files[i], _list);
                    }
                }
            }
        }
    }

    /// <summary>
    /// 通过一个全路径，获取Assets的相对路径
    /// </summary>
    /// <param name="_fullPath"></param>
    /// <param name="_fieldName"></param>
    /// <returns></returns>
    public static string GetRelativePathByName(string _fullPath, string _fieldName)
    {
        if (true == string.IsNullOrEmpty(_fullPath))
        {
            return _fullPath;
        }

        if (true == string.IsNullOrEmpty(_fieldName))
        {
            return _fullPath;
        }

        int assetIndex = _fullPath.IndexOf(_fieldName, System.StringComparison.Ordinal);
        if (-1 != assetIndex)
        {
            string path = _fullPath.Substring(assetIndex);
            return path;
        }
        return _fullPath;
    }
    
    /// <summary>
    /// 获取一个文件的MD5值
    /// </summary>
    /// <param name="filePath"></param>
    /// <returns></returns>
    public static string GetFileHash(string filePath)  
    {             
        try  
        {  
            FileStream fs = new FileStream(filePath, FileMode.Open);  
            int len = (int)fs.Length;  
            byte[] data = new byte[len];  
            fs.Read(data, 0, len);  
            fs.Close();  
            MD5 md5 = new MD5CryptoServiceProvider();  
            byte[] result = md5.ComputeHash(data);  
            string fileMD5 = "";  
            foreach (byte b in result)  
            {  
                fileMD5 += Convert.ToString(b, 16);  
            }  
            return fileMD5;     
        }  
        catch (FileNotFoundException e)  
        {  
            Console.WriteLine(e.Message);  
            return "";  
        }                                   
    }  
}