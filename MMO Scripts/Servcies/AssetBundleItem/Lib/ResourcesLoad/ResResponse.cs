// // ================================================================
// // FileName:ResResponse.cs
// // User: Baron
// // CreateTime:1/30/2018
// // Description: 资源响应返回
// // ================================================================

using HKLibrary;
using UnityEngine;

public class ResResponse : IResResponse
{
    /// <summary>
    /// 源数据名字
    /// </summary>
    public string SourceDataName { get; set; }

    /// <summary>
    /// 真正的资源对象
    /// </summary>
    public Object Data { get; set; }

    /// <summary>
    /// 引用次数
    /// </summary>
    public int ReferencesCount { get; set; }

    /// <summary>
    /// 占位资源
    /// </summary>
    public Object Holder { get; set; }

    /// <summary>
    /// 是否是Gameobject对象
    /// Gameobject对象可能会处理自动释放
    /// </summary>
    public bool IsGamobject = false;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="_object"></param>
    public ResResponse(Object _object)
    {
        if (null == _object)
        {
            return;
        }

        Data = _object;

        IsGamobject = Data is GameObject;
    }

    /// <summary>
    /// 实例化
    /// </summary>
    /// <returns></returns>
    public GameObject Instantiate()
    {
        if (null != Data && true == IsGamobject)
        {
            ReferencesCount++;
            return (GameObject) Object.Instantiate(Data);
        }

        return null;
    }

    /// <summary>
    /// 销毁一个对象
    /// </summary>
    /// <param name="_go"></param>
    public void Destroy(GameObject _go)
    {
        if (null != _go)
        {
            ReferencesCount--;
            if (ReferencesCount < 0)
            {
                ReferencesCount = 0;
            }

            Object.Destroy(_go);
        }
    }

    public void UnLoad()
    {
        AppEvent.BroadCastEvent("unload_asset", SourceDataName);
    }
}