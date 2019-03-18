// // ================================================================
// // FileName:StringListCacche.cs
// // User: Baron-Fisher
// // CreateTime:2017 0807 18:10
// // Description:字符串组合缓存, 由List和StringBuilder组合而成,重复使用 减少new的成本
// // Copyright (c) 2017 Greg.Co.Ltd. All rights reserved.
// // ================================================================

using System.Collections.Generic;
using System.Text;

namespace HKLibrary
{
    public class StringListCacche
    {
        /// <summary>
        /// 内容队列
        /// </summary>
        private List<object> cacheList = new List<object>();

        /// <summary>
        /// stringBuffer
        /// </summary>
        private StringBuilder sp = new StringBuilder();

        /// <summary>
        /// 分隔符
        /// </summary>
        private string split = "";

        /// <summary>
        /// 添加数据
        /// </summary>
        /// <param name="_obj"></param>
        /// <returns></returns>
        public StringListCacche Add(object _obj)
        {
            if (null != _obj)
            {
                cacheList.Add(_obj);
            }
            return this;
        }

        /// <summary>
        /// 添加分隔符
        /// </summary>
        /// <param name="_split"></param>
        /// <returns></returns>
        public StringListCacche SetSplit(string _split)
        {
            if (false == string.IsNullOrEmpty(_split))
            {
                split = _split;
            }
            return this;
        }


        /// <summary>
        /// Release
        /// </summary>
        /// <returns></returns>
        public string Release()
        {
            if (null != cacheList && cacheList.Count > 0)
            {
                for (int index = 0; index < cacheList.Count; index++)
                {
                    if (index > 0 && false == string.IsNullOrEmpty(split))
                    {
                        sp.Append(split);
                    }
                    var str = cacheList[index];
                    sp.Append(str);
                }
                string result = sp.ToString();
                StringCacheFactory.Recover(this);
                return result;
            }
            return "";
        }

        /// <summary>
        ///  清空缓存
        /// </summary>
        /// <returns></returns>
        public void Clear()
        {
            cacheList.Clear();
            sp.Remove(0, sp.Length);
        }

    }
}