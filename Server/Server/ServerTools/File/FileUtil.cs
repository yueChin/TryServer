using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerTools.File
{
    public class FileUtil
    {
        private static FileUtil mInstance;

        /// <summary>
        /// 获取当前运行路径
        /// </summary>
        /// <returns></returns>
        public static string GetRunDictory()
        {
            return Environment.CurrentDirectory;
        }

        /// <summary>
        /// 获取当前运行路径的父对象
        /// </summary>
        /// <returns></returns>
        public static string GetRunDictoryParentPath(int pathNum = 3)
        {
            //获取到当前一运行目录的文件夹信息
            //对于我们当前运行的运行程序来说，当前存储程序的目录即为他的父对象
            //对于我们当前的程序夹来说，此目录问bin目录
            //GetRunDictory()的结果为Debug/Release
            DirectoryInfo pathInfo = Directory.GetParent(GetRunDictory());
            //根据当前路径获取父路径
            while (pathNum > 0 || pathInfo.Parent!= null)
            {
                DirectoryInfo info = pathInfo.Parent;
                pathInfo = info;
                pathNum--;
            }
            //获取到一个完整的文件夹路劲
            return pathInfo.FullName;
        }

        /// <summary>
        /// 创建一个文件夹
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string CreateFolder(string path)
        {
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            return path;
        }

        /// <summary>
        /// 创建一个文本文件
        /// </summary>
        /// <param name="path">文本路径</param>
        /// <param name="filename">文件名称</param>
        /// <param name="info">文件信息</param>
        public static void CreateFile(string path, string filename, string info)
        {
            StreamWriter streamWriter;
            FileInfo finfo = new FileInfo(path +"//"+filename);
            //判断文件是否存在，如果不存在就创建一个
            if(!Directory.Exists(path))
                Directory.CreateDirectory(path);
            //如果文件已存在，就直接删除
            if (finfo.Exists)
            {
                System.IO.File.Delete(path + "//"+filename);
            }
            //将创建一个文本对象并返回给流对象
            streamWriter = finfo.CreateText();
            streamWriter.WriteLine(info);
            streamWriter.Close();
            streamWriter.Dispose();
        }

        /// <summary>
        /// 向一个文件内添加一段数据
        /// </summary>
        /// <param name="path"></param>
        /// <param name="filename"></param>
        /// <param name="info"></param>
        public static void AddFild(string path, string filename, string info)
        {
            StreamWriter streamWriter;
            FileInfo fileInfo = new FileInfo(path +"//"+ filename);
            //判断文件是否存在，如果不存在就创建一个
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            streamWriter = fileInfo.Exists ? fileInfo.AppendText() :fileInfo.CreateText();
            //将创建一个文本对象并返回给流对象
            streamWriter.WriteLine(info);
            streamWriter.Close();
            streamWriter.Dispose();
        }

        /// <summary>
        /// 读取文件信息
        /// </summary>
        /// <param name="path"></param>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static string LoadFile(string path, string filename)
        {
            if (IsExitsFIle(path, filename)) return null;
            StreamReader streamReader = System.IO.File.OpenText(path + "//" + filename);
            ArrayList arr = new ArrayList();
            while (true)
            {
                //按行读取文本
                string line = streamReader.ReadLine();
                //读取到最后一行的下行，就跳出循环
                if(line == null)
                    break;
                arr.Add(line);
            }
            string str = String.Empty;
            //讲读取的内容添加到str字符串中
            foreach (var o in arr)
            {
                str += o;
            }
            streamReader.Close();
            streamReader.Dispose();
            return str;
        }

        /// <summary>
        /// 文件是否存在
        /// </summary>
        /// <param name="path"></param>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static bool IsExitsFIle(string path, string filename)
        {
            if (!Directory.Exists(path))
                return false;
            if (!System.IO.File.Exists(path + "//" + filename))
                return false;
            return true;
        }
    }
}
