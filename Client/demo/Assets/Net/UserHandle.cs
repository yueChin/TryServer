using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Common;
using ClientNetFrame;
using GameProtocol;
using Server.Model.User;
using UnityEngine;

namespace Assets.Net
{
    public class UserHandler : MonoBehaviour, IHandler
    {
        public void MessageReceive(SocketModel model)
        {
            //二级协议
            switch (model.command)
            {
                case UserProtocol.GetInfo_SRes:
                    {                        
                        UserModel um = model.GetMessage<UserModel>();
                        if (um != null)
                        {
                            GameApp.Instance.CommonHintDlg.OpenHint("获取用户信息成功" +um.NickName);
                            GameSession.Instance.UserModel = um;
                            GameApp.Instance.GameLevelMngr.LoadScene(GameResource.SceneName.Main);
                        }
                        else
                        {
                            GameApp.Instance.CommonHintDlg.OpenHint("获取用户信息失败");
                            ExtendHandler.Colse();
                            ExtendHandler.Connect();
                        }
                    }
                    break;
            }
        }
    }
}
