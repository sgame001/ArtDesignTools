// // ================================================================
// // FileName:UILayerTypeAtribute.cs
// // User: Baron
// // CreateTime:12/26/2017
// // Description: Layer Type
// // ================================================================

using System;

namespace HKLibrary.UI
{
    public class UILayerTypeAttribute : Attribute
    {
        /// <summary>
        /// 层信息
        /// </summary>
        public UIDef.ShowLayer ShowLayer;

        /// <summary>
        /// 关闭后是否销毁
        /// true = 销毁 ，调用关闭后立即销毁窗口
        /// false = 关闭后会进入缓存池
        /// </summary>
        public bool IsJustDestroy = false;

        /// <summary>
        /// 隐藏其他窗口
        /// 这个字段只有在Middle中才会有作用
        /// Middle层的Full Screen窗口，如果添加了这个标记
        /// 则会隐藏，middle, bottom, bottomest三个层的所有窗口
        /// </summary>
        public UIDef.ShowLayer HideLayer;

        /// <summary>
        /// 全屏界面
        /// </summary>
        public bool IsFullScreen = false;

        public UILayerTypeAttribute(UIDef.ShowLayer _layer, bool _isFullScreen = false, UIDef.ShowLayer _hideLayer = UIDef.ShowLayer.None, bool _isJustDestroy = false)
        {
            ShowLayer     = _layer;
            IsFullScreen  = _isFullScreen;
            HideLayer     = _hideLayer;
            IsJustDestroy = _isJustDestroy;
        }
    }
}