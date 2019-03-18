// // // ================================================================
// // // FileName:HKILModuleComponent.cs
// // // User: Baron
// // // CreateTime:2017-09-08-18:26
// // // Description:ILModule Component的需求实现
// // // 
// // // Copyright (c) 2016 Heeking.Co.Ltd. All rights reserved.
// // // ================================================================

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using FairyGUI;
using ILRuntime.CLR.Method;
using ILRuntime.CLR.TypeSystem;
using ILRuntime.Runtime.Intepreter;
using UnityEngine;
using AppDomain = ILRuntime.Runtime.Enviorment.AppDomain;
using Object = System.Object;

namespace HKLibrary
{
    public class HKILModuleComponent : IILRuntimeComponent
    {


        #region 类型声明部分，防止IL2CPP裁剪

        public List<ILTypeInstance> lst101;
        public List<UIPackage> list102;
        public List<PackageItem> list103;
        public List<string> list104;
        public List<Controller> list105;
        public List<int> list106;
        public List<GameObject> list107;
        public List<Object> list108;
        public List<SkinnedMeshRenderer> list109;
        public List<ParticleSystem> list110;
        public List<ushort> list111;
        public List<AudioObject> list116;
        public List<short> list118;
        public List<GButton> list119;
        public List<GLabel> list120;
        public List<GTextField> list121;
        public List<float> list122;
        public List<GComponent> list123;
        public List<KeyValuePair<int, int>> list124;
        private Stack<ILTypeInstance> list125;
        
        public Dictionary<string, int> dic200;
        public Dictionary<int, ILTypeInstance> dic201;
        public Dictionary<string, Stack<ILTypeInstance>> dic203;
        public Dictionary<string, string> dic204;
        public Dictionary<string, ILTypeInstance> dic205;
        public Dictionary<int, int> dic208;
        public Dictionary<byte, List<int>> dic209;
        public Dictionary<short, string> dic210;
        public IDictionary<short, string> dic211;
        public Dictionary<string, GameObject> dic212;
        public Dictionary<string, SkinnedMeshRenderer> dic213;
        public Dictionary<byte, Dictionary<short, string>> dic214;
        public Dictionary<string, Object> dic215;
        public Dictionary<byte, string> dic216;
        public Dictionary<int, GButton> dic217;
        public Dictionary<int, List<ILTypeInstance>> dic218;
        public Dictionary<byte, ILTypeInstance> dic219;
        public Dictionary<GButton, int> dic220;
        public Dictionary<int, float> dic221;
        public Dictionary<int, List<int>> dic222;
        public Dictionary<string, double> dic223;
        public Dictionary<int, Dictionary<string, double>> dic224;
        public Dictionary<int, double> dic255;
        public Dictionary<int, double> dic226;
        public Dictionary<string, GLabel> dic227;
        public Dictionary<byte, Dictionary<int, Dictionary<string, double>>> dic228;
        public Dictionary<byte, Dictionary<int, double>> dic229;
        public Dictionary<string, byte> dic230;
        public Dictionary<KeyValuePair<int, int>, int> dic231;
        public Dictionary<int, string> dic232;
        public Dictionary<int, string> dic233;
        public Dictionary<int, short> dic234;
        public Dictionary<byte, int> dic235;
        public Dictionary<uint, Emoji> dic236;
        public Dictionary<int, bool> dic237;
        public Dictionary<Type, Object> dic238;

        private Dictionary<Type, ILTypeInstance> dic239;

        private Dictionary<Type, Stack<ILTypeInstance>> dic242;
        private Dictionary<int, List<string>> dic244;
        private Dictionary<int, Dictionary<int, List<string>>> dic245;
        
         
        // 泛型
//        private UniRx.IObservable<System.Int32> gen01;
//
//        private UniRx.IObservable<UniRx.Unit> gen02;
//
//        private UniRx.IObservable<System.Boolean> gen03;
//        private UniRx.IObservable<System.Int64> gen04;
//        private UniRx.ObservableYieldInstruction<UniRx.Unit> gen05;
//        private UniRx.IObservable<System.Int32[]> gen06;
//        private UniRx.IObserver<System.Int32> gen07;
        
        #endregion
        
        /// <summary>
        /// ilrt环境对象
        /// </summary>
        public AppDomain ILAppDomain { get; set; }

        /// <summary>
        /// 注册过的类型对象
        /// </summary>
        public static Dictionary<string, int> registerList = new Dictionary<string, int>();

