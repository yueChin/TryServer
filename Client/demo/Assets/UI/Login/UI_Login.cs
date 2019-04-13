using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Common;
using Assets.Net;
using GameProtocol;
using GameProtocol.Model.Login;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.UI.Login
{
    public class UI_Login:MonoBehaviour
    {
        private Button mQuickBtn;
        private Button mLoginBtn;
        private InputField UsernameIF;

        void Awake()
        {
            GameApp.Instance.UI_Login = this;
            mQuickBtn = this.transform.Find("Panel/QuickBtn").GetComponent<Button>();
            mLoginBtn = this.transform.Find("Panel/LoginBtn").GetComponent<Button>();
            UsernameIF = this.transform.Find("Panel/Username").GetComponent<InputField>();
            OnClick();
        }

        void OnClick()
        {
            mQuickBtn.onClick.AddListener(() =>
            {
                Debug.Log("请求快速登录");
                this.Write(TypeProtocol.Login, LoginProtocol.QuickReg_CReq, null);  
            });
            mLoginBtn.onClick.AddListener(() =>
            {
                string user = UsernameIF.text;
                if (user.Length < 6) return;
                RequestLoginModel rlm = new RequestLoginModel();
                rlm.UserName = user;
                rlm.password = "Password";
                Debug.Log("请求帐号登录");
                this.Write(TypeProtocol.Login, LoginProtocol.QuickReg_SRes, rlm);                
            });
        }
    }
}
