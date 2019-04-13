using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Manager;
using UnityEngine;

namespace Assets.Common
{
    /// <summary>
    /// 用来管理常量，变量
    /// </summary>
    public class GameResource
    {
        private static GameResource mInstance;
        public static GameResource Instance
        {
            get { return mInstance == null ? mInstance = new GameResource() : mInstance; }
        }

        /// <summary>
        /// 注册函数
        /// </summary>
        public void Register()
        {
            RegisterSceneName();
            RegisterCanvasTag();
            ResiterItemTag();
        }

        #region 场景相关
        /// <summary>
        /// 资源类型
        /// </summary>
        public enum ResourceType
        {
            TextAsset,
            Audio,
            Sprite,
            Prefab
        }

        /// <summary>
        /// 场景类型
        /// </summary>
        public enum SceneName
        {
            Start,
            Logo,
            Login,
            Main,
            Battle
        }

        /// <summary>
        /// 注册场景名称
        /// </summary>
        void RegisterSceneName()
        {
            GameData.Instance.SceneNameDict.Add(SceneName.Start,"Start");
            GameData.Instance.SceneNameDict.Add(SceneName.Login, "Login");
            GameData.Instance.SceneNameDict.Add(SceneName.Logo, "Logo");
            GameData.Instance.SceneNameDict.Add(SceneName.Main, "Main");
            GameData.Instance.SceneNameDict.Add(SceneName.Battle, "Battle");
        }
        #endregion

        #region UI相关      
        public enum CanvasTag
        {
            StartCanvas,
            LogoCanvas,
            LoginCanvas,
            MainCanvas
        }

        public enum ItemTag
        {
            HintItem,
        }

        public enum SystemUIType
        {
            Null,
            MatchPanel
        }

        /// <summary>
        /// 注册UICanvas资源名称
        /// </summary>
        void RegisterCanvasTag()
        {
            GameData.Instance.CanvasDict.Add(CanvasTag.StartCanvas, "StartCanvas");
            GameData.Instance.CanvasDict.Add(CanvasTag.LogoCanvas, "LogoCanvas");
            GameData.Instance.CanvasDict.Add(CanvasTag.LoginCanvas, "LoginCanvas");
            GameData.Instance.CanvasDict.Add(CanvasTag.MainCanvas, "MainCanvas");
        }

        void ResiterPanelTag()
        {
            GameData.Instance.SystemDict.Add(SystemUIType.MatchPanel, "System/MatchPanel");
        }

        void ResiterItemTag()
        {
            GameData.Instance.ItemDict.Add(ItemTag.HintItem, "HintItem");
        }

        #endregion
        /// <summary>
        /// UI存储路径
        /// </summary>
        public const string UIResourcePath = "Prefab/UI/";
        /// <summary>
        /// 组件资源存储路径
        /// </summary>
        public const string ItemResourcePath = "Prefab/Item/";
    }
}
