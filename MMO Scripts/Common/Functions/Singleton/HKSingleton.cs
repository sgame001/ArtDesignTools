// ================================================================
// FileName:HKSingleton.cs
// User: Greg
// CreateTime:2014033010:21
// Description: 单例实现接口类
// Copyright (c) 2014 Greg.Co.Ltd. All rights reserved.
// ================================================================

using UnityEngine;

public class HKSingleton<T> where T : MonoBehaviour
{
    private static T _instance;

    public static T GetInstance()
    {
        if (null == _instance)
        {
            _instance = Object.FindObjectOfType(typeof(T)) as T;
            if (null != _instance) return _instance;

            GameObject container = new GameObject
            {
                name = typeof(T).ToString()
            };
            _instance = container.AddComponent(typeof(T)) as T;

            Object.DontDestroyOnLoad(container); // 场景切换时不销毁
        }
        return _instance;
    }

}


public abstract class HKSingletonMonoBehaviour<T> : MonoBehaviour where T : MonoBehaviour
{
    public static T Instance
    {
        get { return HKSingleton<T>.GetInstance(); }
    }

    public static T GetInstance()
    {
        return HKSingleton<T>.GetInstance();
    }
    
    public virtual void Dispose()
    {
        
    }
}

public abstract class HKSingletonInst<T> where T :new()
{
    private static T _instance;
    public static T GetInstance()
    {
        if (_instance == null)
        {
            _instance = new T();
        }
        return _instance;
    }
}