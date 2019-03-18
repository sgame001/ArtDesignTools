// // // ================================================================
// // // FileName:IAudioComponent.cs
// // // User: Baron
// // // CreateTime:2017-09-08-11:23
// // // Description:声音组件的API接口定义
// // // 
// // // Copyright (c) 2016 Heeking.Co.Ltd. All rights reserved.
// // // ================================================================

using UnityEngine;

namespace HKLibrary
{
    public interface IAudioComponent
    {
        /// <summary>
        /// 全局音量
        /// </summary>
        float GlobleVolume { get; set; }
    
        /// <summary>
        /// 背景音效
        /// </summary>
        float BGMMusicVolume { get; set; }
    
        /// <summary>
        /// 音效音量
        /// </summary>
        float SFXVolume { get; set; }

        /// <summary>
        /// init
        /// 做一些音效的初始化操作
        /// 可能在外部调用,所以暴漏出来
        /// </summary>
        void Init(HKCommonDefine.LoadRes _loadRes);

        /// <summary>
        /// 播放背景音效
        /// </summary>
        /// <param name="_name">音乐名称</param>
        /// <param name="_vol">音量</param>
        /// <param name="_loop">是否循环</param>
        AudioObject PlayBGM(string _name, float _vol, bool _loop = true);

        /// <summary>
        /// 播放音效,可以指定具体的播放位置
        /// </summary>
        /// <param name="_name">音效名字</param>
        /// <param name="_vol">音效音量</param>
        AudioObject PlaySFX(string _name, float _vol);

        /// <summary>
        /// 根据一个绝对坐标,播放一个具体的音效
        /// </summary>
        /// <param name="_name"></param>
        /// <param name="_vol"></param>
        /// <param name="_pos"></param>
        AudioObject PlaySFXByWorldSpace(string _name, float _vol, Vector3 _pos);

        /// <summary>
        /// 释放所有音效
        /// </summary>
        void ReleaseAllAudio();

    
        /// <summary>
        /// 释放一个类别的音效
        /// </summary>
        /// <param name="_name">类别名称</param>
        void ReleaseCategoryAudio(string _name);

        /// <summary>
        /// 停止背景音乐
        /// </summary>
        void StopMusic();
        
        /// <summary>
        /// 停止播放指定id的音效
        /// </summary>
        /// <param name="_id"></param>
        void StopSFX(string _id);
        
        /// <summary>
        /// 停止所有音效
        /// </summary>
        void PauseAudio();
    }
}