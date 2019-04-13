using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ServerTools.Debug;
using ServerTools.File;
using ServerTools.Time;

namespace ServerTools.RunHandle
{
    /// <summary>
    /// 管理控制台工具
    /// </summary>
    public class ConsoleUtil
    {

        public ConsoleUtil()
        {
            //获取当前程序的运行路径
            mFilePath = FileUtil.GetRunDictory();
            mFilePath += "/DebugLog/Fatal";
        }

        #region 正常关闭
        /// <summary>
        /// 关闭时间原型
        /// </summary>
        /// <param name="ctrlType">传入的事件类型：0，2，4</param>
        /// <param name="ctrlType">传入的事件类型：0用户按下Ctrl +C来关闭当前程序</param>
        /// <param name="ctrlType">传入的事件类型：2用户按下关闭按钮来关闭当前程序</param>
        /// <param name="ctrlType">传入的事件类型：4用户关机来关闭当前程序</param>
        /// <returns></returns>
        public delegate bool CtrlHandlerDelegate(int ctrlType);

        /// <summary>
        /// 导入Win32 系统核心库，声明外部导入函数SetConsoleCtrlDelegate
        /// </summary>
        /// <param name="callback"></param>
        /// <returns></returns>
        [DllImport("kernel32.dll")]
        public static extern bool SetConsoleCtrlHandler(CtrlHandlerDelegate callback, bool isAdd);

        /// <summary>
        /// 关闭程序的回调
        /// </summary>
        /// <param name="ctrlType"></param>
        /// <returns></returns>
        bool ConsoleHandler(int ctrlType)
        {
            switch (ctrlType)
            {
                //ctrl +c
                case 0:
                    //如果系统发给我们这些指令后，还希望做其他，就返回true
                    DebugUtil.Instance.Log2Time("程序强制关闭，按任意键继续");
                    ScheduleUtil.Instance.AddSchedule(() =>
                    {                       
                        DebugUtil.Instance.Close();
                    }, 1000);
                    Console.ReadKey(false);
                    return false;
                //关闭按钮
                case 2:
                    DebugUtil.Instance.Log2Time("程序强制关闭，按任意键继续");
                    ScheduleUtil.Instance.AddSchedule(() =>
                    {
                        DebugUtil.Instance.Close();
                    }, 1000);
                    Console.ReadLine();
                    return true; 
                case 4:
                    DebugUtil.Instance.Log2Time("程序强制关闭，按任意键继续");
                    ScheduleUtil.Instance.AddSchedule(() =>
                    {
                        DebugUtil.Instance.Close();
                    }, 1000);
                    Console.ReadLine();
                    return true; 
            }  
            //否则返回false
            return false;
        }

        public void RegisterCtrlHandle()
        {
            CtrlHandlerDelegate handle = new CtrlHandlerDelegate(ConsoleHandler);
            SetConsoleCtrlHandler(handle, true);
        }
        #endregion

        #region 正常关闭

        private static StreamWriter mStreamWriter;
        private string mFilePath = string.Empty;

        /// <summary>
        /// 打印捕捉到的错误日志
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public void UnHandlerExceptionEventHandler(object sender, UnhandledExceptionEventArgs args)
        {
            try
            {
                string path = mFilePath;
                //根目录不存在就创建一个目录文件夹
                FileUtil.CreateFolder(path);

                //将根目录路径和文件名组合
                path += "/" + DateTime.Now.ToString("yyyy-MM-dd") + ".log";
                //如果文件不存在，就创建一个文件，否则就加载文件，并讲对象赋值给该对象
                if (mStreamWriter == null)
                    mStreamWriter = !System.IO.File.Exists(path) ? System.IO.File.CreateText(path) : System.IO.File.AppendText(path);
                //开始写入文件
                mStreamWriter.WriteLine("/************************" + DateTime.Now.ToString("HH:mm:ss.ff") + "************************/");
                //写入发送者
                mStreamWriter.WriteLine("Sender:" +sender);
                //写入时间信息
                mStreamWriter.WriteLine("Args:" + args.ExceptionObject);
            }
            finally
            {
                if (mStreamWriter != null)
                {
                    mStreamWriter.Flush();
                    mStreamWriter.Close();
                    mStreamWriter = null;
                }
            }
            RunReset();
        }

        /// <summary>
        /// 执行我们的重启机制
        /// </summary>
        void RunReset()
        {
            //申明一个有参的线程
            Thread appReset = new Thread(new ParameterizedThreadStart(RunCmd));
            //当前运行目录与当前进程名
            object runPath = Environment.CurrentDirectory + "\\" ;
            appReset.Start(runPath);
            Thread.CurrentThread.Abort();
        }

        void RunCmd(object runPath)
        {
            string path = runPath as string;
            //获取当前进程名
            string processName = Process.GetCurrentProcess().ProcessName + ".exe";
            Console.WriteLine("The system will reset");
            //创建一个新的进程
            Process pc = new Process();
            //赋值启动的文件路径
            pc.StartInfo.FileName = path + processName;
            //开始启动
            pc.Start();
            //关闭当前进程
            RunKill("Server.exe");//"Server.exe"
            //退出当前窗口
            Environment.Exit(0);
        }

        /// <summary>
        /// 关掉当前进程
        /// </summary>
        /// <param name="name"></param>
        void RunKill(string name)
        {
            try
            {
                //查找进程
                foreach (Process p in Process.GetProcessesByName(name))
                {
                    p.Kill();
                }
            }
            catch (Exception e)
            {
                DebugUtil.Instance.Log2Time(e,logType.Error);
            }
        }

        #endregion
    }
}
