// // ================================================================
// // FileName:HKILLogger.cs
// // User: Baron-Fisher
// // CreateTime:2017 0807 23:37
// // Description:IL中的log系统，在生产环境中，可以直接在脚本层屏蔽log，减少和c#端不必要的交互
// // Copyright (c) 2017 Greg.Co.Ltd. All rights reserved.
// // ================================================================

using System.Collections.Generic;
using System.Text;
using CatLib;
using CatLib.API.Debugger;
using ILRuntime.Runtime.Intepreter;
using UnityEditor;
using UnityEngine;
using Application = UnityEngine.Application;
using ILogger = CatLib.API.Debugger.ILogger;

namespace HKLibrary
{
    /// <summary>
    /// Debug环境
    /// </summary>
    public enum GameMakeEnv
    {
        Dev, // 开发环境
        Make //  生产环境
    }

    /// <summary>
    /// 热更新模式
    /// </summary>
    public enum GameHotfixEnv
    {
        Debug,  // debug 模式 （ 会下载并加载pdb文件）
        Release // release 模式（ 不下载也不加载pdb文件 ）
    }

    /// <summary>
    /// Tag针对LOG分类处理使用的
    /// </summary>
    public enum LogTag
    {
        None = 0, // 普通状态
        Net,
        UI,
    }


    /// <summary>
    /// 日志级别
    /// 这个和Catlib是保持一致的
    /// </summary>
    public enum LogLevels
    {
        //
        // 摘要:
        //     紧急(系统不可用)
        Emergency = 0,

        //
        // 摘要:
        //     警报(必须立即采取行动)
        Alert = 1,

        //
        // 摘要:
        //     关键（关键日志）
        Critical = 2,

        //
        // 摘要:
        //     错误
        Error = 3,

        //
        // 摘要:
        //     警告
        Warning = 4,

        //
        // 摘要:
        //     通知
        Notice = 5,

        //
        // 摘要:
        //     信息
        Info = 6,

        //
        // 摘要:
        //     调试级消息
        Debug = 7
    }


    /// <summary>
    /// 资源加载方式
    /// </summary>
    public enum ResourceLoadType
    {
        AssetBundle,
        Editor
    }

    /// <summary>
    /// logger
    /// </summary>
    public static class LoggerQ
    {
        /// <summary>
        /// 网络消息过滤
        /// </summary>
        private static Dictionary<int, bool> netLogMessageFilter =  new Dictionary<int, bool>();

        /// <summary>
        /// 热更新环境
        /// </summary>
        public static GameHotfixEnv HotfixEnv = GameHotfixEnv.Debug;

        /// <summary>
        /// 游戏生产环境
        /// </summary>
        public static GameMakeEnv GameMakeEnv = GameMakeEnv.Dev;

        /// <summary>
        /// Logger Init
        /// </summary>
        public static void Init()
        {
            LoadNetMessageConfig();
        }

        /// <summary>
        /// 加载net message config
        /// </summary>
        private static void LoadNetMessageConfig()
        {
#if UNITY_EDITOR
            var loggerQConfig = (LoggerQConfig) ResourcesMgrFacade.Instance.LoadAsset("LoggerConfigQ").Data;
            if (null != loggerQConfig)
            {
                // S->C
                for (int index = 0; index < loggerQConfig.ReceiverMesasgeItems.Count; index++)
                {
                    var item = loggerQConfig.ReceiverMesasgeItems[index];
                    if (null != item)
                    {
                        netLogMessageFilter.Add(item.MessageCode, item.IsLog);
                    }
                }

                // C->S
                for (int index = 0; index < loggerQConfig.SendMessageItemes.Count; index++)
                {
                    var item = loggerQConfig.SendMessageItemes[index];
                    if (null != item)
                    {
                        netLogMessageFilter.Add(item.MessageCode, item.IsLog);
                    }
                }
            }
#endif
        }


        /// <summary>
        /// 获取资源加载模式
        /// </summary>
        /// <returns></returns>
        public static ResourceLoadType GetResourceLoadType()
        {
            if (true == HKCommonDefine.IsMobileDevice)
            {
                return ResourceLoadType.AssetBundle;
            }
            else
            {
#if LOAD_ASSET_BUNDLE
                return ResourceLoadType.AssetBundle;
#else
                return ResourceLoadType.Editor;
#endif
            }
        }

