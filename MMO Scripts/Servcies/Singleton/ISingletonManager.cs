using System;

namespace HKLibrary
{
    public interface IHKSingletonManager
    {
        /// <summary>
        /// 加载指定的Type
        /// </summary>
        void LoadSingletonType(Type[] _types);
    }
}