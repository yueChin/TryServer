using System;
using System.Net;
using System.Net.Sockets;
using ServerNetFrame.Auto;

namespace ServerNetFrame
{
    /// <summary>
    /// 伺服启动，封装socket
    /// </summary>
    public class ServerStart
    {
        private Socket ServerIPV4;
        private Socket ServerIPV6;
        private int maxClient;
        /// <summary>
        /// 信号量： 限制可同时访问某一资源或资源池的线程数
        /// </summary>
        private System.Threading.Semaphore acceptClients;

        private UserTokenPool userTokenPool;
        public LengthEncode LE;
        public LengthDecode LD;
        public Encode encode;
        public Decode decode;
        public AbsHandlerCenter center;

        /// <summary>
        /// 服务器启动，构造时需指定最大连接数
        /// </summary>
        /// <param name="max">最大连接数</param>
        public ServerStart(int max)
        {
            //把消息编码方法置入编码委托
            this.encode = new Encode(MessageEncoding.Encode);
            //把消息解码方法置入解码委托
            this.decode = new Decode(MessageEncoding.Decode);
            //把定长解码置入定长解码委托中
            this.LD = new LengthDecode(LengthEncoding.Decode);
            //把定长编码置入定长编码委托中
            this.LE = new LengthEncode(LengthEncoding.Encode);
            //TCP ipv4连接,定义socket
            this.ServerIPV4 = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            //TCP ipv6连接,定义socket
            this.ServerIPV6 = new Socket(AddressFamily.InterNetworkV6, SocketType.Stream, ProtocolType.Tcp);
            this.maxClient = max;
            //线程池
            userTokenPool = new UserTokenPool(max);          
            for (int i = 0; i < max; i++)
            {
                UserToken token = new UserToken();
                //设置各种接收完成后的调用--读写完成
                token.receiveSAEA.Completed += new EventHandler<SocketAsyncEventArgs>(this.IO_Comleted);
                token.sendSAEA.Completed += new EventHandler<SocketAsyncEventArgs>(this.IO_Comleted);
                //设置解码/编码
                token.LD = this.LD;
                token.LE = this.LE;
                token.encode = this.encode;
                token.decode = this.decode;
                //设置发送委托和关闭委托
                token.sendProcess = new UserToken.SendProcess(this.ProcessSend);
                token.closeProcess = new UserToken.CloseProcess(this.ClientClose);
                token.center = this.center;
                userTokenPool.EnterQuene(token);
            }
        }
        /// <summary>
        /// 开始和指定端口连接
        /// </summary>
        /// <param name="port"></param>
        public void Start(int port)
        {
            this.acceptClients = new System.Threading.Semaphore(this.maxClient, this.maxClient);
            try
            {
                this.ServerIPV4.Bind((EndPoint)new IPEndPoint(IPAddress.Any, port));
                this.ServerIPV6.Bind((EndPoint)new IPEndPoint(IPAddress.IPv6Any, port));
               
                //挂起的连接队列的最大长度为10
                this.ServerIPV4.Listen(10);
                ////挂起的连接队列的最大长度为10
                this.ServerIPV6.Listen(10);
                //ipv4 开始接受，其中waitone 即是一般的new Thread + thread.start();
                StartAccept((SocketAsyncEventArgs)null);
                //ipv6 开始接受，传null
                StartAcceptV6((SocketAsyncEventArgs)null);
            }
            catch (Exception ex)
            {
                if (DebugMessage.Error == null)
                    return;
                DebugMessage.Error((object)ex.Message);
            }
        }

