using System;
using System.Runtime.CompilerServices;

namespace SQLiteDatabase
{
	public class SQLiteEventListener
	{
		public delegate void OnError(string err);

		public static SQLiteEventListener.OnError onErrorInvoker;

		public static event SQLiteEventListener.OnError onError
		{
			[MethodImpl(MethodImplOptions.Synchronized)]
			add
			{
				SQLiteEventListener.AddHandler_onError(value);
			}
			[MethodImpl(MethodImplOptions.Synchronized)]
			remove
			{
				SQLiteEventListener.RemoveHandler_onError(value);
			}
		}

		[MethodImpl(MethodImplOptions.Synchronized)]
		private static void AddHandler_onError(SQLiteEventListener.OnError value)
		{
			SQLiteEventListener.onErrorInvoker = (SQLiteEventListener.OnError)Delegate.Combine(SQLiteEventListener.onErrorInvoker, value);
		}

		[MethodImpl(MethodImplOptions.Synchronized)]
		private static void RemoveHandler_onError(SQLiteEventListener.OnError value)
		{
			SQLiteEventListener.onErrorInvoker = (SQLiteEventListener.OnError)Delegate.Remove(SQLiteEventListener.onErrorInvoker, value);
		}
	}
}
