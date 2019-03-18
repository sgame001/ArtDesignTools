using System;
using System.Collections.Generic;

namespace SQLiteDatabase
{
	public class DBSchema
	{
		private string _name = string.Empty;

		private List<SQLiteDB.DB_Field> fieldList = new List<SQLiteDB.DB_Field>();

		public string TableName
		{
			get
			{
				return this._name;
			}
		}

		public DBSchema(string tableName)
		{
			this._name = tableName.Trim();
		}

		public void AddField(string fieldName, SQLiteDB.DB_DataType fieldType, int size, bool isNotNull, bool isPrimaryKey, bool isUnique)
		{
			SQLiteDB.DB_Field item = default(SQLiteDB.DB_Field);
			item.name = fieldName;
			item.type = fieldType;
			item.isNotNull = isNotNull;
			item.isPrimaryKey = isPrimaryKey;
			item.isUnique = isUnique;
			item.size = size;
			this.fieldList.Add(item);
		}

		public List<SQLiteDB.DB_Field> GetTableFields()
		{
			return this.fieldList;
		}
	}
}