        #region 无限监听连接
        /// <summary>
        /// ipv4 开始接受
        /// </summary>
        /// <param name="e"></param>
        public void StartAccept(SocketAsyncEventArgs e)
        {
            //如果socket异步时间为空，就新建一个
            if (e == null)
            {
                //新实例一个
                e = new SocketAsyncEventArgs();
                //接受成功监听
                e.Completed += new EventHandler<SocketAsyncEventArgs>(this.Accept_Comleted);
            }
            else
                e.AcceptSocket = (Socket)null;
            //限制连接量
            //检查信号量计数，如果大于零，计数减一，然后继续执行，如果等于零，就挂起，直到计数再次大于零
            this.acceptClients.WaitOne();
            //如果处于挂起状态，就返回
            if (this.ServerIPV4.AcceptAsync(e))
                return;
            //接收开始
            this.ProcessAccept(e);
        }
        /// <summary>
        /// ipv6 开始接受
        /// </summary>
        /// <param name="e"></param>
        public void StartAcceptV6(SocketAsyncEventArgs e)
        {
            if (e == null)
            {
                e = new SocketAsyncEventArgs();
                e.Completed += new EventHandler<SocketAsyncEventArgs>(this.Accept_Comleted);
            }
            else
                e.AcceptSocket = (Socket)null;
            //信号量计数
            this.acceptClients.WaitOne();
            //挂起就返回
            if (this.ServerIPV6.AcceptAsync(e))
                return;
            //接受开始
            this.ProcessAccept(e);
        }
        /// <summary>
        /// 处理接受请求，无限处理
        /// </summary>
        /// <param name="e"></param>
        public void ProcessAccept(SocketAsyncEventArgs e)
        {
            //创建一个新的用户模型，然后设置/从池子中取出
            UserToken token = userTokenPool.DeQuene();           
            //设置socket
            token.conn = e.AcceptSocket;
            //连接处理中心连接socket
            this.center.ClientConnect(token);
            //开始接受数据
            this.StartReceive(token);
            try
            {
                //确认ipv4 或ipv6，无限接收
                switch (e.AcceptSocket.RemoteEndPoint.AddressFamily)
                {
                    case AddressFamily.InterNetwork:
                        this.StartAccept(e);
                        break;
                    case AddressFamily.InterNetworkV6:
                        this.StartAcceptV6(e);
                        break;
                }
            }
            catch (Exception ex)
            {
                if (DebugMessage.Error != null)
                    DebugMessage.Error((object)ex.Message);
                this.StartAccept(e);
            }
        }
        /// <summary>
        /// 接受完成后调用
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void Accept_Comleted(object sender, SocketAsyncEventArgs e)
        {
            this.ProcessAccept(e);
        }
#endregion
        #region 单socket处理
        /// <summary>
        /// 开始接收
        /// </summary>
        /// <param name="token"></param>
        public void StartReceive(UserToken token)
        {
            try
            {
                //如果目前socket处于挂起状态，返回
                if (token.conn.ReceiveAsync(token.receiveSAEA))
                    return;
                //否则开始接收，传入异步事件事件
                ProcessReceive(token.receiveSAEA);
            }
            catch (Exception ex)
            {
                if (DebugMessage.Error == null)
                    return;
                DebugMessage.Error((object)ex.Message);
            }
        }
        /// <summary>
        /// 接收/发送完成，如果未完成就继续接受，不然就发送
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void IO_Comleted(object sender, SocketAsyncEventArgs e)
        {
            //如果socket最后一个操作是接受的话，就继续调用进程接受
            if (e.LastOperation == SocketAsyncOperation.Receive)
                ProcessReceive(e);
            //否则就发送
            else
                ProcessSend(e);
        }
        /// <summary>
        /// 处理接收
        /// </summary>
        /// <param name="e"></param>
        public void ProcessReceive(SocketAsyncEventArgs e)
        {
            //从socket中提取用户模型
            UserToken userToken = e.UserToken as UserToken;
            //输入接受到的字节输大于零，并且socket标识为接收成功
            if (userToken.receiveSAEA.BytesTransferred > 0 && userToken.receiveSAEA.SocketError == SocketError.Success)
            {
                //提取传输出来的字节数组
                byte[] buff = new byte[userToken.receiveSAEA.BytesTransferred];
                //copy缓冲区到新建的字节数组
                Buffer.BlockCopy((Array)userToken.receiveSAEA.Buffer, 0, (Array)buff, 0, userToken.receiveSAEA.BytesTransferred);
                //把数组置入用户模型中
                userToken.Receive(buff);
                this.StartReceive(userToken);
            }
            //客户端断开连接处理
            else if ((uint)userToken.receiveSAEA.SocketError > 0U)
            {
                if (DebugMessage.Notice != null)
                    DebugMessage.Notice((object)("网络消息接收异常:用户退出+" + e.SocketError.ToString()));
                this.ClientClose(userToken, userToken.receiveSAEA.SocketError.ToString());
            }
            else
                this.ClientClose(userToken, "客户端主动断开连接");
        }
        /// <summary>
        /// 处理发送，赋值到UserToken
        /// </summary>
        /// <param name="e"></param>
        public void ProcessSend(SocketAsyncEventArgs e)
        {
            UserToken userToken = e.UserToken as UserToken;
            //客户端断开连接
            if ((uint)e.SocketError > 0U)
            {
                if (DebugMessage.Notice != null)
                    DebugMessage.Notice((object)("网络消息发送异常:用户退出+" + e.SocketError.ToString()));
                this.ClientClose(userToken, e.SocketError.ToString());
            }
            else
                userToken.Writed();
        }
        /// <summary>
        /// 客户端关闭
        /// </summary>
        /// <param name="token">断开的客户端对象</param>
        /// <param name="error">断开原因</param>
        public void ClientClose(UserToken token, string error)
        {
            if (token == null || token.conn == null)
            {
                if (DebugMessage.Debug == null)
                    return;
                DebugMessage.Debug((object)("客户端异常退出:" + error));
            }
            //如果不为空，正常执行客户端断开连接
            else
            {
                lock (token)
                {
                    if (DebugMessage.Debug != null)
                        DebugMessage.Debug((object)("用户退出" + error));
                    //应用层断开连接业务处理，并处具体客户端以及原因
                    this.center.ClientClose(token, error);                  
                    //客户端关闭
                    token.Close();
                    //关闭客户端，回收回池子
                    userTokenPool.EnterQuene(token);
                    try
                    {
                        //释放信号量
                        this.acceptClients.Release();
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
#endregion
    }
}
