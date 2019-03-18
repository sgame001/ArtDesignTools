using System;
using System.Collections.Generic;
using System.Linq;
using CatLib;
using UnityEngine;

namespace HKLibrary
{
    public class AppSingleton : IHKSingletonManager
    {
        /// <summary>
        /// 单例缓存
        /// </summary>
        private readonly Dictionary<Type, object> SingletonDic = new Dictionary<Type, object>();

        /// <summary>
        /// 加载所有的单例对象
        /// </summary>
        public void LoadSingletonType(Type[] _types/**由外部传入的的类对象*/)
        {
            List<Type> AllTypes = new List<Type>(); // 统计所有的类型

            var firstAssemblyTypes = typeof(AppSingleton).Assembly.GetTypes();
            AllTypes.AddRange(firstAssemblyTypes);

            if (null != _types && _types.Length > 0)
            {
                AllTypes.AddRange(_types);
            }
            for (var index = 0; index < AllTypes.Count; index++)
            {
                var type = AllTypes[index];
                var nameSpace = type.Namespace;
                if (nameSpace != "HKLibrary" && nameSpace != "GameCoreLib")
                {
                    continue;
                }
                var attributes = type.GetCustomAttributes(typeof(SingletonTagAttribute), false);
                if (attributes.Length > 0)
                {
                    object instance = null;
                    if (true == SingletonDic.ContainsKey(type))
                    {
                        this.Error("重复注册单例类型 = " + type.FullName);
                        continue;
                    }

                    if (true == type.IsSubclassOf(typeof(MonoBehaviour)))
                    {
                        this.Error("Monobehaviour的子类不能以此方式实现单例");
                        continue;
                    }

                    instance = Activator.CreateInstance(type);
                    if (true == instance is IInit)
                    {
                        ((IInit)instance).Init();
                    }
                    
                    SingletonDic.Add(type, instance);
                    App.Instance(type.FullName, instance);
                }
            }
            this.Info(string.Format("单例对象，总计{0}个", SingletonDic.Count));
        }
    }
}