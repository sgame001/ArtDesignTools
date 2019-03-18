using System;
using UnityEngine;

namespace HKLibrary
{
    public class HKConfigManager : IConfigManager
    {
        /// <summary>
        /// 设置一个配置
        /// </summary>
        /// <param name="_key"></param>
        /// <param name="_value"></param>
        public void Set(string _key, object _value)
        {
            if (true == string.IsNullOrEmpty(_key))
            {
                this.Warr("空key无法设置属性");
                return;
            }

            if (_value is int)
            {
                PlayerPrefs.SetInt(_key, Convert.ToInt32(_value));
            }
            else if (_value is float)
            {
                PlayerPrefs.SetFloat(_key, Convert.ToSingle(_value));
            }
            else
            {
                PlayerPrefs.SetString(_key, _value.ToString());
            }
        }

        public T Get<T>(string _key)
        {
            if (true == string.IsNullOrEmpty(_key))
            {
                this.Warr("空key无法获取属性");
                return default(T);
            }

            if (false == PlayerPrefs.HasKey(_key))
            {
                return default(T);
            }

            object value = null;
            if (typeof(T) == typeof(int))
            {
                value = PlayerPrefs.GetInt(_key);
                return (T) value;
            }
            else if (typeof(T) == typeof(float))
            {
                value = PlayerPrefs.GetFloat(_key);
                return (T) value;
            }
            else if (typeof(T) == typeof(string))
            {
                value = PlayerPrefs.GetString(_key);
                return (T) value;
            }
            else if (typeof(T) == typeof(bool))
            {
                value = PlayerPrefs.GetString(_key);
                value = Convert.ToBoolean(value);
                return (T) value;
            }
            return default(T);
        }

        public T Get<T>(string _key, T _defaultValue)
        {
            if (true == string.IsNullOrEmpty(_key) || false == PlayerPrefs.HasKey(_key))
            {
                return _defaultValue;
            }
            return Get<T>(_key);
        }
    }
}