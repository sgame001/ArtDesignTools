using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStartNodeInfo {

	[System.Serializable]
	public class AStartNode
	{
		/// <summary>
		/// x坐标
		/// </summary>
		public int x;

		/// <summary>
		/// y坐标
		/// </summary>
		public int y;

		/// <summary>
		/// 是否可以行走
		/// </summary>
		public bool walkable = true;

		/// <summary>
		/// 区域 用来二次检查是否是可通过区域
		/// </summary>
		public uint area;
	}




	[System.Serializable]
	public class AStarGrid
	{
		/// <summary>
		/// 地图id
		/// </summary>
		public int map_id;

		/// <summary>
		/// 地图宽度
		/// </summary>
		public int size_width;

		/// <summary>
		/// 地图高度
		/// </summary>
		public int size_height;

		/// <summary>
		/// 原点坐标 x
		/// </summary>
		public double source_x;

		/// <summary>
		/// 原点坐标 z
		/// </summary>
		public double source_z;

		/// <summary>
		/// 路径点
		/// </summary>
		public List<AStartNode> tiles = new List<AStartNode>();

	}
}
