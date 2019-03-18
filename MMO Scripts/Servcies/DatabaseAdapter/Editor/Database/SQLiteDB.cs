using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using HKLibrary;
using Mono.Data.Sqlite;
using UnityEngine;
using Application = UnityEngine.Application;

namespace SQLiteDatabase
{
    public class SQLiteDB
    {
        public enum DB_DataType
        {
            DB_INT = 1,
            DB_STRING,
            DB_TEXT,
            DB_VARCHAR,
            DB_FLOAT
        }

        public enum DB_Condition
        {
            LESS_THAN = 1,
            GREATER_THAN,
            EQUAL_TO
        }

        public struct DB_Field
        {
            public string name;

            public SQLiteDB.DB_DataType type;

            public int size;

            public bool isPrimaryKey;

            public bool isNotNull;

            public bool isUnique;
        }

        public struct DB_DataPair
        {
            public string fieldName;

            public string value;
        }

        public struct DB_ConditionPair
        {
            public string fieldName;

            public string value;

            public SQLiteDB.DB_Condition condition;
        }

        private static SQLiteDB _instance;

        private string _dbName = string.Empty;

        private string _dbPath = string.Empty;

        private bool _isOverWrite;

        private SqliteConnection connection;

        private SqliteDataReader reader;

        private SqliteCommand command;

        public bool Exists
        {
            get
            {
                return File.Exists(Path.Combine(this._dbPath, this._dbName));
            }
        }

        public static SQLiteDB Instance
        {
            get
            {
                if (SQLiteDB._instance == null)
                {
                    SQLiteDB._instance = new SQLiteDB();
                }
                return SQLiteDB._instance;
            }
        }

        public string DBName
        {
            get
            {
                return this._dbName;
            }
            set
            {
                this._dbName = value;
            }
        }

        public string DBLocation
        {
            get
            {
                return this._dbPath;
            }
            set
            {
                this._dbPath = value;
            }
        }

        private SQLiteDB()
        {
        }

        public bool ConnectToDefaultDatabase(string dbName, bool loadFresh)
        {
            if (string.IsNullOrEmpty(this._dbPath.Trim()))
            {
                this._dbPath = Application.persistentDataPath;
            }
            if (loadFresh)
            {
                try
                {
                    string text = Path.Combine(Application.streamingAssetsPath, dbName);
                    string text2 = Path.Combine(this._dbPath, dbName);
                    if (Application.platform == RuntimePlatform.Android)
                    {
                        text = "jar:file://" + Application.dataPath + "!/assets/" + dbName;
                        WWW wWW = new WWW(text);
                        while (!wWW.isDone)
                        {
                        }
                        if (!wWW.isDone)
                        {
                            if (SQLiteEventListener.onErrorInvoker != null)
                            {
                                SQLiteEventListener.onErrorInvoker("Database not created, please check the default database file. ");
                            }
                            bool result = false;
                            return result;
                        }
                        if (File.Exists(text2))
                        {
                            File.Delete(text2);
                        }
                        File.WriteAllBytes(text2, wWW.bytes);
                    }
                    else
                    {
                        if (File.Exists(text2))
                        {
                            File.Delete(text2);
                        }
                        File.Copy(text, text2);
                    }
                }
                catch (Exception ex)
                {
                    if (SQLiteEventListener.onErrorInvoker != null)
                    {
                        SQLiteEventListener.onErrorInvoker("Database error, " + ex.Message);
                    }
                    bool result = false;
                    return result;
                }
            }
            return this.CreateDatabase(dbName, false);
        }

        public bool CreateDatabase(string dbName, bool isOverWrite)
        {
            this._dbName = dbName.Trim();
            this._isOverWrite = isOverWrite;
            bool result = false;
            if (string.IsNullOrEmpty(this._dbName))
            {
                if (SQLiteEventListener.onErrorInvoker != null)
                {
                    SQLiteEventListener.onErrorInvoker("Database name invalid!");
                }
                return false;
            }
            string text = Path.Combine(this._dbPath, this._dbName);
            try
            {
                if (this._isOverWrite && File.Exists(text))
                {
                    File.Delete(text);
                }
                if (!File.Exists(text))
                {
                    FileStream fileStream = File.Create(text);
                    fileStream.Close();
                }
                this.connection = new SqliteConnection("URI=file:" + text);
                result = true;
            }
            catch (Exception ex)
            {
                if (SQLiteEventListener.onErrorInvoker != null)
                {
                    SQLiteEventListener.onErrorInvoker(string.Concat(new object[]
                    {
                        "Unable to create database. \n",
                        ex.Message,
                        "\n",
                        ex.InnerException,
                        "\n",
                        ex.StackTrace
                    }));
                }
                result = false;
            }
            return result;
        }

