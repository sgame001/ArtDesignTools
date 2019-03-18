// // ================================================================
// // FileName:HKDBAdapterComponent.cs
// // User: Baron-Fisher
// // CreateTime:2017 0822 18:50
// // Description:数据库适配工具
// // Copyright (c) 2017 Greg.Co.Ltd. All rights reserved.
// // ================================================================

using HKGameLogic;

namespace HKLibrary
{
    /// <summary>
    /// 适配器类型
    /// </summary>
    public enum SqlDBAdapterDataType
    {
        Mysql,
        Sqlite
    }

    public class HKDatabaseAdapter : IDatabaseAdapterInterface
    {
        /// <summary>
        /// 数据库类型
        /// </summary>
        private SqlDBAdapterDataType SqlType { get; set; }

        /// <summary>
        /// 数据库适配器
        /// </summary>
        private IDatabaseAdapter databaseAdapter;

        /// <summary>
        /// 初始化
        /// </summary>
        public void InitData(SqlDBAdapterDataType _sqlType)
        {
            SqlType = _sqlType;
            if (SqlType == SqlDBAdapterDataType.Mysql)
            {
#if UNITY_EDITOR
                databaseAdapter = new HKDatabaseMySqlAdapter();
#endif
            }
            else if (SqlType == SqlDBAdapterDataType.Sqlite)
            {
                databaseAdapter = new HKDatabaseSqliteAdapter();
            }

            if (null != databaseAdapter)
            {
                databaseAdapter.InitData();
            }
        }

        /// <summary>
        /// 打开数据库
        /// </summary>
        /// <param name="_dbName"></param>
        public void OpenDatabase(string _dbName)
        {
            if (null != databaseAdapter)
            {
                databaseAdapter.OpenDatabase(_dbName);
            }
        }

        /// <summary>
        /// 关闭数据库
        /// </summary>
        public void CloseDatabase()
        {
            if (null != databaseAdapter)
            {
                databaseAdapter.CloseDatabase();
            }
        }

        public bool Query(string _sql)
        {
            return databaseAdapter.Query(_sql);
        }

        /// <summary>
        /// 读取一行数据
        /// </summary>
        /// <returns></returns>
        public bool Read()
        {
            return databaseAdapter.Read();
        }

        /// <summary>
        /// 读取byte
        /// </summary>
        /// <param name="_index"></param>
        /// <returns></returns>
        public byte GetByte(int _index)
        {
            return databaseAdapter.GetByte(_index);
        }

        /// <summary>
        /// 读取short
        /// </summary>
        /// <param name="_index"></param>
        /// <returns></returns>
        public short GetShort(int _index)
        {
            return databaseAdapter.GetShort(_index);
        }

        /// <summary>
        /// 读取int
        /// </summary>
        /// <param name="_index"></param>
        /// <returns></returns>
        public int GetInt(int _index)
        {
            return databaseAdapter.GetInt(_index);
        }

        /// <summary>
        /// 读取long
        /// </summary>
        /// <param name="_index"></param>
        /// <returns></returns>
        public long GetLong(int _index)
        {
            return databaseAdapter.GetLong(_index);
        }

        /// <summary>
        /// 读取float
        /// </summary>
        /// <param name="_index"></param>
        /// <returns></returns>
        public float GetFloat(int _index)
        {
            return databaseAdapter.GetFloat(_index);
        }

        /// <summary>
        /// 读取string
        /// </summary>
        /// <param name="_index"></param>
        /// <returns></returns>
        public string GetString(int _index)
        {
            return databaseAdapter.GetString(_index);
        }
    }
}