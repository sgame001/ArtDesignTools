using System;
using System.Net.Mail;

namespace HKLibrary
{
    /// <summary>
    /// 属性标签
    /// </summary>
    public class DynamicPropertyAttribute : Attribute
    {
        /// <summary>
        /// 属性类型
        /// </summary>
        public string PropertyType;

        /// <summary>
        /// 注释
        /// </summary>
        public string Note;

        public DynamicPropertyAttribute(string _type, string _note)
        {
            PropertyType = _type;
            Note = _note;
        }
    }
}