        public bool CreateTable(DBSchema schema)
        {
            if (!this.Exists)
            {
                if (SQLiteEventListener.onErrorInvoker != null)
                {
                    SQLiteEventListener.onErrorInvoker("Database does not exists!");
                }
                return false;
            }
            if (this.IsTableExists(schema.TableName))
            {
                if (SQLiteEventListener.onErrorInvoker != null)
                {
                    SQLiteEventListener.onErrorInvoker("Table " + schema.TableName + " already exists.");
                }
                return false;
            }
            if (string.IsNullOrEmpty(schema.TableName))
            {
                if (SQLiteEventListener.onErrorInvoker != null)
                {
                    SQLiteEventListener.onErrorInvoker("No schema name!");
                }
                return false;
            }
            if (schema == null || schema.GetTableFields().Count == 0)
            {
                if (SQLiteEventListener.onErrorInvoker != null)
                {
                    SQLiteEventListener.onErrorInvoker("Empty schema!");
                }
                return false;
            }
            bool result = false;
            try
            {
                int num = 0;
                string text = "create table " + schema.TableName + "(";
                for (int i = 0; i < schema.GetTableFields().Count; i++)
                {
                    if (i == schema.GetTableFields().Count - 1)
                    {
                        string text2 = text;
                        text = string.Concat(new object[]
                        {
                            text2,
                            schema.GetTableFields()[i].name,
                            " ",
                            this.GetDataType(schema.GetTableFields()[i].type),
                            "(",
                            schema.GetTableFields()[i].size,
                            ") "
                        });
                        if (schema.GetTableFields()[i].isNotNull)
                        {
                            text += "NOT NULL";
                        }
                        if (schema.GetTableFields()[i].isUnique || schema.GetTableFields()[i].isPrimaryKey)
                        {
                            text += "UNIQUE";
                        }
                        if (schema.GetTableFields()[i].isPrimaryKey)
                        {
                            num = i + 1;
                        }
                        if (num > 0)
                        {
                            text = text + ",PRIMARY KEY(" + schema.GetTableFields()[num - 1].name + ")";
                        }
                        text += ")";
                    }
                    else
                    {
                        string text2 = text;
                        text = string.Concat(new object[]
                        {
                            text2,
                            schema.GetTableFields()[i].name,
                            " ",
                            this.GetDataType(schema.GetTableFields()[i].type),
                            "(",
                            schema.GetTableFields()[i].size,
                            ") "
                        });
                        if (schema.GetTableFields()[i].isNotNull)
                        {
                            text += "NOT NULL";
                        }
                        if (schema.GetTableFields()[i].isUnique || schema.GetTableFields()[i].isPrimaryKey)
                        {
                            text += "UNIQUE";
                        }
                        if (schema.GetTableFields()[i].isPrimaryKey)
                        {
                            num = i + 1;
                        }
                        text += ",";
                    }
                }
                this.connection.Open();
                this.command = this.connection.CreateCommand();
                this.command.CommandText = text;
                this.command.ExecuteNonQuery();
                result = true;
            }
            catch (Exception ex)
            {
                if (SQLiteEventListener.onErrorInvoker != null)
                {
                    SQLiteEventListener.onErrorInvoker("Unable to create table.\n" + ex.Message);
                }
                result = false;
            }
            finally
            {
                this.command.Dispose();
                this.connection.Close();
            }
            return result;
        }

