// // ================================================================
// // FileName:HKSpriteManager.cs
// // User: Baron-Fisher
// // CreateTime:2017 0615 9:27
// // Description:大小端的转换,Server-Java和Client-C#的大小端不同
// // Copyright (c) 2016 Greg.Co.Ltd. All rights reserved.
// // ================================================================

namespace HKLibrary
{
    public static class HKEndian
    {
        /// <summary>
        /// short
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        public static short SwapInt16(this short n)
        {
            return (short) (((n & 0xff) << 8) | ((n >> 8) & 0xff));
        }


        /// <summary>
        /// ushort
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        public static ushort SwapUInt16(this ushort n)
        {
            return (ushort) (((n & 0xff) << 8) | ((n >> 8) & 0xff));
        }

        /// <summary>
        /// int
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        public static int SwapInt32(this int n)
        {
            return (int) (((SwapInt16((short) n) & 0xffff) << 0x10) |
                          (SwapInt16((short) (n >> 0x10)) & 0xffff));
        }

        /// <summary>
        /// uint
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        public static uint SwapUInt32(this uint n)
        {
            return (uint) (((SwapUInt16((ushort) n) & 0xffff) << 0x10) |
                           (SwapUInt16((ushort) (n >> 0x10)) & 0xffff));
        }

        /// <summary>
        /// long
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        public static long SwapInt64(this long n)
        {
            return (long) (((SwapInt32((int) n) & 0xffffffffL) << 0x20) |
                           (SwapInt32((int) (n >> 0x20)) & 0xffffffffL));
        }

        /// <summary>
        /// ulong
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        public static ulong SwapUInt64(this ulong n)
        {
            return (ulong) (((SwapUInt32((uint) n) & 0xffffffffL) << 0x20) |
                            (SwapUInt32((uint) (n >> 0x20)) & 0xffffffffL));
        }
    }
}