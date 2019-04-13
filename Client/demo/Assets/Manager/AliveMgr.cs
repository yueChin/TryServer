using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Common;
using Assets.Net;
using GameProtocol;
using GameProtocol.Model.Alive;
using UnityEngine;

namespace Assets.Manager
{
    public class AliveMgr:MonoBehaviour
    {
        public AliveModel AliveModel;
        public long SeverTime;
        void Awake()
        {
            GameApp.Instance.AliveMgr = this;
        }

        void Start()
        {
            StartAlive();
        }

        void StartAlive()
        {
            //如果是第一次，那么新建一个包
            if (AliveModel == null)
                AliveModel = new AliveModel()
                {
                    Statue = -1,
                };
            //更新时间
            AliveModel.Time = DateTime.Now.Ticks;
            //发送心跳包
            this.Write(TypeProtocol.Alive, ALiveProtocol.Alive_Creq, AliveModel);
            CheckTime();
            GameApp.Instance.TimeMngr.AddSchedule(() =>
            {              
                //伪循环
                StartAlive();
            }, 3000);
        }
        /// <summary>
        /// 确认是否断连
        /// </summary>
        void CheckTime()
        {
            if (AliveModel == null) return;
            if(AliveModel.Time - SeverTime > 30000)
                GameApp.Instance.CommonHintDlg.OpenHint("您已断开连接");
        }
        /// <summary>
        /// 连接自救
        /// </summary>
        public void LivingSelf()
        {
            //如果是第一次，那么新建一个包
            if (AliveModel == null)
                AliveModel = new AliveModel()
                {
                    Statue = -1,
                };
            //更新时间
            AliveModel.Time = DateTime.Now.Ticks;
            //发送心跳包
            this.Write(TypeProtocol.Alive, ALiveProtocol.Alive_Creq, AliveModel);
        }
    }
}
