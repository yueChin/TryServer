using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Common;
using Assets.Model;
using Assets.UI;
using Assets.UI.Login;
using Assets.UI.Main;
using UnityEngine;

namespace Assets.Manager
{
    public class GameLevelMngr:MonoBehaviour
    {
        public Dictionary<GameResource.SystemUIType, GameObject> SystemUIGODict = new Dictionary<GameResource.SystemUIType, GameObject>();
        List<GameResource.SystemUIType> systemUIList = new List<GameResource.SystemUIType>();
        void Awake()
        {
            GameApp.Instance.GameLevelMngr = this;
        }

        void Start()
        {
            DontDestroyOnLoad(transform.parent);
            Screen.SetResolution(1920, 1080, false);
            //GameApp.Instance.TimeMngr.AddSchedule(() => { LoadScene(GameResource.SceneName.Logo); }, 10000);
        }

        public void LoadScene(GameResource.SceneName tag)
        {
            //如果当前要加载的场景和当前的一致，就返回
            if (GameData.Instance.GameLevelName == GameData.Instance.SceneNameDict[tag])
                return;
            List<LoadResourceModel> resList = new List<LoadResourceModel>();
            LoadMgr.AsyncCallback callback = null;
            switch (tag)
            {
                //Logo场景
                case GameResource.SceneName.Logo:
                {
                    LoadResourceModel model = new LoadResourceModel(GameResource.ResourceType.Prefab,
                        GameResource.UIResourcePath + GameData.Instance.CanvasDict[GameResource.CanvasTag.LogoCanvas]);
                    resList.Add(model);
                    callback = delegate()
                    {
                        //Debug.Log(GameResource.UIResourcePath + GameData.Instance.CanvasDict[GameResource.CanvasTag.LogoCanvas]);
                        GameObject go = GameApp.Instance.ResourceMngr.InstantiateGameObject(
                            GameResource.UIResourcePath +
                            GameData.Instance.CanvasDict[GameResource.CanvasTag.LogoCanvas], null, Vector3.zero);
                       
                        go.AddComponent<UI_Logo>();
                    };
                    break;
                }
                //登录场景
                case GameResource.SceneName.Login:
                {
                    LoadResourceModel model = new LoadResourceModel(GameResource.ResourceType.Prefab,
                        GameResource.UIResourcePath + GameData.Instance.CanvasDict[GameResource.CanvasTag.LoginCanvas]);
                    resList.Add(model);
                    callback = delegate()
                    {
                        GameObject go = GameApp.Instance.ResourceMngr.InstantiateGameObject(
                            GameResource.UIResourcePath +
                            GameData.Instance.CanvasDict[GameResource.CanvasTag.LoginCanvas], null, Vector3.zero);
                        go.AddComponent<UI_Login>();
                    };
                    break;
                }
                case GameResource.SceneName.Main:
                {
                    LoadResourceModel model = new LoadResourceModel(GameResource.ResourceType.Prefab,
                        GameResource.UIResourcePath + GameData.Instance.CanvasDict[GameResource.CanvasTag.MainCanvas]);
                    resList.Add(model);
                    callback = delegate ()
                    {
                        GameObject go = GameApp.Instance.ResourceMngr.InstantiateGameObject(
                            GameResource.UIResourcePath +
                            GameData.Instance.CanvasDict[GameResource.CanvasTag.MainCanvas], null, Vector3.zero);
                        go.AddComponent<UI_Main>();
                    };
                    break;
                }
                case GameResource.SceneName.Battle:
                    break;
            }

            GameData.Instance.GameLevelName = GameData.Instance.SceneNameDict[tag];
            GameApp.Instance.LoadMgr.StartLoadScene(tag,resList, callback);
        }

        public void LoadSystemUI(GameResource.SystemUIType type,LoadMgr.AsyncCallback callback = null)
        {
            Transform transform = this.transform.parent.Find("System").GetComponent<Transform>();
            //判断缓存中是否有存在对象
            GameObject go;
            if (!SystemUIGODict.TryGetValue(type, out go))
            {
                go = GameApp.Instance.ResourceMngr.InstantiateGameObject(
                    GameResource.UIResourcePath + GameData.Instance.SystemDict[type], null, Vector3.zero);
                SystemUIGODict.Add(type, go);
            }
            else
            {
                go.transform.SetAsLastSibling();
                go.SetActive(true);
            }
            for (int i = 0; i < systemUIList.Count; i++)
            {
                if (systemUIList[i] == type)
                {
                    systemUIList.RemoveAt(i);
                    break;
                }
            }
            //将队列显示放到最后一位
            systemUIList.Add(type);
        }

        public void CloseSystemUI(GameResource.SystemUIType type = GameResource.SystemUIType.Null, LoadMgr.AsyncCallback callback= null)
        {
            if(systemUIList.Count <= 0)
                return;;
            if (type == GameResource.SystemUIType.Null)
            {
                GameObject go;
                if (SystemUIGODict.TryGetValue(systemUIList[systemUIList.Count - 1], out go))
                    return;
                go.SetActive(false);
                systemUIList.RemoveAt(systemUIList.Count - 1);
            }
            else
            {
                GameObject go;
                if (!SystemUIGODict.TryGetValue(type, out go))
                    return;;
                go.SetActive(false);
                //将UI界面从队列中移除
                for (int i = 0; i < systemUIList.Count; i++)
                {
                    if (systemUIList[i] == type)
                    {
                        systemUIList.RemoveAt(i);
                        break;
                    }
                }
            }
            if (callback != null)
            {
                callback();
            }
        }
    }
}
