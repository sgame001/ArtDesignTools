// // // ================================================================
// // // FileName:IDatabaseAdapterInterface.cs
// // // User: Baron
// // // CreateTime:2017-09-08-16:44
// // // Description:数据库适配器接口处理
// // // 
// // // Copyright (c) 2016 Heeking.Co.Ltd. All rights reserved.
// // // ================================================================

using System;
using System.Collections.Generic;
using System.Reflection;

namespace HKLibrary
{
    public interface IDatabaseAdapterInterface
    {
        /// <summary>
        /// 打开数据库
        /// 可以指定数据库名字
        /// Mysql不需要传入名字
        /// Sqlite默认是main
        /// </summary>
        /// <param name="_dbName"></param>
        void OpenDatabase(string _dbName);

        /// <summary>
        /// 关闭数据库
        /// </summary>
        void CloseDatabase();

        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="_sql"></param>
        bool Query(string _sql);
        
        /// <summary>
        /// 读取一条数据
        /// </summary>
        /// <returns></returns>
        bool Read();
        byte   GetByte(int   _index);
        short  GetShort(int  _index);
        int    GetInt(int    _index);
        long   GetLong(int   _index);
        float  GetFloat(int  _index);
        string GetString(int _index);
    }
}