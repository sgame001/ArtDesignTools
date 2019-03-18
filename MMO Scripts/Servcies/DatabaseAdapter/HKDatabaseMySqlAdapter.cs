// // ================================================================
// // FileName:HKDatabaseMySqlAdapter.cs
// // User: Baron-Fisher
// // CreateTime:2017 0822 17:28
// // Description:
// // Copyright (c) 2017 Greg.Co.Ltd. All rights reserved.
// // ================================================================

#if UNITY_EDITOR
using System;
using System.Data;
using MySql.Data.MySqlClient;

namespace HKGameLogic
{
    public class HKDatabaseMySqlAdapter : IDatabaseAdapter
    {
        /// <summary>
        /// 数据库ip
        /// </summary>
        private string databaseIP = "192.168.1.251";

        /// <summary>
        /// 数据库名称
        /// </summary>
        private string databaseName = "heeking_gamedata_ahsj";

        /// <summary>
        /// 数据库端口号
        /// </summary>
        private int databasePort = 3306;

        /// <summary>
        /// 数据库用户名
        /// </summary>
        private string databaseUserName = "root";

        /// <summary>
        /// 数据库密码
        /// </summary>
        private string databasePassword = "root123";


        /// <summary>
        /// 连接mySql的语句
        /// </summary>
        private string openSqlStr = "";

        /// <summary>
        /// 数据库连接
        /// </summary>
        private MySqlConnection dbConntect = null;

        /// <summary>
        /// data set
        /// </summary>
        private DataTableReader dataReader;

        /// <summary>
        /// 初始化数据库相关信息
        /// </summary>
        public void InitData()
        {
            openSqlStr =
                string.Format("Server = {0};port={4};Database = {1}; User ID = {2}; Password = {3};", databaseIP,
                    databaseName, databaseUserName, databasePassword, databasePort);
        }

        /// <summary>
        /// 打开数据库
        /// </summary>
        /// <param name="_dbName"></param>
        public void OpenDatabase(string _dbName)
        {
            if (false == string.IsNullOrEmpty(openSqlStr))
            {
                dbConntect = new MySqlConnection(openSqlStr);
                dbConntect.Open();
                UnityEngine.Debug.Log("MySql  数据库连接成功");
            }
            else
            {
                UnityEngine.Debug.LogError("数据库连接字符串出错");
            }
        }


        /// <summary>
        /// 关闭数据库
        /// </summary>
        public void CloseDatabase()
        {
            if (null != dbConntect)
            {
                dbConntect.Close();
                dbConntect.Dispose();
                dbConntect = null;
            }
        }

        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="_sql"></param>
        public bool Query(string _sql)
        {
            if (dbConntect.State == ConnectionState.Open)
            {
                var dataSet = new DataSet();
                try
                {
                    MySqlDataAdapter da = new MySqlDataAdapter(_sql, dbConntect);
                    da.Fill(dataSet);
                    dataReader = dataSet.CreateDataReader();
                    return true;
                }
                catch (Exception ee)
                {
                    UnityEngine.Debug.Log(_sql + "#" + ee.Message);
                }
            }
            return false;
        }

        public bool Read()
        {
            return dataReader.Read();
        }

        public byte GetByte(int _index)
        {
            return dataReader.GetByte(_index);
        }

        public short GetShort(int _index)
        {
            return dataReader.GetInt16(_index);
        }

        public int GetInt(int _index)
        {
            return dataReader.GetInt32(_index);
        }

        public long GetLong(int _index)
        {
            return dataReader.GetInt64(_index);
        }

        public float GetFloat(int _index)
        {
            return dataReader.GetFloat(_index);
        }

        public string GetString(int _index)
        {
            return dataReader.GetString(_index);
        }

        /// <summary>
        /// 异常监听
        /// </summary>
        /// <param name="_errorMessage"></param>
        public void OnError(string _errorMessage)
        {
        }

    }
}
#endif