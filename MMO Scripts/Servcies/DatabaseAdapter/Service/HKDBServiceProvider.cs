// // // ================================================================
// // // FileName:HKDBServiceProvider.cs
// // // User: Baron
// // // CreateTime:2017-09-08-16:52
// // // Description:数据库的provider
// // // 
// // // Copyright (c) 2016 Heeking.Co.Ltd. All rights reserved.
// // // ================================================================

using CatLib;

namespace HKLibrary
{
    public class HKDBServiceProvider : IServiceProvider
    {
        /// <summary>
        /// 数据库连接类型
        /// </summary>
        public SqlDBAdapterDataType SqlType { get; set; }
        
        public void Init()
        {
        }

        public void Register()
        {
            App.Singleton<HKDatabaseAdapter>().Alias<IDatabaseAdapterInterface>().OnResolving((data, o) =>
            {
                var databaseAdapter = (HKDatabaseAdapter) o;
                databaseAdapter.InitData(SqlType);
                return databaseAdapter;
            }).OnRelease((data, o) =>
            {
                var databaseAdapter = (HKDatabaseAdapter) o;
                databaseAdapter.CloseDatabase();
            });
        }
    }
}