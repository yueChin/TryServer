using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Common;
using Assets.Model;
using Assets.Net;
using UnityEngine;
using ClientNetFrame;
using GameProtocol;

namespace Assets.Util
{
    public class NetMsgUtil : MonoBehaviour
    {
        public InitializeNetIO NetIO;

        #region 二级协议分发组件
        /// <summary>
        /// 登录信息组件
        /// </summary>
        private LoginHandler mLoginHandler;
        public LoginHandler LoginHandler
        {
            get
            {
                if (mLoginHandler == null)
                {
                    if (!this.transform.GetComponent<LoginHandler>())
                    {
                        mLoginHandler = this.gameObject.AddComponent<LoginHandler>();
                    }
                    else
                    {
                        mLoginHandler = this.gameObject.GetComponent<LoginHandler>();
                    }
                }
                return mLoginHandler;
            }
        }
        /// <summary>
        /// 玩家信息组件
        /// </summary>
        private UserHandler mUserHandler;
        public UserHandler UserHandler
        {
            get
            {
                if (mUserHandler == null)
                {
                    if (!this.transform.GetComponent<UserHandler>())
                    {
                        mUserHandler = this.gameObject.AddComponent<UserHandler>();
                    }
                    else
                    {
                        mUserHandler = this.gameObject.GetComponent<UserHandler>();
                    }
                }
                return mUserHandler;
            }
        }
        /// <summary>
        /// 匹配组件
        /// </summary>
        private MatchHandler mMatchHandler;
        public MatchHandler MatchHandler
        {
            get
            {
                if (mMatchHandler == null)
                {
                    if (!this.transform.GetComponent<MatchHandler>())
                    {
                        mMatchHandler = this.gameObject.AddComponent<MatchHandler>();
                    }
                    else
                    {
                        mMatchHandler = this.gameObject.GetComponent<MatchHandler>();
                    }
                }
                return mMatchHandler;
            }
        }
        //心跳组件
        private AliveHandler mAliveHandler;
        public AliveHandler AliveHandler
        {
            get
            {
                if (mAliveHandler == null)
                {
                    if (!this.transform.GetComponent<MatchHandler>())
                    {
                        mAliveHandler = this.gameObject.AddComponent<AliveHandler>();
                    }
                    else
                    {
                        mAliveHandler = this.gameObject.GetComponent<AliveHandler>();
                    }
                }
                return mAliveHandler;
            }
        }
        #endregion

        void Awake()
        {
            GameApp.Instance.NetMsgUtil = this;
            GameResource.Instance.Register();
        }

        void Start()
        {
            //初始化一个客户端网络组件
            NetIO = new InitializeNetIO();
            //为网路组件添加日志输出回调函数
            NetIO.DebugCallBack = delegate (object obj) { };
            //为网络组件添加失败回调
            NetIO.ConnectFeiledCallBack = delegate (Exception e) { };
            //添加向服务器发送消息失败回调
            NetIO.WriteFeiledCallBack = delegate (Exception e) { };
            //添加接受消息失败回调
            NetIO.ReceiveFeiledCallBack = delegate (Exception e) { };
            //为网络组件进行服务器链接初始化
            NetIO.Initialize("127.0.0.1", 6650);
            //开始连接服务器
            NetIO.ConnnectToSever();

            #region 测试用



            #endregion
            Load();
        }

        // Update is called once per frame
        void Update()
        {
            //获取客户端消息本帧缓存的个数
            while (NetIO.GetSocketMessageCount() > 0)
            {
                SocketModel model = NetIO.GetMessage();
                if (model == null)
                    continue;
                ;
                StartCoroutine("MsgRecvCallBack", model);
            }
        }

        void MsgRecvCallBack(SocketModel model)
        {
            //一级协议模块分发
            switch (model.type)
            {
                //登录模块
                case TypeProtocol.Login:
                    LoginHandler.MessageReceive(model);
                    break;
                //用户模块
                case TypeProtocol.User:
                    UserHandler.MessageReceive(model);
                    break;
                //匹配模块
                case TypeProtocol.Match:
                    MatchHandler.MessageReceive(model);
                    break; ;
                case TypeProtocol.Alive:
                    AliveHandler.MessageReceive(model);
                    break;;
            }
        }

        public void Load()
        {
            GameApp.Instance.GameLevelMngr.LoadScene(GameResource.SceneName.Logo);
        }
    }
}
