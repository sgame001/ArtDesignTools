// // ================================================================
// // FileName:IDatabaseAdapter.cs
// // User: Baron-Fisher
// // CreateTime:2017 0822 16:15
// // Description:
// // Copyright (c) 2017 Greg.Co.Ltd. All rights reserved.
// // ================================================================

using System;
using System.Collections.Generic;

namespace HKGameLogic
{
    public interface IDatabaseAdapter
    {
        /// <summary>
        /// 初始化数据
        /// </summary>
        void InitData();

        /// <summary>
        /// 接收数据库错误信息
        /// </summary>
        /// <param name="_errorMessage"></param>
        void OnError(string _errorMessage);

        /// <summary>
        /// 打开数据库
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
        short   GetShort(int  _index);
        int   GetInt(int    _index);
        long   GetLong(int   _index);
        float  GetFloat(int  _index);
        string GetString(int _index);
    }
}