using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Common;
using UnityEngine;

namespace Assets.Manager
{
    public class DebugMngr:MonoBehaviour
    {
        private void Awake()
        {
            //为单例组件赋值
            GameApp.Instance.DebugMngr = this;
            //为unity事件添加监听动作
            if(GameData.Instance.IsDebugWrite)
                Application.logMessageReceived += UnityLogMsg;
        }

        private void Start()
        {
            //为Unity的Log事件添加监听
            
        }

        #region MyRegion UnityLog函数的封装     
        public void Log(object obj)
        {
            Debug.Log(obj);
        }

        public void LogWarring(object obj)
        {
            Debug.LogWarning(obj);
        }

        public void LogError(object obj)
        {
            Debug.LogError(obj);
        }
        #endregion

        //为unity输出添加一个监听事件
        private void UnityLogMsg(string LogMessage,string stack,LogType type)
        {
            switch (type)
            {
                //如果是普通日志，则只打印事件和内容
                case LogType.Log:
                    LogMessage = DateTime.Now.ToString("hh:mm:ss ") + LogMessage;
                    break;;
                //如果是错误日志，则打印错误信息和调用信息
                case LogType.Warning:
                    LogMessage = DateTime.Now.ToString("hh:mm:ss ") + LogMessage + "<" + stack + ">";
                    break;
                default:
                    break;
                    ;
            }
            Write(LogMessage,type);
        }
        //声明一个数据流对象
        private StreamWriter mStreamWriter;
        //讲输出的日志保存至本地
        private void Write(string LogMessage, LogType type)
        {
            //获取本地缓存沙盒路径
            string path = Application.persistentDataPath + "/OutLog/" + type;
            //如果文件不存在，就创建
            try
            {
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
                path += "/ "+ DateTime.Now.ToString("yyyy-mm-dd") + ".log";
                if (mStreamWriter == null)
                    mStreamWriter = !File.Exists(path) ? File.CreateText(path) : File.AppendText(path);
                mStreamWriter.WriteLine(LogMessage);
            }
            finally
            {
                if (mStreamWriter != null)
                {
                    //释放缓存区资源
                    mStreamWriter.Flush();
                    //释放占用的资源
                    mStreamWriter.Dispose();
                    mStreamWriter = null;
                }
            }

        }
    }
}
