// // ================================================================
// // FileName:HKDatabaseSqliteAdapter.cs
// // User: Baron-Fisher
// // CreateTime:2017 0822 16:52
// // Description:
// // Copyright (c) 2017 Greg.Co.Ltd. All rights reserved.
// // ================================================================

using System;
using CatLib;
using GOEGame;
using HKLibrary;
using Application = UnityEngine.Application;

namespace HKGameLogic
{
    public class HKDatabaseSqliteAdapter : IDatabaseAdapter
    {
        /// <summary>
        /// 所有的表
        /// </summary>
        private const string DATA_BASE_NAME = "all_data.db";

        /// <summary>
        /// 数据库路径
        /// </summary>
        private string databasePath = "";

        /// <summary>
        /// 数据库文件
        /// </summary>
        private SqliteDatabase database = null;

        /// <summary>
        /// data table
        /// </summary>
        private GOEGame.DataTable dataTable;
        
        /// <summary>
        /// 初始化数据
        /// </summary>
        public void InitData()
        {
            if (true == HKCommonDefine.IsMobileDevice)
            {
                databasePath = string.Format("{0}/{1}", FileSystemFacade.Instance.GetDiskPath(), DATA_BASE_NAME);
            }
            else
            {
                databasePath = string.Format("{0}/ClientResources/DataBase/{1}", Application.dataPath, DATA_BASE_NAME);
            }
            database = new SqliteDatabase(databasePath);
        }

        /// <summary>
        /// 打开数据库
        /// </summary>
        /// <param name="_dbName"></param>
        public void OpenDatabase(string _dbName)
        {
            if (null != database)
            {
                database.Open();
            }
        }
        
        /// <summary>
        /// 关闭数据库
        /// </summary>
        public void CloseDatabase()
        {
            if (null != database)
            {
                database.Close();
                database = null;
            }
        }

        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="_sql"></param>
        public bool Query(string _sql)
        {
            dataTable = database.ExecuteQuery(_sql);
            return true;
        }

        /// <summary>
        /// 读取一条数据
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public bool Read()
        {
            return true == dataTable.Read() && true == dataTable.HasRows;
        }

        public byte GetByte(int _index)
        {
            return dataTable.GetByte(_index);
        }

        public short GetShort(int _index)
        {
            return dataTable.GetShort(_index);
        }

        public int GetInt(int _index)
        {
            return dataTable.GetInt(_index);
        }

        public long GetLong(int _index)
        {
            return dataTable.GetLong(_index);
        }

        public float GetFloat(int _index)
        {
            return dataTable.GetFloat(_index);
        }

        public string GetString(int _index)
        {
            return dataTable.GetString(_index);
        }

        /// <summary>
        /// 数据库错误信息回调
        /// </summary>
        /// <param name="_errorMessage"></param>
        public void OnError(string _errorMessage)
        {
        }
    }
}