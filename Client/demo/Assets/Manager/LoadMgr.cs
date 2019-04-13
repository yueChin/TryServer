using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Common;
using Assets.Model;
using UnityEngine;
using UnityEngine.Experimental.PlayerLoop;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Assets.Manager
{
    public class LoadMgr:MonoBehaviour
    {
        public delegate void AsyncCallback();
        /// <summary>
        /// 待加载的资源数量
        /// </summary>
        private int mLoadResNumber = 0;
        /// <summary>
        /// 资源加载进度值
        /// </summary>
        private int mResPogrsValue = 0;
        /// <summary>
        /// 加载进度条
        /// </summary>
        private Slider mPogrsSlider;
        /// <summary>
        /// 加载进度条显示
        /// </summary>
        private Text mPogrsText;
        /// <summary>
        /// 加载进度条物体
        /// </summary>
        private GameObject mPogresGO;
        /// <summary>
        /// 异步加载场景的对象
        /// </summary>
        private AsyncOperation mAsyncOperation;
        /// <summary>
        /// 场景加载进度值
        /// </summary>
        private int mScenePogrsValue;

        private const int mMaxResPogrsValue = 100;
        private bool mIsStartLoading;
        void Awake()
        {
            GameApp.Instance.LoadMgr = this;
            mPogrsSlider = this.transform.Find("GO/ValueSlider").GetComponent<Slider>();
            mPogrsText = this.transform.Find("GO/ValueText").GetComponent<Text>();
            mPogresGO = this.transform.Find("GO").gameObject;
            mPogresGO.gameObject.SetActive(false);
        }

        /// <summary>
        /// 进度条显示
        /// </summary>
        void FixedUpdate()
        {
            if (mIsStartLoading)
            {
                mPogrsSlider.value = mScenePogrsValue / 100f + mResPogrsValue / 100f;
                mPogrsText.text = "资源加载中" + mPogrsSlider.value * 100 + "%";
            }
        }

        /// <summary>
        /// 开始加载下一个场景
        /// </summary>
        /// <param name="scene"></param>
        /// <param name="loadResourceModels"></param>
        public void StartLoadScene(GameResource.SceneName scene,List<LoadResourceModel> loadResourceModels, AsyncCallback loadCallback = null)
        {
            mPogrsSlider.value = 0;
            mPogrsText.text = "正在加载中...0%";
            mLoadResNumber = 0;
            mResPogrsValue = 0;
            mScenePogrsValue = 0;
            //开始加载
            mIsStartLoading = true;
            mPogresGO.gameObject.SetActive(true);
            StartCoroutine(LoadScene(scene, loadResourceModels, loadCallback));

        }

        IEnumerator LoadScene(GameResource.SceneName scene,List<LoadResourceModel> resList,AsyncCallback loadCallback = null)
        {
            //显示用的进度
            int displaypro = 0;
            //实际的进度
            int topro = 0;
            //开始加载下一个场景
            mAsyncOperation = SceneManager.LoadSceneAsync(GameData.Instance.SceneNameDict[scene]);
            mAsyncOperation.allowSceneActivation = false;
            //进度不足90时，播放缓动动画
            while (mAsyncOperation.progress < 0.9f)
            {
                //如果我们显示的进度未到实际的进度，就增加显示的进度
                topro = (int) (mAsyncOperation.progress * 100);
                while (displaypro < topro)
                {
                    displaypro += 2;
                    mScenePogrsValue = displaypro / 2;
                    yield return new WaitForFixedUpdate();
                }
            }
            //加载最后一段进度,加载资源
            topro = 100;
            while (displaypro < topro)
            {
                displaypro += 2;
                mScenePogrsValue = displaypro / 2;
                yield return new WaitForFixedUpdate();
            }
            //加载资源
            LoadResource(resList);
            displaypro = 0;
            while (mResPogrsValue <= 100 && displaypro < mMaxResPogrsValue)
            {
                displaypro++;
                mResPogrsValue = displaypro / 2;
                yield return new WaitForFixedUpdate();
            }
            //全部加载完毕后，进入下一个场景
            mAsyncOperation.allowSceneActivation = true;
            while (!mAsyncOperation.isDone)
                yield return new FixedUpdate();
            mIsStartLoading = false;
            //全部加载完毕后，执行回调
            if (loadCallback!= null)
                loadCallback();
            //将加载页面隐藏
            mPogresGO.gameObject.SetActive(false);
        }

        void LoadResource(List<LoadResourceModel> resList)
        {
            for (int i = 0; i < resList.Count; i++)
            {
                //loadResourceModels[i].Type;
                switch (resList[i].Type)
                {
                    case GameResource.ResourceType.TextAsset:
                        GameApp.Instance.ResourceMngr.LoadText(resList[i].Path);                       
                        break;
                    case GameResource.ResourceType.Audio:
                        GameApp.Instance.MusicMngr.LoadClip(resList[i].Path);
                        break; 
                    case GameResource.ResourceType.Sprite:
                        GameApp.Instance.ResourceMngr.LoadSprite(resList[i].Path);
                        break; 
                    case GameResource.ResourceType.Prefab:
                        GameApp.Instance.ResourceMngr.LoadGameObject(resList[i].Path);
                        break; 
                }
                LoadResCallBack(resList.Count);
                if (resList.Count == 0)
                    LoadResCallBack(0);
            }
        }

        /// <summary>
        /// 加载资源回调，跟新加载数量
        /// </summary>
        /// <param name="modleCount"></param>
        void LoadResCallBack(int modleCount)
        {
            float num = 100f / modleCount;
            mLoadResNumber++;
            mResPogrsValue = (int)(mLoadResNumber * num);
        }
    }
}
