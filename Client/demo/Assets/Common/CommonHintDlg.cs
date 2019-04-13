using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Manager;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Common
{
    /// <summary>
    /// 常用提示框
    /// </summary>
    public class CommonHintDlg:MonoBehaviour
    {
        List<string> filterList = new List<string>();
        List<string> msgList = new List<string>();
        List<GameObject> cacheList = new List<GameObject>();
        void Awake()
        {
            GameApp.Instance.CommonHintDlg = this;
        }

        /// <summary>
        /// 打开提示框，显示将要显示的提示
        /// </summary>
        /// <param name="msg"></param>
        public void OpenHint(string msg)
        {
            //如果过滤列表里已经有该文本，就直接返回
            if(filterList.Contains(msg))return;
            msgList.Add(msg);
            UpdateBox();
        }

        void UpdateBox()
        {
            //如果提示缓存为空，就返回
            if (msgList.Count == 0)
                return;
            
            GameObject go;
            //提示组件的小对象池
            if (cacheList.Count == 0)
            {
                //组件路径
                string path = GameResource.ItemResourcePath + GameData.Instance.ItemDict[GameResource.ItemTag.HintItem];
                go = GameApp.Instance.ResourceMngr.InstantiateGameObject(path, this.transform, Vector3.zero);
            }
            else
            {
                go = cacheList[0];
                cacheList.Remove(go);
            }
            //重设提示框默认值
            go.SetActive(true);
            go.GetComponent<Image>().color = new Color(1,1,1,1);
            Text text = go.GetComponentInChildren<Text>();
            string msg = msgList[0];
            text.text = msg;
            filterList.Add(msg);
            StartCoroutine(CloseBox(go, msg));
            msgList.RemoveAt(0);
            //如果列表中还有，递归
            if(msgList.Count > 0)
                UpdateBox();
        }

        IEnumerator CloseBox(GameObject go,string hint)
        {
            yield return new WaitForSeconds(2.0f);
            filterList.Remove(hint);
            for (int i = 20; i > -1; i--)
            {
                //一帧之后执行后面的语句
                yield return new WaitForFixedUpdate();
                //让提示变为透明
                go.GetComponent<Image>().color = new Color(1f,1f,1f,i/20f);
            }
            go.SetActive(false);
            cacheList.Add(go);
        }
    }
}
