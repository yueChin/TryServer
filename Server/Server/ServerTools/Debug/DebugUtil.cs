using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using ServerTools.File;

namespace ServerTools.Debug
{
    public enum logType
    {
        Fatal = 0,//致命的错误
        Error = 1,//一般性错误
        Warring = 2,//运行警告
        Notice = 3,//高级别提醒
        Debug = 4,//普通提醒

    }

    /// <summary>
    /// 日志输出功能
    /// </summary>
    public class DebugUtil
    {
        /// <summary>
        /// 打印日志到本地的流对象
        /// </summary>
        private StreamWriter mStreamWriter;
        /// <summary>
        /// 是否正在写入中
        /// </summary>
        private bool mIsWriting;
        /// <summary>
        /// 打印日志的线程
        /// </summary>
        private Thread LogThread;
        /// <summary>
        /// 打印日志的线程池
        /// </summary>
        private List<LogClass> LogMsgs = new List<LogClass>();

        private Dictionary<logType, ConsoleColor> WriteColorDict = new Dictionary<logType, ConsoleColor>() { };
        private bool mIsWriteDebug = true;
        /// <summary>
        /// 单例
        /// </summary>
        private static DebugUtil mInstance;
        public static DebugUtil Instance
        {
            get
            {
                if (mInstance == null)
                {
                    mInstance = new DebugUtil();
                }
                return mInstance;
            }
        }

        private DebugUtil()
        {
            WriteColorDict.Add(logType.Debug,ConsoleColor.White);
            WriteColorDict.Add(logType.Notice,ConsoleColor.Green);
            WriteColorDict.Add(logType.Warring,ConsoleColor.Yellow);
            WriteColorDict.Add(logType.Error,ConsoleColor.Blue);
            WriteColorDict.Add(logType.Fatal,ConsoleColor.Red);
            LogThread = new Thread(new ThreadStart(start));
            LogThread.Start();
        }

        /// <summary>
        /// 开启线程
        /// </summary>
        private void start()
        {
            do
            {
                if (LogMsgs.Count > 0 && !mIsWriting)
                {
                    //TODO:打印日志
                    mIsWriting = true;
                    //将第零个日志打印
                    WriteMsg(LogMsgs[0]);
                    LogMsgs.RemoveAt(0);
                }
                //打印日志阻塞线程10ms
                Thread.Sleep(10);
            } while (mIsWriteDebug);
        }

        public void Close()
        {
            mIsWriteDebug = false;
        }

        /// <summary>
        /// 添加一个日志输出
        /// </summary>
        /// <param name="str"></param>
        /// <param name="type"></param>
        public void Log(object str, logType type = logType.Debug)
        {
            if (type == logType.Fatal)
                return; ;
            LogMsgs.Add(new LogClass(str,type));
        }

        /// <summary>
        /// 添加一个日志输出
        /// </summary>
        /// <param name="str"></param>
        /// <param name="type"></param>
        public void Log2Time(object str, logType type = logType.Debug)
        {
            if(type == logType.Fatal)
                return;;
            LogMsgs.Add(new LogClass(DateTime.Now.ToString("hh:mm:ss:ffff")+ str, type));
        }

        /// <summary>
        /// 开始打印日志
        /// </summary>
        /// <param name="log"></param>
        private void WriteMsg(LogClass log)
        {
            if (log == null)
            {
                Console.WriteLine("Error: Message is null");
                return;
            }
            //讲控制台打印的颜色调整为对应日志级别的颜色
            Console.ForegroundColor = WriteColorDict[log.Type];
            //将日志打印至控制台
            Console.WriteLine(log.msg);
            //重置打印信息的颜色
            Console.ResetColor();
            //储存日志信息
            WriteStream2Fold(log);
        }

        /// <summary>
        /// 将待打印的日志存贮到本地，将日志永久存储
        /// </summary>
        /// <param name="log"></param>
        private void WriteStream2Fold(LogClass log)
        {
            try
            {
                mIsWriting = true;
                //获取当前程序的运行路径
                string path = FileUtil.GetRunDictory();
                path += "/DebugLog/" + log.Type;
                //根目录不存在就创建一个目录文件夹
                FileUtil.CreateFolder(path);
                
                //将根目录路径和文件名组合
                path += "/" + DateTime.Now.ToString("yyyy-MM-dd") + ".log";
                //如果文件不存在，就创建一个文件，否则就加载文件，并讲对象赋值给该对象
                if (mStreamWriter == null)
                    mStreamWriter = !System.IO.File.Exists(path) ? System.IO.File.CreateText(path) : System.IO.File.AppendText(path);
                //开始写入文件
                mStreamWriter.WriteLine(log.msg);
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

                mIsWriting = false;
            }
        }
    }
}
