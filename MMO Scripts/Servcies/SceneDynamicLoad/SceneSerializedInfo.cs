// // ================================================================
// // FileName:SceneSerializedInfo.cs
// // User: Baron
// // CreateTime:2018/5/21
// // Description: 序列化队列定义
// // ================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneSerializedInfo : ScriptableObject {

	/// <summary>
	/// 静态物体队列
	/// </summary>
	public List<SceneItemSerialized> ItemSerializeds = new List<SceneItemSerialized>();
}


/// <summary>
/// 序列化物体的信息
/// </summary>
[System.Serializable]
public class SceneItemSerialized
{
	/// <summary>
	/// 资源名称
	/// </summary>
	public string PrefabName;
	
	/// <summary>
	/// 位置
	/// </summary>
	public Vector3 Position;

	/// <summary>
	/// 旋转
	/// </summary>
	public Vector3 LocalEulerAngles;

	/// <summary>
	/// 缩放
	/// </summary>
	public Vector3 LocalScale;

	/// <summary>
	/// 光照信息
	/// </summary>
	public List<LightMapInfo> LightMapInfos = new List<LightMapInfo>();

	/// <summary>
	/// 是否是透贴
	/// 特效默认就是透贴范围内的
	/// </summary>
	public bool IsTransparent = false;

	/// <summary>
	/// 是否是特效对象
	/// </summary>
	public bool IsFX = false;

}

[System.Serializable]
public class LightMapInfo
{
	/// <summary>
	/// 光照贴图索引
	/// </summary>
	public int LightMapIndex;

	/// <summary>
	/// 光照信息偏移和缩放
	/// </summary>
	public Vector4 LightMapScaleOffset;
}
