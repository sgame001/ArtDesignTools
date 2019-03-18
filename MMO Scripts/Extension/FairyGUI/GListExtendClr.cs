// // ================================================================
// // FileName:GListExtend.cs
// // User: Baron-Fisher
// // CreateTime:2018 0104 23:35
// // Description:
// // Copyright (c) 2016 Greg.Co.Ltd. All rights reserved.
// // ================================================================

using System;
using System.Collections.Generic;
using FairyGUI;

namespace HKLibrary
{
    public static class GListExtendClr
    {
        /// <summary>
        /// 安全移除所有list中对象
        /// 封装了对list内button事件的移除
        /// </summary>
        /// <param name="_list"></param>
        /// <param name="_callback"></param>
        public static void SafeRemoveChildrenToPool(this GList _list, Action<object> _callback = null)
        {
            if (null == _list)
            {
                return;
            }

            // 清空list的中button子对象的事件
            var children = _list.GetChildren();
            if (null != children && children.Length > 0)
            {
                for (int index = 0; index < children.Length; index++)
                {
                    var child = children[index];
                    if (null == child)
                    {
                        continue;
                    }

                    if (null != _callback)
                    {
                        _callback(child.data);
                    }

                    child.data = null;
                    
                    if (true == child is GButton)
                    {
                        child.RemoveEventListeners();
                    }
                }
            }
            _list.RemoveChildrenToPool();
        }
    }
}