        /// <summary>
        /// 所有的热更类型
        /// </summary>
        private readonly List<IType> AllHotfixTypes = new List<IType>();
        
        /// <summary>
        /// 初始化方法
        /// </summary>
        public void Init()
        {
            if (null == ILAppDomain)
            {
                ILAppDomain = new AppDomain();
            }

            RegisterAdapter();
            
            // 注册Json配置
            LitJson.JsonMapper.RegisterILRuntimeCLRRedirection(ILAppDomain);
            
            // 加上这句话，关闭deep profile 可以看到具体消耗
            #if UNITY_EDITOR
            ILAppDomain.UnityMainThreadID = System.Threading.Thread.CurrentThread.ManagedThreadId;
            #endif
        }

        /// <summary>
        /// 注册委托以及事件,处理IL和C#之间的回调
        /// </summary>
        public void RegisterAdapter()
        {
            if (null != ILAppDomain)
            {
#if UNITY_EDITOR
                if (s_genericTypes == null)
                {
                    s_genericTypes = new List<Type>();

                    Type tGeneric = typeof(HKILModuleComponent);
                    System.Reflection.FieldInfo[] infos = tGeneric.GetFields(
                        System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic |
                        System.Reflection.BindingFlags.Instance);
                    foreach (System.Reflection.FieldInfo info in infos)
                    {
                        s_genericTypes.Add(info.FieldType);
                    }
                }
#endif


                #region 注册Adapater

                ILAppDomain.RegisterCrossBindingAdaptor(new GButtonAdaptor());
                ILAppDomain.RegisterCrossBindingAdaptor(new GComponentAdaptor());
                ILAppDomain.RegisterCrossBindingAdaptor(new GComboBoxAdapter());
                ILAppDomain.RegisterCrossBindingAdaptor(new GLabelAdapter());
                ILAppDomain.RegisterCrossBindingAdaptor(new GSliderAdapter());
                ILAppDomain.RegisterCrossBindingAdaptor(new GProgressBarAdapter());
                ILAppDomain.RegisterCrossBindingAdaptor(new CoroutineAdapter());

                #endregion
                
                
                #region 类型的注册

                // 加入默认的delegate
                ILAppDomain.DelegateManager.RegisterMethodDelegate<Object>();
                ILAppDomain.DelegateManager.RegisterMethodDelegate<EventContext>();
                ILAppDomain.DelegateManager.RegisterMethodDelegate<float>();
                ILAppDomain.DelegateManager.RegisterMethodDelegate<int>();
                ILAppDomain.DelegateManager.RegisterMethodDelegate<ILTypeInstance>();
                ILAppDomain.DelegateManager
                    .RegisterFunctionDelegate<CatLib.IContainer, Object[], Object>();
                
                #region list 排序

                ILAppDomain.DelegateManager.RegisterFunctionDelegate<ILTypeInstance, ILTypeInstance, int>();
                ILAppDomain.DelegateManager.RegisterDelegateConvertor<Comparison<ILTypeInstance>>((action) =>
                {
                    return new Comparison<ILTypeInstance>((a, b) =>
                    {
                        return ((Func<ILTypeInstance, ILTypeInstance, int>) action)(a, b);
                    });
                });


                ILAppDomain.DelegateManager.RegisterFunctionDelegate<int, int, int>();
                ILAppDomain.DelegateManager.RegisterDelegateConvertor<Comparison<int>>((action) =>
                {
                    return new Comparison<int>((a, b) => { return ((Func<int, int, int>) action)(a, b); });
                });

                #endregion

                ILAppDomain.DelegateManager.RegisterDelegateConvertor<PlayCompleteCallback>(
                    (_action) => { return new PlayCompleteCallback(delegate { ((Action) _action)(); }); }
                );


                ILAppDomain.DelegateManager.RegisterDelegateConvertor<EventCallback1>((_action) =>
                {
                    return new EventCallback1(delegate(EventContext context)
                    {
                        ((Action<EventContext>) _action)(context);
                    });
                });


                ILAppDomain.DelegateManager.RegisterDelegateConvertor<EventCallback0>((_action) =>
                {
                    return new EventCallback0(delegate { ((Action) _action)(); });
                });

                ILAppDomain.DelegateManager.RegisterMethodDelegate<Int32, GObject>();
                ILAppDomain.DelegateManager.RegisterDelegateConvertor<ListItemRenderer>((_action) =>
                {
                    return new ListItemRenderer(delegate(Int32 index, GObject item)
                    {
                        ((Action<int, GObject>) _action)(index, item);
                    });
                });

//                ILAppDomain.DelegateManager.RegisterFunctionDelegate<UniRx.IObserver<int>, System.IDisposable>();
                ILAppDomain.DelegateManager.RegisterMethodDelegate<Int32[]>();
                ILAppDomain.DelegateManager.RegisterMethodDelegate<UnityEngine.Object>();
                ILAppDomain.DelegateManager.RegisterFunctionDelegate<Object, Object>();
                ILAppDomain.DelegateManager.RegisterFunctionDelegate<GComponent>();

                
                ILAppDomain.DelegateManager.RegisterDelegateConvertor<UIObjectFactory.GComponentCreator>((act) =>
                {
                    return new UIObjectFactory.GComponentCreator(() =>
                    {
                        return ((Func<GComponent>)act)();
                    });
                });

                ILAppDomain.DelegateManager.RegisterMethodDelegate<String>();
                ILAppDomain.DelegateManager.RegisterMethodDelegate<String, Object, Object>();
                ILAppDomain.DelegateManager.RegisterMethodDelegate<String, Object>();
                ILAppDomain.DelegateManager.RegisterDelegateConvertor<GameCoreLib.SceneMgr.SceneEneter>((act) =>
                {
                    return new GameCoreLib.SceneMgr.SceneEneter((_sceneName, _params) =>
                    {
                        ((Action<String, Object>)act)(_sceneName, _params);
                    });
                });


                ILAppDomain.DelegateManager.RegisterMethodDelegate<CatLib.API.Network.INetworkChannel>();
                ILAppDomain.DelegateManager.RegisterFunctionDelegate<GameObject, Boolean>();
                ILAppDomain.DelegateManager.RegisterMethodDelegate<AnimEventItem>();
                ILAppDomain.DelegateManager.RegisterMethodDelegate<Int32, Object>();

                
                ILAppDomain.DelegateManager.RegisterDelegateConvertor<DG.Tweening.TweenCallback>((act) =>
                {
                    return new DG.Tweening.TweenCallback(() =>
                    {
                        ((Action)act)();
                    });
                });

                ILAppDomain.DelegateManager.RegisterMethodDelegate<IResResponse>();
                ILAppDomain.DelegateManager.RegisterMethodDelegate<UIPackage>();
                ILAppDomain.DelegateManager.RegisterMethodDelegate<String, UIPackage>();
                ILAppDomain.DelegateManager.RegisterMethodDelegate<CatLib.API.Network.INetworkChannel, Exception>();
                ILAppDomain.DelegateManager.RegisterMethodDelegate<Int32, ILTypeInstance>();
                ILAppDomain.DelegateManager.RegisterMethodDelegate<GObject>();
                ILAppDomain.DelegateManager.RegisterDelegateConvertor<UIPackage.CreateObjectCallback>((act) =>
                {
                    return new UIPackage.CreateObjectCallback((result) =>
                    {
                        ((Action<GObject>)act)(result);
                    });
                });

                #endregion
            }
        }
       