        public bool DeleteTable(string tableName)
        {
            if (!this.Exists)
            {
                if (SQLiteEventListener.onErrorInvoker != null)
                {
                    SQLiteEventListener.onErrorInvoker("Database does not exists!");
                }
                return false;
            }
            if (!this.IsTableExists(tableName))
            {
                if (SQLiteEventListener.onErrorInvoker != null)
                {
                    SQLiteEventListener.onErrorInvoker("Table does not exists.");
                }
                return false;
            }
            bool result = false;
            try
            {
                string commandText = "drop table " + tableName;
                this.connection.Open();
                this.command = this.connection.CreateCommand();
                this.command.CommandText = commandText;
                this.command.ExecuteNonQuery();
                result = true;
            }
            catch (Exception ex)
            {
                if (SQLiteEventListener.onErrorInvoker != null)
                {
                    SQLiteEventListener.onErrorInvoker("Unable to delete table.\n" + ex.Message);
                }
                result = false;
            }
            finally
            {
                this.command.Dispose();
                this.connection.Close();
            }
            return result;
        }

        public int ClearTable(string tableName)
        {
            int result = -1;
            if (!this.Exists)
            {
                if (SQLiteEventListener.onErrorInvoker != null)
                {
                    SQLiteEventListener.onErrorInvoker("Database does not exists!");
                }
                return -1;
            }
            if (!this.IsTableExists(tableName))
            {
                if (SQLiteEventListener.onErrorInvoker != null)
                {
                    SQLiteEventListener.onErrorInvoker("Table does not exists.");
                }
                return -1;
            }
            try
            {
                string commandText = "delete from " + tableName;
                this.connection.Open();
                this.command = this.connection.CreateCommand();
                this.command.CommandText = commandText;
                result = this.command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                if (SQLiteEventListener.onErrorInvoker != null)
                {
                    SQLiteEventListener.onErrorInvoker("Unable to clear table.\n" + ex.Message);
                }
            }
            finally
            {
                this.command.Dispose();
                this.connection.Close();
            }
            return result;
        }

        public bool DeleteDatabase()
        {
            if (!this.Exists)
            {
                if (SQLiteEventListener.onErrorInvoker != null)
                {
                    SQLiteEventListener.onErrorInvoker("Database does not exists!");
                }
                return false;
            }
            bool result = false;
            try
            {
                string path = Path.Combine(this._dbPath, this._dbName);
                if (File.Exists(path))
                {
                    this.Dispose();
                    File.Delete(path);
                    result = true;
                }
                else
                {
                    if (SQLiteEventListener.onErrorInvoker != null)
                    {
                        SQLiteEventListener.onErrorInvoker("Database does not exists!");
                    }
                    result = false;
                }
            }
            catch (Exception ex)
            {
                if (SQLiteEventListener.onErrorInvoker != null)
                {
                    SQLiteEventListener.onErrorInvoker("Unable to delete database.\n" + ex.Message);
                }
                result = false;
            }
            finally
            {
            }
            return result;
        }

        public int Insert(string tableName, List<SQLiteDB.DB_DataPair> dataList)
        {
            if (!this.Exists)
            {
                if (SQLiteEventListener.onErrorInvoker != null)
                {
                    SQLiteEventListener.onErrorInvoker("Database does not exists!");
                }
                return -1;
            }
            if (!this.IsTableExists(tableName))
            {
                if (SQLiteEventListener.onErrorInvoker != null)
                {
                    SQLiteEventListener.onErrorInvoker("Table does not exists!");
                }
                return -1;
            }
            int result = -1;
            try
            {
                string str = "insert into " + tableName + "(";
                string text = " values('";
                for (int i = 0; i < dataList.Count; i++)
                {
                    SQLiteDB.DB_DataPair dB_DataPair = dataList[i];
                    if (i == dataList.Count - 1)
                    {
                        str = str + dB_DataPair.fieldName + ") ";
                        text = text + dB_DataPair.value + "')";
                    }
                    else
                    {
                        str = str + dB_DataPair.fieldName + ",";
                        text = text + dB_DataPair.value + "','";
                    }
                }
                string commandText = str + text;
                this.connection.Open();
                this.command = this.connection.CreateCommand();
                this.command.CommandText = commandText;
                result = this.command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                if (SQLiteEventListener.onErrorInvoker != null)
                {
                    SQLiteEventListener.onErrorInvoker("Unable to insert.\n" + ex.Message);
                }
            }
            finally
            {
                this.command.Dispose();
                this.connection.Close();
            }
            return result;
        }

