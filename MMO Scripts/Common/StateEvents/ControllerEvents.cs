// // ================================================================
// // FileName:ControllerEvents.cs
// // User: Baron
// // CreateTime:2/28/2018
// // Description: Contrller对应的事件节点
// // ================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 事件类型
/// </summary>
public enum AnimEventType
{    
    Enter = 0,
    On,
    Exit
}


/// <summary>
/// 事件参数类型
/// </summary>
public enum AnimEventParamsType
{
    Normal, // 普通事件，显示全部参数
    Effect,
    Audio,
    ShakeScreen, // 震屏
}

/// <summary>
/// 动作事件类型
/// </summary>
public enum AnimEventLoopType
{
    Once, // 触发一次
    Loop, // 循环触发
}


/// <summary>
/// Controller 包含多个动作
/// </summary>
public class ControllerEvents : ScriptableObject
{
    /// <summary>
    /// controller 名称
    /// </summary>
    public string controllerName;

    /// <summary>
    /// controller 路径
    /// </summary>
    public string controllerPath;
    
    /// <summary>
    /// controlle id
    /// </summary>
    public int InstanceId;

    /// <summary>
    /// 动作事件集合
    /// </summary>
    public List<AnimationStateEvents> StateEvents = new List<AnimationStateEvents>();

    /// <summary>
    /// 用于显示成enum格式
    /// </summary>
    public List<string> ControllerFullNames = new List<string>();
    
}


/// <summary>
/// 单个动作包含的事件
/// </summary>
[System.Serializable]
public class AnimationStateEvents
{
    /// <summary>
    /// 动作全名 layer.state
    /// </summary>
    public string AnimationFullName;

    /// <summary>
    /// state hash
    /// </summary>
    public int AnimationStateHash;

    /// <summary>
    /// state tag
    /// </summary>
    public string StateTag;

    /// <summary>
    /// 进入状态的事件
    /// </summary>
    public AnimEventItem EnterEvent = new AnimEventItem();
    
    /// <summary>
    /// 动作上的事件队列
    /// </summary>
    public List<AnimEventItem> AnimEventItems = new List<AnimEventItem>();
}


/// <summary>
/// 动作事件
/// </summary>
[System.Serializable]
public class AnimEventItem
{
    /// <summary>
    /// 当前所处State的全名
    /// 要通过这个字符串拼接事件监听
    /// </summary>
    public string AnimatorStateFullName;
    
    /// <summary>
    /// 事件名字
    /// </summary>
    public string EventName;

    /// <summary>
    /// state tag
    /// </summary>
    public string StateTag;
    
    /// <summary>
    /// 事件的时间节点
    /// </summary>
    public float EventNormalizedTime;
    
    /// <summary>
    /// 动作事件
    /// </summary>
    public AnimEventType EventType;

    /// <summary>
    /// 循环事件类型
    /// </summary>
    public AnimEventLoopType LoopType;
    
    /// <summary>
    /// 参数队列
    /// </summary>
    public List<AnimParamItem> ParamItems = new List<AnimParamItem>();
    
}

/// <summary>
/// 事件参数对象
/// </summary>
[System.Serializable]
public class AnimParamItem
{
    /// <summary>
    /// 参数类型
    /// </summary>
    public AnimEventParamsType ParamType;
    
    /// <summary>
    /// int类型参数
    /// </summary>
    public int IntParam;
    
    /// <summary>
    /// float类型
    /// </summary>
    public float FloatParam;

    /// <summary>
    /// string类型
    /// </summary>
    public string StringParam;

    /// <summary>
    /// bool类型
    /// </summary>
    public bool BoolParam;
}