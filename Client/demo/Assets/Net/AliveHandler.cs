using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Common;
using ClientNetFrame;
using GameProtocol.Model.Alive;
using UnityEngine;

namespace Assets.Net
{
    public class AliveHandler :MonoBehaviour, IHandler
    {
        public void MessageReceive(SocketModel model)
        {
            AliveModel am = model.GetMessage<AliveModel>();
            if (am != null)
            {
                switch (am.Statue)
                {
                    case 0:
                        //让客户端的包时间和服务器保持一致
                        GameApp.Instance.AliveMgr.SeverTime = am.Time;
                        break;
                    case -1:
                        //重新发送
                        GameApp.Instance.AliveMgr.LivingSelf();
                        break;
                    case -2:
                        //非法或离线
                        GameApp.Instance.CommonHintDlg.OpenHint("您已断开连接");
                        break;
                }
            }
        }
    }
}