        public int Insert(string query)
        {
            if (!this.Exists)
            {
                if (SQLiteEventListener.onErrorInvoker != null)
                {
                    SQLiteEventListener.onErrorInvoker("Database does not exists!");
                }
                return -1;
            }
            int result = -1;
            try
            {
                this.connection.Open();
                this.command = this.connection.CreateCommand();
                this.command.CommandText = query;
                result = this.command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                if (SQLiteEventListener.onErrorInvoker != null)
                {
                    SQLiteEventListener.onErrorInvoker("Unable to insert.\n" + ex.Message);
                }
            }
            finally
            {
                this.command.Dispose();
                this.connection.Close();
            }
            return result;
        }

        public int ExecuteNonQuery(string query)
        {
            if (!this.Exists)
            {
                if (SQLiteEventListener.onErrorInvoker != null)
                {
                    SQLiteEventListener.onErrorInvoker("Database does not exists!");
                }
                return -1;
            }
            int result = -1;
            try
            {
                this.connection.Open();
                this.command = this.connection.CreateCommand();
                this.command.CommandText = query;
                result = this.command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                if (SQLiteEventListener.onErrorInvoker != null)
                {
                    SQLiteEventListener.onErrorInvoker("Unable to execute query.\n" + ex.Message);
                }
            }
            finally
            {
                this.command.Dispose();
                this.connection.Close();
            }
            return result;
        }

        public int Update(string tableName, List<SQLiteDB.DB_DataPair> dataList, SQLiteDB.DB_ConditionPair condition)
        {
            if (!this.Exists)
            {
                if (SQLiteEventListener.onErrorInvoker != null)
                {
                    SQLiteEventListener.onErrorInvoker("Database does not exists!");
                }
                return -1;
            }
            if (!this.IsTableExists(tableName))
            {
                if (SQLiteEventListener.onErrorInvoker != null)
                {
                    SQLiteEventListener.onErrorInvoker("Table does not exists!");
                }
                return -1;
            }
            if (string.IsNullOrEmpty(condition.fieldName.Trim()) || string.IsNullOrEmpty(condition.value.Trim()))
            {
                if (SQLiteEventListener.onErrorInvoker != null)
                {
                    SQLiteEventListener.onErrorInvoker("Wrong condition!");
                }
                return -1;
            }
            int result = -1;
            try
            {
                string text = "update " + tableName + " set ";
                for (int i = 0; i < dataList.Count; i++)
                {
                    if (i == dataList.Count - 1)
                    {
                        string text2 = text;
                        text = string.Concat(new string[]
                        {
                            text2,
                            dataList[i].fieldName,
                            "='",
                            dataList[i].value,
                            "' where ",
                            condition.fieldName,
                            this.GetConditionSymbol(condition.condition),
                            "'",
                            condition.value,
                            "'"
                        });
                    }
                    else
                    {
                        string text2 = text;
                        text = string.Concat(new string[]
                        {
                            text2,
                            dataList[i].fieldName,
                            "='",
                            dataList[i].value,
                            "',"
                        });
                    }
                }
                this.connection.Open();
                this.command = this.connection.CreateCommand();
                this.command.CommandText = text;
                result = this.command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                if (SQLiteEventListener.onErrorInvoker != null)
                {
                    SQLiteEventListener.onErrorInvoker("Unable to update.\n" + ex.Message);
                }
            }
            finally
            {
                this.command.Dispose();
                this.connection.Close();
            }
            return result;
        }

        public int Update(string query)
        {
            if (!this.Exists)
            {
                if (SQLiteEventListener.onErrorInvoker != null)
                {
                    SQLiteEventListener.onErrorInvoker("Database does not exists!");
                }
                return -1;
            }
            int result = -1;
            try
            {
                this.connection.Open();
                this.command = this.connection.CreateCommand();
                this.command.CommandText = query;
                result = this.command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                if (SQLiteEventListener.onErrorInvoker != null)
                {
                    SQLiteEventListener.onErrorInvoker("Unable to update.\n" + ex.Message);
                }
            }
            finally
            {
                this.command.Dispose();
                this.connection.Close();
            }
            return result;
        }