        /// <summary>
        /// Info 有Tag参数版
        /// </summary>
        /// <param name="_object">扩展类型Type</param>
        /// <param name="_level">Log的Tag</param>
        /// <param name="_message"></param>
        public static void LogMessage(this object _object, LogLevels _level, string _message)
        {
            if (null == _message)
            {
                return;
            }

            if (GameMakeEnv == GameMakeEnv.Make && (int) _level == (int) LogLevels.Debug) // 生产环境下，不输出Debug信息
            {
                return;
            }

            string typeName = _object.GetType().Name;
            if (true == _object is ILTypeInstance)
            {
                typeName = "<color=#3299CC>ILRuntime:</color>" + ((ILTypeInstance) _object).Type.Name;
            }

            StringBuilder sp = new StringBuilder();
            sp.Append("[" + typeName + "] ==> ");
            sp.Append(_message);
            if (_level == LogLevels.Error)
            {
                UnityEngine.Debug.LogError(sp.ToString());
            }
            else
            {
                UnityEngine.Debug.Log(DoUpLogColor(sp.ToString(), _level));
            }
        }

        /// <summary>
        /// 给log修饰颜色
        /// </summary>
        /// <param name="_logMessage"></param>
        /// <param name="_level"></param>
        /// <returns></returns>
        private static string DoUpLogColor(string _logMessage, LogLevels _level)
        {
            string result = _logMessage;
            if (null == _logMessage)
            {
                result = "";
            }

            if (_level == LogLevels.Info)
            {
                result = "<color=#3299CC>" + _logMessage + "</color>";
            }
            else if (_level == LogLevels.Error)
            {
//                result = "<color=#FF0000>" + _logMessage + "</color>";
            }
            else if (_level == LogLevels.Warning)
            {
                result = "<color=#D9D919>" + _logMessage + "</color>";
            }
            else if (_level == LogLevels.Debug)
            {
                result = "<color=#B4EEB4>" + _logMessage + "</color>";
            }

            return result;
        }


        /// <summary>
        /// debug信息打印
        /// debug主要是在调试时打印
        /// 生产环境中，不会打印出来
        /// debug信息调试完成后，应该自觉屏蔽
        /// </summary>
        /// <param name="_object"></param>
        /// <param name="_message"></param>
        /// <param name="_params">附加参数，只有在Dev环境中才会存在，在上线时dev的都会被屏蔽掉，所以不会有new object[]的额外开销</param>
        public static void Debug(this object _object, string _message, params object[] _params)
        {
            var strApp = StringCacheFactory.GetFree().Add(_message);
            if (_params.Length > 0)
            {
                for (int index = 0; index < _params.Length; index++)
                {
                    strApp.Add(_params[index]);
                }
            }

            _object.LogMessage(LogLevels.Debug, strApp.Release());
        }

        /// <summary>
        /// Error
        /// 错误信息，错误信息会在生产环境中打印出来
        /// 在关键性信息加载错误时，会使用Error信息打印
        /// </summary>
        public static void Error(this object _object, StringListCacche _errorMessage)
        {
            if (null == _errorMessage)
            {
                return;
            }

            _object.LogMessage(LogLevels.Error, _errorMessage.Release());
        }

        /// <summary>
        /// 字符串重载的Error
        /// </summary>
        /// <param name="_object"></param>
        /// <param name="_errorMessage"></param>
        public static void Error(this object _object, string _errorMessage)
        {
            _object.LogMessage(LogLevels.Error, _errorMessage);
        }

        /// <summary>
        /// info信息
        /// info主要处理流程性的信息
        /// 主要的流程节点会在info中打印出来
        /// </summary>
        /// <param name="_object"></param>
        /// <param name="_message"></param>
        public static void Info(this object _object, StringListCacche _message)
        {
            if (null == _message)
            {
                return;
            }

            LogMessage(_object, LogLevels.Info, _message.Release());
        }

        /// <summary>
        /// 判断是否需要记录网络消息日志
        /// </summary>
        /// <param name="_messageCode"></param>
        /// <returns></returns>
        public static bool IsLogByMessageCode(int _messageCode)
        {
            if (true == netLogMessageFilter.ContainsKey(_messageCode))
            {
                return netLogMessageFilter[_messageCode];
            }

            return false;
        }


        /// <summary>
        /// string类型的info
        /// </summary>
        /// <param name="_object"></param>
        /// <param name="_message"></param>
        public static void Info(this object _object, string _message)
        {
            if (null == _message)
            {
                return;
            }

            LogMessage(_object, LogLevels.Info, _message);
        }

        /// <summary>
        /// warring信息
        /// 主要使用在参数检测部分
        /// 不是特别重要的参数检测出异常，使用warring进行打印
        /// 在生产环境中会被打印出来 
        /// </summary>
        /// <param name="_object"></param>
        /// <param name="_message"></param>
        public static void Warr(this object _object, StringListCacche _message)
        {
            if (null == _message)
            {
                return;
            }

            LogMessage(_object, LogLevels.Warning, _message.Release());
        }

        /// <summary>
        /// 字符串参数类型的重载
        /// </summary>
        /// <param name="_object"></param>
        /// <param name="_message"></param>
        public static void Warr(this object _object, string _message)
        {
            LogMessage(_object, LogLevels.Warning, _message);
        }
    }
}