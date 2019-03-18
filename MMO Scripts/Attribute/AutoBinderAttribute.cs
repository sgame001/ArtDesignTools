// // ================================================================
// // FileName:AutoBinderAttribute.cs
// // User: Baron
// // CreateTime:3/1/2018
// // Description: Binder的标签，用于处理接口和实现之间的绑定
// // ================================================================

using System;

namespace HKLibrary
{
    public class AutoBinderAttribute : Attribute
    {
        /// <summary>
        /// 绑定的接口名字
        /// </summary>
        public string InterfaceTypeName;
        
        /// <summary>
        /// 是否是单例模式
        /// </summary>
        public bool IsSingle;

        
        public AutoBinderAttribute(string _interfaceName, bool _isSingle = false)
        {
            InterfaceTypeName = _interfaceName;
            IsSingle = _isSingle;
        }
    }
}