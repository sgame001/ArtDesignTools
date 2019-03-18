// // ================================================================
// // FileName:HKSpriteManager.cs
// // User: Baron-Fisher
// // CreateTime:2017 0910 22:47
// // Description:挂载在gameobject上面，用于标记内存池名字
// // Copyright (c) 2016 Greg.Co.Ltd. All rights reserved.
// // ================================================================

using UnityEngine;

namespace HKLibrary
{
    [DisallowMultipleComponent]
    public class ItemEntity : MonoBehaviour, IPoolEntity
    {
        /// <summary>
        /// 所在的pool名称
        /// 这个主要是用来在Inpsector中显示
        /// </summary>
        public string parentGroupName = "";

        public string ParentGroupName
        {
            get { return parentGroupName; }
            set { parentGroupName = value; }
        }

        /// <summary>
        /// 缓存池
        /// </summary>
        public Transform cacheTransform;

        /// <summary>
        /// 标记是否要被删除
        /// 如果此标记为true, 在Destroy的时候,则不会执行回收内存池相关的事件
        /// </summary>
        public bool IsDestroyTag { get; set; }

        /// <summary>
        /// 最近一次被回收的时间
        /// 这个值会和过期时间做对比
        /// 单位是 秒
        /// </summary>
        public float LastRecoverTime { get; set; }

        void Awake()
        {
            cacheTransform = transform;
        }

        /// <summary>
        /// 获取Free对象后的事件
        /// </summary>
        public void PoolInitEvent()
        {
        }

        /// <summary>
        /// 回收到内存池之前触发的事件
        /// </summary>
        public void PoolRecoverEvent()
        {
            IsDestroyTag = false;
        }

        /// <summary>
        /// 对象销毁之前触发的事件
        /// </summary>
        public void PoolDisposeEvent()
        {
        }


        /// <summary>
        /// 被销毁时会调用到这里
        /// </summary>
        private void OnDestroy()
        {
            if (false == IsDestroyTag && false == HKCommonDefine.IsApplicationQuit) // 如果为false,说是被动销毁
            {
                this.Error(StringCacheFactory.GetFree().Add("非法销毁 = ")
                    .Add(gameObject.name)
                    .Add(" parent name = ").Add(transform.parent.name).Add(" group name = ").Add(parentGroupName));
            }
        }
    }
}