        /// <summary>
        /// 调用IL中第一个静态函数
        /// </summary>
        /// <param name="_clazzFullName">类名称</param>
        /// <param name="_methodName">方法名称</param>
        /// <param name="_params">参数</param>
        /// <returns></returns>
        public object InvokeStatic(string _clazzFullName, string _methodName, params object[] _params)
        {
            if (true == string.IsNullOrEmpty(_clazzFullName))
            {
                this.Warr("执行class full name不能为空");
                return null;
            }

            if (true == string.IsNullOrEmpty(_methodName))
            {
                this.Error("执行静态函数方法名字不能为空");
                return null;
            }

            if (null != ILAppDomain)
            {
                return ILAppDomain.Invoke(_clazzFullName, _methodName, null, _params);
            }
            return null;
        }

        /// <summary>
        /// 根据类的全名,获取一个IL对象
        /// </summary>
        /// <param name="_clazzName"></param>
        /// <returns></returns>
        public ILTypeInstance GetILRTInstance(string _clazzName)
        {
            if (true == string.IsNullOrEmpty(_clazzName))
            {
                return null;
            }

            if (null != ILAppDomain)
            {
                return ILAppDomain.Instantiate(_clazzName);
            }
            return null;
        }

        
        /// <summary>
        /// 获取方法
        /// </summary>
        /// <param name="_typeName"></param>
        /// <param name="_methodName"></param>
        /// <returns></returns>
        public IMethod GetMethod(string _typeName, string _methodName)
        {
            if (true == string.IsNullOrEmpty(_typeName) || true == string.IsNullOrEmpty(_methodName))
            {
                return null;
            }

            if (null != ILAppDomain)
            {
                var method = ILAppDomain.LoadedTypes[_typeName].GetMethod(_methodName, 0);
                return method;
            }
            return null;
        }

