using System;
using System.Collections.Generic;
using System.Net.Sockets;
using ServerNetFrame.Auto;

namespace ServerNetFrame
{
    /// <summary>
    /// 客户端
    /// </summary>
    public class UserToken
    {
        /// <summary>
        /// 处理发送委托
        /// </summary>
        /// <param name="e"></param>
        public delegate void SendProcess(SocketAsyncEventArgs e);
        /// <summary>
        /// 处理关闭委托
        /// </summary>
        /// <param name="token"></param>
        /// <param name="error"></param>
        public delegate void CloseProcess(UserToken token, string error);
        /// <summary>
        /// 数据缓存
        /// </summary>
        private List<byte> cache = new List<byte>();
        private bool isReading = false;
        private bool isWriting = false;
        /// <summary>
        /// 粘包，拆包
        /// </summary>
        private List<byte[]> writeQueue = new List<byte[]>();
        /// <summary>
        /// socket封装
        /// </summary>
        public Socket conn;
        /// <summary>
        /// 异步接收事件
        /// </summary>
        public SocketAsyncEventArgs receiveSAEA;
        /// <summary>
        /// 异步发送事件
        /// </summary>
        public SocketAsyncEventArgs sendSAEA;
        public LengthEncode LE;
        public LengthDecode LD;
        public Encode encode;
        public Decode decode;
        //外部设置处理事件
        public UserToken.SendProcess sendProcess;
        public UserToken.CloseProcess closeProcess;
        public AbsHandlerCenter center;

        public UserToken()
        {           
            //设置异步事件
            this.receiveSAEA = new SocketAsyncEventArgs();
            this.sendSAEA = new SocketAsyncEventArgs();
            //异步事件的对象设置
            this.receiveSAEA.UserToken = (object)this;
            this.sendSAEA.UserToken = (object)this;
            //设置缓冲区
            this.receiveSAEA.SetBuffer(new byte[1024], 0, 1024);
        }
        /// <summary>
        /// 接受消息，数据包
        /// </summary>
        /// <param name="buff"></param>
        public void Receive(byte[] buff)
        {
            //增加枚举字节数组
            this.cache.AddRange((IEnumerable<byte>)buff);
            //如果该数据正在读取，返回
            if (this.isReading)
                return;
            //设置正在处理
            this.isReading = true;
            //处理数据包
            this.OnData();
        }

        /// <summary>
        /// 处理接收的数据包
        /// </summary>
        private void OnData()
        {
            byte[] numArray;
            //如果定长解码不为空，
            if (this.LD != null)
            {
                //解析数据包，定长解析
                numArray = this.LD(ref this.cache);
                //如果该数据包为空，就说明失败，返回
                if (numArray == null)
                {
                    this.isReading = false;
                    return;
                }
            }
            //如果定长解码为空
            else
            {
                //如果缓冲区也为空，说明不再读取，返回
                if (this.cache.Count == 0)
                {
                    this.isReading = false;
                    return;
                }
                //把缓存区的放到字节数组中
                numArray = this.cache.ToArray();
                this.cache.Clear();
            }
            //如果解码委托未设置，就抛错误
            if (this.decode == null)
                throw new Exception("message decode process is null");
            //把解析完毕的报文发送给报文处理中枢/应用层
            this.center.MessageReceive(this, this.decode(numArray));
            //递归
            this.OnData();
        }
        /// <summary>
        /// 发送报文
        /// </summary>
        /// <param name="type"></param>
        /// <param name="command"></param>
        /// <param name="message"></param>
        public void Write(byte type, int command, object message)
        {
            //长编码<-报文编码<-socket模型
            this.Write(LengthEncoding.Encode(MessageEncoding.Encode((object)new SocketModel(type, command, message))));
        }
        /// <summary>
        /// 发送报文
        /// </summary>
        /// <param name="value"></param>
        public void Write(byte[] value)
        {
            //如果socket为null，就
            if (this.conn == null)
            {
                //自己把自己赋值进关闭进程
                this.closeProcess(this, "调用已经断开的连接");
            }
            else
            {
                if (value == null && DebugMessage.Notice != null)
                    DebugMessage.Notice((object)"编码出待发送的值为null");
                //加入到发送队列中
                this.writeQueue.Add(value);
                //如果正在写，就返回
                if (this.isWriting)
                    return;
                this.isWriting = true;
                //调用写
                this.OnWrite();
            }
        }
        /// <summary>
        /// 正在发送的数据包
        /// </summary>
        public void OnWrite()
        {
            try
            {
                lock (this.sendSAEA)
                {
                    if (this.writeQueue.Count == 0)
                    {
                        //取消正在写标识
                        this.isWriting = false;
                    }
                    else
                    {
                        //取出队列中最前面的消息
                        byte[] write = this.writeQueue[0];
                        this.writeQueue.RemoveAt(0);
                        //如果write队列是空
                        if (write == null)
                        {
                            if (DebugMessage.Notice != null)
                                DebugMessage.Notice((object)"解析出待发送的值为null");
                            this.isWriting = false;
                            //递归
                            this.OnWrite();
                        }
                        else
                        {
                            //设置数据缓冲
                            this.sendSAEA.SetBuffer(write, 0, write.Length);
                            //发送异步，先发送拆出来的一部分出去
                            bool flag = this.conn.SendAsync(this.sendSAEA);
                            //如果处于挂起状态直接返回
                            if (flag)
                                return;
                            //不处于挂起就发送进程
                            this.sendProcess(this.sendSAEA);
                            Console.Write("是否挂起:" + flag.ToString());
                        }
                    }
                }
            }
            catch (InvalidOperationException ex)
            {
                if (DebugMessage.Error == null)
                    return;
                DebugMessage.Error((object)ex.Message);
            }
            catch (Exception ex)
            {
                if (DebugMessage.Error == null)
                    return;
                DebugMessage.Error((object)ex.Message);
            }
        }
        /// <summary>
        /// 一段报文发送成功后继续回调
        /// </summary>
        public void Writed()
        {
            this.OnWrite();
        }
        /// <summary>
        /// 断开连接
        /// </summary>
        public void Close()
        {
            try
            {
                this.writeQueue.Clear();
                this.cache.Clear();
                this.isReading = false;
                this.isWriting = false;
                //socket关闭，断开：发送以及接收
                this.conn.Shutdown(SocketShutdown.Both);
                //关闭连接
                this.conn.Close();
                this.conn = (Socket)null;
            }
            catch (Exception ex)
            {
                if (DebugMessage.Error == null)
                    return;
                DebugMessage.Error((object)ex.Message);
            }
        }

        
    }
}
