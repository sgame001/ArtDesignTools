// // ================================================================
// // FileName:HKSpriteManager.cs
// // User: Baron-Fisher
// // CreateTime:2017 1002 17:52
// // Description:
// // Copyright (c) 2016 Greg.Co.Ltd. All rights reserved.
// // ================================================================

namespace HKLibrary
{
    public static class HKStringExtends
    {
        /// <summary>
        /// 获取字符串缓存的cache
        /// </summary>
        /// <param name="_str"></param>
        /// <returns></returns>
        public static StringListCacche GetCache(this string _str)
        {
            return StringCacheFactory.GetFree().Add(_str);
        }


        /// <summary>
        /// Fmt
        /// </summary>
        /// <param name="_source"></param>
        /// <param name="_args"></param>
        /// <returns></returns>
        public static string Fmt(this string _source, params object[] _args)
        {
            if (true == string.IsNullOrEmpty(_source) || null == _args || _args.Length <= 0)
            {
                return _source;
            }
            
            return string.Format(_source, _args);
        }
    }
}