        public int DeleteRow(string tableName, SQLiteDB.DB_ConditionPair condition)
        {
            if (!this.Exists)
            {
                if (SQLiteEventListener.onErrorInvoker != null)
                {
                    SQLiteEventListener.onErrorInvoker("Database does not exists!");
                }
                return -1;
            }
            if (!this.IsTableExists(tableName))
            {
                if (SQLiteEventListener.onErrorInvoker != null)
                {
                    SQLiteEventListener.onErrorInvoker("Table does not exists!");
                }
                return -1;
            }
            if (string.IsNullOrEmpty(condition.fieldName.Trim()) || string.IsNullOrEmpty(condition.value.Trim()))
            {
                if (SQLiteEventListener.onErrorInvoker != null)
                {
                    SQLiteEventListener.onErrorInvoker("Wrong condition!");
                }
                return -1;
            }
            int result = -1;
            try
            {
                string commandText = string.Concat(new string[]
                {
                    "delete from ",
                    tableName,
                    " where ",
                    condition.fieldName,
                    this.GetConditionSymbol(condition.condition),
                    "'",
                    condition.value,
                    "'"
                });
                this.connection.Open();
                this.command = this.connection.CreateCommand();
                this.command.CommandText = commandText;
                result = this.command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                if (SQLiteEventListener.onErrorInvoker != null)
                {
                    SQLiteEventListener.onErrorInvoker("Unable to delete row.\n" + ex.Message);
                }
            }
            finally
            {
                this.command.Dispose();
                this.connection.Close();
            }
            return result;
        }

        private string GetConditionSymbol(SQLiteDB.DB_Condition id)
        {
            string result = string.Empty;
            switch (id)
            {
                case SQLiteDB.DB_Condition.LESS_THAN:
                    result = "<";
                    break;
                case SQLiteDB.DB_Condition.GREATER_THAN:
                    result = ">";
                    break;
                case SQLiteDB.DB_Condition.EQUAL_TO:
                    result = "=";
                    break;
            }
            return result;
        }

        private string GetDataType(SQLiteDB.DB_DataType id)
        {
            string result = string.Empty;
            switch (id)
            {
                case SQLiteDB.DB_DataType.DB_INT:
                    result = "INT";
                    break;
                case SQLiteDB.DB_DataType.DB_STRING:
                    result = "STRING";
                    break;
                case SQLiteDB.DB_DataType.DB_TEXT:
                    result = "TEXT";
                    break;
                case SQLiteDB.DB_DataType.DB_VARCHAR:
                    result = "VARCHAR";
                    break;
                case SQLiteDB.DB_DataType.DB_FLOAT:
                    result = "REAL";
                    break;
            }
            return result;
        }

        public DBReader ExecuteReader(string query)
        {
            //		    var tableName = GetTalbeName(query);
            DBReader result = null;
            //		    if (false == string.IsNullOrEmpty(tableName))
            //		    {
            //		        result = App.Make<HKDatabaseReaderCache>().GetReader(tableName);
            //		    }
            //
            //		    if (null == result)
            //		    {
            //		        App.Make<HKDatabaseReaderCache>().AddTalbeReader(tableName, result);
            //            }

            using (new TimerWatch("sqlite search data", TimerProfileType.Profile))
            {
                result = this.Select(query);
            }
            return result;
        }


        private string GetTalbeName(string _query)
        {
            if (true == string.IsNullOrEmpty(_query))
            {
                return null;
            }
            string[] spliteStrs = _query.Split(' ');
            for (int index = 0; index < spliteStrs.Length; index++)
            {
                var str = spliteStrs[index];
                if (str == "from")
                {
                    return spliteStrs[index + 1];
                }
            }
            return null;
        }

        public DBReader Select(string query)
        {
            if (!this.Exists)
            {
                if (SQLiteEventListener.onErrorInvoker != null)
                {
                    SQLiteEventListener.onErrorInvoker("Database does not exists!");
                }
                return null;
            }
            DBReader result = null;
            try
            {
                this.connection.Open();
                this.command = this.connection.CreateCommand();
                this.command.CommandText = query;
                this.reader = this.command.ExecuteReader();
                result = new DBReader(this.reader);
            }
            catch (Exception ex)
            {
                if (SQLiteEventListener.onErrorInvoker != null)
                {
                    SQLiteEventListener.onErrorInvoker("Unable to select data from table.\n" + ex.Message + "\tquery = " + query);
                }
            }
            finally
            {
                this.command.Dispose();
                this.connection.Close();
            }
            return result;
        }

