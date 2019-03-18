using Mono.Data.Sqlite;
using System.Collections.Generic;

namespace SQLiteDatabase
{
    public class DBReader
    {
        public List<Dictionary<string, object>> dataTable;

        private short rowCounter = -1;

        public DBReader(SqliteDataReader reader)
        {
            this.dataTable = new List<Dictionary<string, object>>();
            while (reader.Read())
            {
                Dictionary<string, object> dictionary = new Dictionary<string, object>();
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    dictionary.Add(reader.GetName(i), reader.GetValue(i));
                }
                this.dataTable.Add(dictionary);
            }
            if (reader != null)
            {
                reader.Close();
            }
        }

        public bool Read()
        {
            this.rowCounter += 1;
            if ((int)this.rowCounter < this.dataTable.Count)
            {
                return true;
            }
            this.Dispose();
            //            this.rowCounter = -1; // ÖØÖÃµ½-1
            return false;
        }


        public string GetStringValue(string fieldName)
        {
            if (this.rowCounter >= 0 && this.dataTable != null && this.dataTable.Count > 0)
            {
                return this.dataTable[(int)this.rowCounter][fieldName].ToString();
            }
            if (SQLiteEventListener.onErrorInvoker != null)
            {
                SQLiteEventListener.onErrorInvoker("No record found");
            }
            return string.Empty;
        }

        public int GetIntValue(string fieldName)
        {
            if (this.rowCounter >= 0 && this.dataTable != null && this.dataTable.Count > 0)
            {
                int result = 0;
                if (int.TryParse(this.dataTable[(int)this.rowCounter][fieldName].ToString(), out result))
                {
                    return result;
                }
            }
            else if (SQLiteEventListener.onErrorInvoker != null)
            {
                SQLiteEventListener.onErrorInvoker("No record found");
            }
            return 0;
        }

        public long GetLongValue(string fieldName)
        {
            if (this.rowCounter >= 0 && this.dataTable != null && this.dataTable.Count > 0)
            {
                long result = 0L;
                if (long.TryParse(this.dataTable[(int)this.rowCounter][fieldName].ToString(), out result))
                {
                    return result;
                }
            }
            else if (SQLiteEventListener.onErrorInvoker != null)
            {
                SQLiteEventListener.onErrorInvoker("No record found");
            }
            return 0L;
        }

        public short GetShortValue(string fieldName)
        {
            if (this.rowCounter >= 0 && this.dataTable != null && this.dataTable.Count > 0)
            {
                short result = 0;
                if (short.TryParse(this.dataTable[(int)this.rowCounter][fieldName].ToString(), out result))
                {
                    return result;
                }
            }
            else if (SQLiteEventListener.onErrorInvoker != null)
            {
                SQLiteEventListener.onErrorInvoker("No record found");
            }
            return 0;
        }

        public double GetDoubleValue(string fieldName)
        {
            if (this.rowCounter >= 0 && this.dataTable != null && this.dataTable.Count > 0)
            {
                double result = 0.0;
                if (double.TryParse(this.dataTable[(int)this.rowCounter][fieldName].ToString(), out result))
                {
                    return result;
                }
            }
            else if (SQLiteEventListener.onErrorInvoker != null)
            {
                SQLiteEventListener.onErrorInvoker("No record found");
            }
            return 0.0;
        }

        public float GetFloatValue(string fieldName)
        {
            if (this.rowCounter >= 0 && this.dataTable != null && this.dataTable.Count > 0)
            {
                float result = 0f;
                if (float.TryParse(this.dataTable[(int)this.rowCounter][fieldName].ToString(), out result))
                {
                    return result;
                }
            }
            else if (SQLiteEventListener.onErrorInvoker != null)
            {
                SQLiteEventListener.onErrorInvoker("No record found");
            }
            return 0f;
        }

        public object GetValue(string fieldName)
        {
            if (this.rowCounter >= 0 && this.dataTable != null && this.dataTable.Count > 0)
            {
                return this.dataTable[(int)this.rowCounter][fieldName];
            }
            if (SQLiteEventListener.onErrorInvoker != null)
            {
                SQLiteEventListener.onErrorInvoker("No record found");
            }
            return null;
        }

        public void Dispose()
        {
            this.dataTable.Clear();
            this.dataTable.TrimExcess();
            this.dataTable = null;
            //            GC.Collect();
        }
    }
}