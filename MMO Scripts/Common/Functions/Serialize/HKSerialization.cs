// // // // ================================================================
// // // // FileName:HKSerialization.cs
// // // // User: Heeking
// // // // CreateTime: 2016  03 08 12:00 
// // // // Description: 针对5.3x的版本，对Json进行扩展，支持List 以及 Dictionary
// // // // Copyright (c) 2015 Heeking.Co.Ltd. All rights reserved.
// // // // ================================================================
using UnityEngine;
using System.Collections.Generic;
using System;
// List<T>
[Serializable]
public class HKSerialization<T>
{
    [SerializeField]
    List<T> target;
    public List<T> ToList() { return target; }
    public HKSerialization(List<T> target)
    {
        this.target = target;
    }
}
// Dictionary<TKey, TValue>

[Serializable]
public class HKSerialization<TKey, TValue> : ISerializationCallbackReceiver
{
    [SerializeField]
    List<TKey> keys;
    [SerializeField]
    List<TValue> values;
    Dictionary<TKey, TValue> target;
    public Dictionary<TKey, TValue> ToDictionary()
    {



        return target;
    }
    public HKSerialization(Dictionary<TKey, TValue> target)
    {
        this.target = target;
    }
    public void OnBeforeSerialize()
    {


        keys = new List<TKey>(target.Keys);
        values = new List<TValue>(target.Values);
    }
    public void OnAfterDeserialize()
    {


        var count = Math.Min(keys.Count, values.Count);
        target = new Dictionary<TKey, TValue>(count);
        for (var i = 0; i < count; ++i)
        {
            target.Add(keys[i], values[i]);
        }
    }
}
//// List<T> -> Json文字列 ( 例 : List<Enemy> )
//string str = JsonUtility.ToJson(new Serialization<Enemy>(enemies)); // 出力例 : {"target":[{"name":"スライム","skills":["攻撃"]},{"name":"キングスライム","skills":["攻撃","回復"]}]}
//// Json文字列 -> List<T>
//List<Enemy> enemies = JsonUtility.FromJson<Serialization<Enemy>>(str).ToList();
//
//// Dictionary<TKey,TValue> -> Json文字列 ( 例 : Dictionary<int, Enemy> )
//string str = JsonUtility.ToJson(new Serialization<int, Enemy>(enemies)); // 出力例 : {"keys":[1000,2000],"values":[{"name":"スライム","skills":["攻撃"]},{"name":"キングスライム","skills":["攻撃","回復"]}]}
//// Json文字列 -> Dictionary<TKey,TValue>
//Dictionary<int, Enemy> enemies = JsonUtility.FromJson<Serialization<int, Enemy>>(str).ToDictionary();
