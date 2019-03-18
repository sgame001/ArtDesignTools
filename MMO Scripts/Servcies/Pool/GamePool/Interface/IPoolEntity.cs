// // ================================================================
// // FileName:HKSpriteManager.cs
// // User: Baron-Fisher
// // CreateTime:2017 0910 19:23
// // Description:Pool Item
// // Copyright (c) 2016 Greg.Co.Ltd. All rights reserved.
// // ================================================================

public interface IPoolEntity
{
    /// <summary>
    /// 所在的Group
    /// </summary>
    string ParentGroupName { get; set; }

    /// <summary>
    /// 标记是否要被删除
    /// 如果此标记为true, 在Destroy的时候,则不会执行回收内存池相关的事件
    /// </summary>
    bool IsDestroyTag { get; set; }

    /// <summary>
    /// 上一次回收的时间
    /// </summary>
    float LastRecoverTime { get; set; }
    
    /// <summary>
    /// 初始化
    /// </summary>
    void PoolInitEvent();

    /// <summary>
    /// 重置
    /// </summary>
    void PoolRecoverEvent();

    
    /// <summary>
    /// 销毁
    /// </summary>
    void PoolDisposeEvent();

}