        public DBReader Select(string tableName, List<string> fields, SQLiteDB.DB_ConditionPair condition)
        {
            if (!this.Exists)
            {
                if (SQLiteEventListener.onErrorInvoker != null)
                {
                    SQLiteEventListener.onErrorInvoker("Database does not exists!");
                }
                return null;
            }
            if (!this.IsTableExists(tableName))
            {
                if (SQLiteEventListener.onErrorInvoker != null)
                {
                    SQLiteEventListener.onErrorInvoker("Table does not exists!");
                }
                return null;
            }
            if (string.IsNullOrEmpty(condition.fieldName.Trim()) || string.IsNullOrEmpty(condition.value.Trim()))
            {
                if (SQLiteEventListener.onErrorInvoker != null)
                {
                    SQLiteEventListener.onErrorInvoker("Wrong condition!");
                }
                return null;
            }
            DBReader result = null;
            try
            {
                string text = "select ";
                for (int i = 0; i < fields.Count; i++)
                {
                    if (i == fields.Count - 1)
                    {
                        text += fields[i];
                    }
                    else
                    {
                        text = text + fields[i] + ",";
                    }
                }
                text = text + " from " + tableName;
                string text2 = text;
                text = string.Concat(new string[]
                {
                    text2,
                    " where ",
                    condition.fieldName,
                    this.GetConditionSymbol(condition.condition),
                    "'",
                    condition.value,
                    "'"
                });
                this.connection.Open();
                this.command = this.connection.CreateCommand();
                this.command.CommandText = text;
                this.reader = this.command.ExecuteReader();
                result = new DBReader(this.reader);
            }
            catch (Exception ex)
            {
                if (SQLiteEventListener.onErrorInvoker != null)
                {
                    SQLiteEventListener.onErrorInvoker("Unable to select data.\n" + ex.Message);
                }
            }
            finally
            {
                this.command.Dispose();
                this.connection.Close();
            }
            return result;
        }

        public DBReader GetAllData(string tableName)
        {
            if (!this.IsTableExists(tableName))
            {
                if (SQLiteEventListener.onErrorInvoker != null)
                {
                    SQLiteEventListener.onErrorInvoker("Table does not exists!");
                }
                return null;
            }
            string query = "select * from " + tableName;
            return this.Select(query);
        }

        public bool IsTableExists(string tableName)
        {
            bool result = false;
            string commandText = "SELECT name FROM sqlite_master WHERE type='table' AND name='" + tableName + "'";
            try
            {
                this.connection.Open();
                this.command = this.connection.CreateCommand();
                this.command.CommandText = commandText;
                this.reader = this.command.ExecuteReader();
                result = this.reader.HasRows;
                this.reader.Close();
            }
            catch
            {
                result = false;
            }
            finally
            {
                this.command.Dispose();
                this.connection.Close();
            }
            return result;
        }

        public object ExecuteScalar(string query)
        {
            if (!this.Exists)
            {
                if (SQLiteEventListener.onErrorInvoker != null)
                {
                    SQLiteEventListener.onErrorInvoker("Database does not exists!");
                }
                return -1;
            }
            object result = null;
            try
            {
                this.connection.Open();
                this.command = this.connection.CreateCommand();
                this.command.CommandText = query;
                result = this.command.ExecuteScalar();
            }
            catch (Exception ex)
            {
                if (SQLiteEventListener.onErrorInvoker != null)
                {
                    SQLiteEventListener.onErrorInvoker("Unable to execute query.\n" + ex.Message);
                }
            }
            finally
            {
                this.command.Dispose();
                this.connection.Close();
            }
            return result;
        }

        public void Dispose()
        {
            if (this.connection != null)
            {
                this.connection.Dispose();
            }
            if (this.command != null)
            {
                this.command.Dispose();
            }
            this.connection = null;
            this.reader = null;
            this.command = null;
            SQLiteDB._instance = null;
            GC.Collect();
        }
    }
}
