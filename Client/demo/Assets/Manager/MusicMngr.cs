using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Common;
using Assets.Util;
using UnityEngine;
using LitJson;

namespace Assets.Manager
{
     public class MusicMngr:MonoBehaviour
    {
       

        private void Awake()
        {
            GameApp.Instance.MusicMngr = this;
        }

        private AudioSource mBgm;
        public AudioSource BgmSource
        {
            get
            {
                if (mBgm == null)
                {
                    if (gameObject.GetComponent<AudioSource>())
                        mBgm = gameObject.GetComponent<AudioSource>();
                }
                else
                {
                    mBgm = gameObject.AddComponent<AudioSource>();
                }

                return mBgm;
            }
        }

        private bool mIsPlayAudioBgm = true;
        private bool mIsPlayAudioEff = true;

        List<AudioSource> EffectSource = new List<AudioSource>();

        Dictionary<string, AudioClip> ClipChche = new Dictionary<string, AudioClip>();

        public AudioClip LoadClip(string path)
        {
            AudioClip clip;
            if (ClipChche.TryGetValue(path, out clip))
                return ClipChche[path];
            clip = GameApp.Instance.ResourceMngr.LoadAudioClip(name);
            ClipChche.Add(path,clip);
            return clip;
        }

        public void PlayBgm(string name)
        {
            AudioClip clip = LoadClip(name);
            if (clip == null)
                return;
            BgmSource.clip = clip;
            BgmSource.loop = true;
            BgmSource.volume = 1;
            if(mIsPlayAudioBgm)
                BgmSource.Play();
        }

        #region 背景音乐        
        /// <summary>
        /// 播放背景音乐
        /// </summary>
        public void ShutDownBgm()
        {
            mBgm.Stop();;
        }

        public void SetPlayBgmAudio(bool isPlay)
        {
            mIsPlayAudioBgm = isPlay;
            if (!isPlay)
            {
                ShutDownBgm();
            }
            else if(mBgm.clip != null)
            {
                mBgm.Play();
            }

            string str = "{\"music\":{" +
                         "\"bgm\":" + (mIsPlayAudioBgm ? 1 : 0) + "," + 
                         "\"effet\":" +(mIsPlayAudioBgm ? 1 : 0)+ "}}";
            FileUtil.CreateFile(Application.persistentDataPath,"musicdata.txt",str);
        }

        /// <summary>
        /// 关闭背景音乐
        /// </summary>
        public void PlayBgm()
        {
            mBgm.Stop();
        }
        #endregion

        #region 音效      
        /// <summary>
        /// 播放音效
        /// </summary>
        /// <param name="name"></param>
        public void PlayAudioEffect(string name)
        {
            if (!mIsPlayAudioEff) return;;
                AudioClip clip = LoadClip(name);
            if (clip == null)
                return;
            AudioSource source = GetAudioSource();
            source.clip = clip;
            source.loop = false;          
            source.Play();
        }

        public void SetEffectPlayAudio(bool isplay)
        {
            mIsPlayAudioEff = isplay;
            string str = "{\"music\":{" +
                         "\"bgm\":" + (mIsPlayAudioBgm ? 1 : 0) + "," +
                         "\"effet\":" + (mIsPlayAudioBgm ? 1 : 0) + "}}";
            FileUtil.CreateFile(Application.persistentDataPath, "musicdata.txt", str);
        }

        /// <summary>
        /// 关闭音效
        /// </summary>
        /// <param name="name"></param>
        public void ShutDownAudioEffect(string name)
        {
            for (int i = 0; i < EffectSource.Count; i++)
            {
                //找到与传入进来的音效名称一直的源
                if (EffectSource[i].clip.name == name)
                {
                    EffectSource[i].Stop();
                    return;;
                }
            }
        }

        AudioSource GetAudioSource()
        {
            for (int i = 0; i < EffectSource.Count; i++)
            {
                //获取一个未播放的源
                if (!EffectSource[i].isPlaying)
                {
                    return EffectSource[i];
                }
            }
            //如果没有获取到，就创建一个新的
            AudioSource source = gameObject.AddComponent<AudioSource>();
            source.volume = 1;
            source.loop = false;
            EffectSource.Add(source);
            return source;
        }
        #endregion

        #region 数据持久化
        /// <summary>
        /// 初始化加载
        /// </summary>
        public void InitLoad()
        {
            string path = Application.persistentDataPath;
            string data = FileUtil.LoadFile(path, "data.txt");
            JsonData jd = JsonMapper.ToObject(data);
            int bgmIsPlay = (int) jd["music"]["bgm"];
            int effIsPlay = (int) jd["music"]["effect"];
            mIsPlayAudioBgm = (bgmIsPlay == 0 ? false : true);
            mIsPlayAudioEff = (effIsPlay == 0 ? false : true);
        }


        #endregion
    }
}
