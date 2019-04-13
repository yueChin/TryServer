using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using Assets.Common;
using ClientNetFrame;
using GameProtocol;
using GameProtocol.Model.Login;
using UnityEditor.Experimental.U2D;
using UnityEngine;

namespace Assets.Net
{
    /// <summary>
    /// 处理服务器二级消息
    /// </summary>
    public class LoginHandler : MonoBehaviour, IHandler
    {
        public void MessageReceive(SocketModel model)
        {
            switch (model.command)
            {
                //处理服务器返回登录的结果
                case LoginProtocol.Login_SRes:
                {
                    int status = model.GetMessage<int>();
                    switch (status)
                    {
                        case 0:
                            GameApp.Instance.CommonHintDlg.OpenHint("登录成功");
                            LoginReceive();
                            break;
                        case -1:
                            GameApp.Instance.CommonHintDlg.OpenHint("请求错误");
                            break;
                        case -2:
                            GameApp.Instance.CommonHintDlg.OpenHint("请求不合法");
                            break;
                        case -3:
                            GameApp.Instance.CommonHintDlg.OpenHint("没有此帐号");
                            break;
                        case 4:
                            GameApp.Instance.CommonHintDlg.OpenHint("密码错误");
                            break;
                        case -5:
                            GameApp.Instance.CommonHintDlg.OpenHint("帐号已登录");
                            break;
                        }
                }
                break;
                //处理快速注册请求的结果
                case LoginProtocol.QuickReg_SRes:
                {
                    ResponseRegisterModel rrm = model.GetMessage<ResponseRegisterModel>();
                    if (rrm == null||rrm.Status != 0)
                    {
                        GameApp.Instance.CommonHintDlg.OpenHint("注册失败");
                        return;
                    }
                    Debug.Log("注册成功,密码是："+ rrm.Password);
                    LoginReceive();//请求信息
                }
                break;
            }
        }

        /// <summary>
        /// 请求登录后的信息
        /// </summary>
        void LoginReceive()
        {
            this.Write(GameProtocol.TypeProtocol.User,GameProtocol.UserProtocol.GetInfo_CReq,null);
        }
    }
}

