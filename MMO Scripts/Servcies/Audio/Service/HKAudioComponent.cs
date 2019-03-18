// // // ================================================================
// // // FileName:HKAudioComponent.cs
// // // User: Baron
// // // CreateTime:2017-09-08-11:41
// // // Description: 音效实现类
// // // 
// // // Copyright (c) 2016 Heeking.Co.Ltd. All rights reserved.
// // // ================================================================

using System.Collections.Generic;
using UnityEngine;

namespace HKLibrary
{
    public class HKAudioComponent : IAudioComponent
    {
        #region CONST

        /// <summary>
        /// 全局音量
        /// </summary>
        public const string GLOBLE_VOL = "GLOBLE_VOL";

        /// <summary>
        /// 背景音量
        /// </summary>
        public const string BG_VOL = "BG_VOL";

        /// <summary>
        /// 音效音量
        /// </summary>
        public const string SFX_VOL = "SFX_VOL";

        /// <summary>
        /// 音效类别 - 背景音乐
        /// </summary>
        public const string AUDIO_CONTROLLER_BGM_CATEGORY = "BGM_CATEGORY";

        /// <summary>
        /// 音效类别 - 音效
        /// </summary>
        public const string AUDIO_CONTROLLER_SFX_CATEGORY = "SFX_CATGORY";

        /// <summary>
        /// 加载方法回调
        /// </summary>
        public HKCommonDefine.LoadRes loadResEvent;

        #endregion


        /// <summary>
        /// 音效类别队列
        /// </summary>
        public Dictionary<string, AudioCategory> audioCategories = new Dictionary<string, AudioCategory>(4);

        /// <summary>
        /// 全局音量
        /// </summary>
        private float globleVolume = 1f;

        public float GlobleVolume
        {
            get { return globleVolume; }
            set
            {
                if (globleVolume >= 0 && globleVolume <= 1)
                {
                    globleVolume = value;
                }
            }
        }

        /// <summary>
        /// 背景音效
        /// </summary>
        private float bgmMusicVolum = 1f;

        public float BGMMusicVolume
        {
            get { return bgmMusicVolum; }
            set
            {
                if (bgmMusicVolum >= 0 && bgmMusicVolum <= 1)
                {
                    bgmMusicVolum = value;
                }
            }
        }

        /// <summary>
        /// 音效音量
        /// </summary>
        private float sfxVolume = 1f;

        public float SFXVolume
        {
            get { return sfxVolume; }
            set
            {
                if (sfxVolume >= 0 && sfxVolume <= 1)
                {
                    sfxVolume = value;
                }
            }
        }

        /// <summary>
        /// init
        /// 做一些音效的初始化操作
        /// 可能在外部调用,所以暴漏出来
        /// </summary>
        public void Init(HKCommonDefine.LoadRes _loadRes)
        {
            if (null != _loadRes)
            {
                loadResEvent = _loadRes;
            }
            InitCategory();
        }

        /// <summary>
        /// 播放背景音效
        /// </summary>
        /// <param name="_name">音乐名称</param>
        /// <param name="_vol">音量</param>
        /// <param name="_loop">是否循环</param>
        public AudioObject PlayBGM(string _name, float _vol, bool _loop = true)
        {
            AudioItem audioItem = CheckAudioItem(_name, AUDIO_CONTROLLER_BGM_CATEGORY);
            if (null != audioItem)
            {
                audioItem.Loop = (true == _loop ? AudioItem.LoopMode.LoopSubitem : AudioItem.LoopMode.DoNotLoop);
                return AudioController.PlayMusic(_name, _vol);
            }
            return null;
        }


        /// <summary>
        /// 播放音效,可以指定具体的播放位置
        /// </summary>
        /// <param name="_name">音效名字</param>
        /// <param name="_vol">音效音量</param>
        public AudioObject PlaySFX(string _name, float _vol)
        {
            AudioItem audioItem = CheckAudioItem(_name, AUDIO_CONTROLLER_SFX_CATEGORY);
            if (null != audioItem)
            {
                return AudioController.Play(_name, _vol);
            }
            return null;
        }

        /// <summary>
        /// 根据一个绝对坐标,播放一个具体的音效
        /// </summary>
        /// <param name="_name"></param>
        /// <param name="_vol"></param>
        /// <param name="_pos"></param>
        public AudioObject PlaySFXByWorldSpace(string _name, float _vol, Vector3 _pos)
        {
            AudioItem audioItem = CheckAudioItem(_name, AUDIO_CONTROLLER_SFX_CATEGORY);
            if (null != audioItem)
            {
                var audioObject = AudioController.Play(_name, _pos);
                audioObject.volume = _vol;
                return audioObject;
            }
            return null;
        }

        /// <summary>
        /// 释放所有音效
        /// </summary>
        public void ReleaseAllAudio()
        {
            int count = AudioController.Instance.AudioCategories.Length;
            for (int index = 0; index < count; index++)
            {
                var category = AudioController.Instance.AudioCategories[index]; // 这里不确定instance是否可以获取到唯一单例
                if (null == category)
                {
                    continue;
                }
                category.UnloadAllAudioClips();
                AudioController.RemoveCategory(category.Name);
            }
        }

