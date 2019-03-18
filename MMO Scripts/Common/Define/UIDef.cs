using System;

namespace HKLibrary
{
    public class UIDef
    {
        [Flags]
        public enum ShowLayer
        {
            None      = 1 << 0, // 无效层
            Topest    = 1 << 1, // 最上层（目前只定义显示Loading窗口，覆盖所有窗口）
            Top       = 1 << 2, // 上层（广播类消息，全屏等待窗口， 新手引导等）
            Middle    = 1 << 3, // 功能性窗口（全屏的功能窗口，以及弹出的模态类的二级窗口）
            Bottom    = 1 << 4, // 底层窗口（掉血数字，快捷装备栏以及装备提示栏等动态显示在常驻窗口和一级窗口之间的）
            Bottomest = 1 << 5  // 最底层窗口（常驻窗口，界面上的常驻UI， 摇杆，各种活动菜单等等）
        }
    }
}