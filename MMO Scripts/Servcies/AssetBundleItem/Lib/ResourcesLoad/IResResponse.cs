// // ================================================================
// // FileName:IResResponse.cs
// // User: Baron
// // CreateTime:1/30/2018
// // Description: Response interface
// // ================================================================

using UnityEngine;

public interface IResResponse
{
    
    /// <summary>
    /// 源数据名字
    /// </summary>
    string SourceDataName { get; set; }
    
    /// <summary>
    /// 真实的源数据
    /// </summary>
    Object Data { get; set; }
    
    /// <summary>
    /// 引用次数
    /// </summary>
    int ReferencesCount { get; set; }

    /// <summary>
    /// 占位资源
    /// </summary>
    Object Holder { get; set; }

    /// <summary>
    /// 实例化资源
    /// 仅限于Prefab类型的
    /// </summary>
    /// <returns></returns>
    GameObject Instantiate();

    /// <summary>
    /// 销毁资源
    /// </summary>
    void Destroy(GameObject _go);

    /// <summary>
    /// 彻底释放资源
    /// </summary>
    void UnLoad();
}