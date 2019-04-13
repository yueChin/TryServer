using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Common;
using UnityEngine;

namespace Assets.Net
{
    /// <summary>
    /// 网络事件拓展
    /// </summary>
    public static class ExtendHandler
    {
        /// <summary>
        /// 封装消息发送
        /// </summary>
        /// <param name="type">一级协议</param>
        /// <param name="commond">二级协议</param>
        /// <param name="msg">三级内容</param>
        public static void SendMsg(byte type, int commond, object msg)
        {
            GameApp.Instance.NetMsgUtil.NetIO.write(type,commond,msg);
        }

        /// <summary>
        /// 继承mono的消息发送
        /// </summary>
        /// <param name="mono">继承的mono本身</param>
        /// <param name="type">一级协议</param>
        /// <param name="commond">二级协议</param>
        /// <param name="msg">三级内容</param>
        public static void Write(this MonoBehaviour mono,byte type, int commond, object msg)
        {
            SendMsg(type,commond, msg);
        }

        /// <summary>
        /// 封装连接网络
        /// </summary>
        /// <returns></returns>
        public static void Connect()
        {
            GameApp.Instance.NetMsgUtil.NetIO.ConnnectToSever();
        }

        /// <summary>
        /// 封装关闭网络
        /// </summary>
        /// <returns></returns>
        public static bool Colse()
        {
            return GameApp.Instance.NetMsgUtil.NetIO.CloseSocket();
        }

        /// <summary>
        /// 封装是否重新链接
        /// </summary>
        /// <returns></returns>
        public static bool IsReconnect()
        {
            return GameApp.Instance.NetMsgUtil.NetIO.IsReconnect;
        }
    }
}
