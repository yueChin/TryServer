using System;
using System.Collections.Generic;
using System.IO;
using Server.Logic;
using ServerNetFrame;
using ServerTools.Debug;
using ServerTools.File;
using ServerTools.RunHandle;
using ServerTools.Time;

namespace Server
{
    class Program
    {
        static void Main(string[] args)
        {
            //控制台功能
            ConsoleUtil consoleUtil = new ConsoleUtil();
            //注册正常关闭函数
            consoleUtil.RegisterCtrlHandle();
            //注册异常关闭监听函数
            //为当前应做作用域添加一个异常函数
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(consoleUtil.UnHandlerExceptionEventHandler);
            DebugUtil.Instance.Log2Time("控制台初始化", logType.Notice);

            //服务器端口
            int port = 6650;
            //最大客户端连接人数
            int maxClient = 1000;         
            DebugUtil.Instance.Log2Time("服务器初始化",logType.Notice);
            //初始话一个服务器通讯程序
            ServerStart server = new ServerStart(maxClient);
            DebugMessage.Debug = delegate(object obj) { DebugUtil.Instance.Log2Time(obj.ToString(),logType.Debug);};
            DebugMessage.Notice = delegate (object obj) { DebugUtil.Instance.Log2Time(obj.ToString(), logType.Notice); };
            DebugMessage.Error = delegate (object obj) { DebugUtil.Instance.Log2Time(obj.ToString(), logType.Error); };
            //初始化一个消息分发中心
            server.center = new HandleCenter();
            //启动服务器
            server.Start(port);
                
            while (true)
            {              
            }
        }
    }
}
