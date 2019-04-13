using System.Collections;
using System.Collections.Generic;
using Assets.Common;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.UI
{
    public class UI_Logo : MonoBehaviour
    {
        void Start()
        {
            //测试用
            //GameApp.Instance.TimeMngr.AddSchedule(() => { Debug.Log("测试成功"); }, 5000);
            //transform.Find("Button").GetComponent<Button>().onClick.AddListener(() => { });
            GameApp.Instance.TimeMngr.AddSchedule(() =>
            {
                GameApp.Instance.GameLevelMngr.LoadScene(GameResource.SceneName.Login);
            },1000);
           
        }
    }


}