        /// <summary>
        /// 释放一个类别的音效
        /// </summary>
        /// <param name="_name">类别名称</param>
        public void ReleaseCategoryAudio(string _name)
        {
            if (true == string.IsNullOrEmpty(_name))
            {
                return;
            }
            AudioCategory category = AudioController.GetCategory(_name);
            if (null != category)
            {
                category.UnloadAllAudioClips();
                AudioController.RemoveCategory(category.Name);
            }
        }

        #region Private Method

        /// <summary>
        /// 初始化音效类别
        /// </summary>
        private void InitCategory()
        {
            // 获取背景音乐类别

            #region Patcher Body

            #endregion

            if (null == AudioController.GetCategory(AUDIO_CONTROLLER_BGM_CATEGORY))
            {
                var bgm_category = AudioController.NewCategory(AUDIO_CONTROLLER_BGM_CATEGORY);
                if (null != bgm_category)
                {
                    audioCategories.Add(AUDIO_CONTROLLER_BGM_CATEGORY, bgm_category);
                }
            }
            // 获取音效类别
            if (null == AudioController.GetCategory(AUDIO_CONTROLLER_SFX_CATEGORY))
            {
                var sfx_category = AudioController.NewCategory(AUDIO_CONTROLLER_SFX_CATEGORY);
                if (null != sfx_category)
                {
                    audioCategories.Add(AUDIO_CONTROLLER_SFX_CATEGORY, sfx_category);
                }
            }

            // 全局音量
            float globle_vol = PlayerPrefs.GetFloat(GLOBLE_VOL, 1.0f);
            SetGlobleVolume(globle_vol);

            // 背景音量
            float bg_vol = PlayerPrefs.GetFloat(BG_VOL, 1.0f);
            SetBGVolume(bg_vol);

            // 音效音量
            float sfx_vol = PlayerPrefs.GetFloat(SFX_VOL, 1.0f);
            SetSFXVolume(sfx_vol);
        }


        /// <summary>
        /// 检测音量的合法性
        /// </summary>
        /// <param name="_vol"></param>
        /// <returns></returns>
        private bool CheckVol(float _vol)
        {
            if (_vol < 0f || _vol > 1f)
                return false;
            return true;
        }


        /// <summary>
        /// 设置全局音量
        /// </summary>
        /// <param name="_vol"></param>
        public void SetGlobleVolume(float _vol)
        {
            if (false == CheckVol(_vol))
            {
                return;
            }
            GlobleVolume = _vol;
            AudioController.SetGlobalVolume(_vol);

            // 存储到本地
            PlayerPrefs.SetFloat(GLOBLE_VOL, _vol);
        }


        /// <summary>
        /// 设置背景音量
        /// </summary>
        /// <param name="_vol"></param>
        public void SetBGVolume(float _vol)
        {
            if (false == CheckVol(_vol))
            {
                return;
            }
            this.BGMMusicVolume = _vol;
            AudioCategory bgm_category = null;
            if (true == audioCategories.TryGetValue(AUDIO_CONTROLLER_BGM_CATEGORY, out bgm_category))
            {
                bgm_category.Volume = _vol;
            }
            // 存储到本地
            PlayerPrefs.SetFloat(BG_VOL, _vol);
        }


        /// <summary>
        /// 设置音效音量
        /// </summary>
        /// <param name="_vol"></param>
        public void SetSFXVolume(float _vol)
        {
            if (false == CheckVol(_vol))
            {
                return;
            }
            this.SFXVolume = _vol;
            AudioCategory sfx_category = null;
            if (true == audioCategories.TryGetValue(AUDIO_CONTROLLER_BGM_CATEGORY, out sfx_category))
            {
                sfx_category.Volume = _vol;
            }
            // 存储到本地
            PlayerPrefs.SetFloat(SFX_VOL, _vol);
        }



        /// <summary>
        /// 停止播放背景音乐
        /// </summary>
        public void StopMusic()
        {
            AudioController.StopMusic();
        }

        
        /// <summary>
        /// 停止播放音效
        /// </summary>
        /// <param name="_id"></param>
        public void StopSFX(string _id)
        {
            AudioController.Stop(_id);
        }


        /// <summary>
        /// 暂停所有声音
        /// </summary>
        public void PauseAudio()
        {
            AudioController.PauseAll();
        }
        
        
        /// <summary>
        /// 检测并返回一个Audio Item
        /// </summary>
        /// <param name="_name">音效名字</param>
        /// <param name="_category">音效类别</param>
        /// <returns></returns>
        public AudioItem CheckAudioItem(string _name, string _category)
        {
            if (true == string.IsNullOrEmpty(_name))
            {
                return null;
            }
            if (true == AudioController.IsValidAudioID(_name))
            {
                return AudioController.GetAudioItem(_name);
            }
            AudioClip audioClip = null;

            // 加载音频资源
            if (null != loadResEvent)
            {
                var audioObject = loadResEvent(_name, "audio");
                if (null != audioObject && true == audioObject is AudioClip)
                {
                    audioClip = (AudioClip) audioObject;
                }
            }
            if (null == audioClip)
            {
                this.Warr(StringCacheFactory.GetFree().Add("无法加载音频 = ").Add(_name));
                return null;
            }
            var audio_category = AudioController.GetCategory(_category);
            if (null != audio_category)
            {
                AudioItem audioItem = AudioController.AddToCategory(audio_category, audioClip, _name);
                return audioItem;
            }
            return null;
        }

        #endregion
    }
}