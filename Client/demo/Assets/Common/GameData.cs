using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Common;
using UnityEngine;

namespace Assets.Manager
{
    /// <summary>
    /// 程序运行是本地数据的存储
    /// </summary>
    public class GameData
    {
        private static GameData mInstance;

        public static GameData Instance
        {
            get
            {
                return mInstance == null ? mInstance = new GameData() : mInstance;
            }
        }

        public bool IsDebugWrite = true;

        /// <summary>
        /// 游戏当前名称
        /// </summary>
        public string GameLevelName = "Start";
        /// <summary>
        /// 预制体缓存
        /// </summary>
        public Dictionary<string,GameObject> GODict = new Dictionary<string, GameObject>();

        /// <summary>
        /// 场景缓存
        /// </summary>
        public Dictionary<GameResource.SceneName,string> SceneNameDict = new Dictionary<GameResource.SceneName, string>();

        public Dictionary<GameResource.CanvasTag,string> CanvasDict = new Dictionary<GameResource.CanvasTag, string>();

        public Dictionary<GameResource.SystemUIType,string> SystemDict = new Dictionary<GameResource.SystemUIType, string>();

        public Dictionary<GameResource.ItemTag,string> ItemDict = new Dictionary<GameResource.ItemTag, string>();
    }
}