        /// <summary>
        /// 根据类全名以及方法名称,执行一个IL对象的成员函数
        /// </summary>
        /// <param name="_clazzFullName">类全名</param>
        /// <param name="_methodName">方法名称</param>
        /// <param name="_params">参数</param>
        /// <returns></returns>
        public object Invoke(string _clazzFullName, string _methodName, params object[] _params)
        {
            if (true == string.IsNullOrEmpty(_clazzFullName))
            {
                this.Warr("执行class full name不能为空");
                return null;
            }

            if (true == string.IsNullOrEmpty(_methodName))
            {
                this.Warr("成员函数名称不能为空");
                return null;
            }

            if (null != ILAppDomain)
            {
                var instance = ILAppDomain.Instantiate(_clazzFullName); // 创建成员对象
                return Invoke(instance, _methodName, _params);
            }
            return null;
        }

        /// <summary>
        /// 根据一个IL对象以及方法名称,执行一个成员函数
        /// </summary>
        /// <param name="_instance"></param>
        /// <param name="_methodName"></param>
        /// <param name="_params"></param>
        /// <returns></returns>
        public object Invoke(ILTypeInstance _instance, string _methodName, params object[] _params)
        {
            if (null == _instance)
            {
                this.Warr("成员对象不能为空");
                return null;
            }

            if (true == string.IsNullOrEmpty(_methodName))
            {
                this.Warr("成员函数名称不能为空");
                return null;
            }

            var method = _instance.Type.GetMethod(_methodName); // 获取method方法
            return ILAppDomain.Invoke(method, _instance, _params); // 执行
        }

        /// <summary>
        /// 获取所有的ilrt类型
        /// </summary>
        /// <returns></returns>
        public Type[] GetAllILRTTypes()
        {
            return ILAppDomain.LoadedTypes.Values.Select(x => x.ReflectionType).ToArray();
        }


        #region 内部实现

#if UNITY_EDITOR

        private static List<Type> s_genericTypes = null;
        public void CheckGenericType(CLRType clrType, Type[] args)
        {
            if (clrType.IsDelegate) return;
            if (clrType.FullName.Contains("ReadOnlyCollection")) return;
            if (clrType.FullName.Contains("Enumerator")) return;
            if (clrType.FullName.Contains("IEnumerable")) return;
            if (clrType.FullName.Contains("IEqualityComparer")) return;
            if (clrType.FullName.Contains("KeyCollection")) return;
            if (clrType.FullName.Contains("ValueCollection")) return;
            if (clrType.FullName.Contains("KeyValuePair")) return;
            if (clrType.FullName.Contains("IComparer")) return;

            //检查是否注册过
            for (int i = 0, cnt = s_genericTypes.Count; i < cnt; ++i)
            {
                Type t = s_genericTypes[i];
                Type[] types = t.GetGenericArguments();
                if (types.Length != args.Length) continue;

                if (clrType.TypeForCLR.Name != t.Name) continue;

                bool isSame = true;
                for (int j = 0; j < types.Length; ++j)
                {
                    if (types[j] != args[j])
                    {
                        isSame = false;
                        break;
                    }
                }
                if (isSame) return;
            }
            //没有注册过
            StringBuilder sb = new StringBuilder();
            sb.Append(clrType.FullName + "<");
            for (int i = 0; i < args.Length; i++)
            {
                sb.Append(args[i].FullName);
                if (i != args.Length - 1) sb.Append(",");
            }
            sb.Append(">");

            if (false == registerList.ContainsKey(sb.ToString()))
            {
                string outStr = sb.ToString().Replace("`1", "");
                outStr = outStr.Replace("`2", "");
                registerList.Add(sb.ToString(), 1);
//                this.LogMessageInfo("register = ", outStr);
            }
            else
            {
                var count = registerList[sb.ToString()];
                count++;
                registerList[sb.ToString()] = count;
            }
            //            this.LogMessageInfo("register = ", sb.ToString());
            throw new Exception("没有注册的新类型 = " + sb.ToString());
        }
#endif

        #endregion
    }
}