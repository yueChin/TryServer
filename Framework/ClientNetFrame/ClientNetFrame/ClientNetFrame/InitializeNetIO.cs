// Decompiled with JetBrains decompiler
// Type: ClientNetFrame.InitializeNetIO
// Assembly: ClientNetFrame, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: A7A2F2C3-738D-4CF8-AD06-770174DB25BF
// Assembly location: E:\Demo\Client\demo\Assets\Plugin\ClientNetFrame.dll

using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using ProtoBuf;
using ProtoBuf.Meta;

namespace ClientNetFrame
{
    /// <summary>
    /// 初始化网络IO
    /// </summary>
    public class InitializeNetIO
    {
        private string readip = string.Empty;
        private int readport = 0;
        private bool isreconnect = false;
        private byte[] readbuff = new byte[1024];
        private List<byte> cache = new List<byte>();
        private List<SocketModel> messages = new List<SocketModel>();
        private AddressFamily Address = AddressFamily.InterNetwork;
        private bool isReading = false;
        public DebugLogCallBack DebugCallBack;
        public ConnectFeiledCallBack ConnectFeiledCallBack;
        public ConnectFeiledCallBack ReceiveFeiledCallBack;
        public ConnectFeiledCallBack WriteFeiledCallBack;
        private Socket socket;

        private string IP
        {
            get
            {
                return this.readip;
            }
            set
            {
                this.readip = value;
            }
        }

        private int Port
        {
            get
            {
                return this.readport;
            }
            set
            {
                this.readport = value;
            }
        }

        public bool IsReconnect
        {
            get
            {
                return this.isreconnect;
            }
        }

        /// <summary>
        /// 初始化，给出服务器端口和ip地址
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        /// <param name="address"></param>
        public void Initialize(string ip, int port, AddressFamily address = AddressFamily.InterNetwork)
        {
            this.IP = ip;
            this.Port = port;
            this.Address = address;
        }
        /// <summary>
        /// 连接到服务器
        /// </summary>
        public void ConnnectToSever()
        {
            try
            {
                //开启一个socket
                this.socket = new Socket(this.Address, SocketType.Stream, ProtocolType.Tcp);
                if (this.DebugCallBack != null)
                    this.DebugCallBack((object)("开始连接服务器：" + this.IP + ":" + (object)this.Port));
                this.socket.Connect(this.IP, this.Port);
                if (this.DebugCallBack != null)
                    this.DebugCallBack((object)"连接服务器成功");
                //开始连接，给出接受的方法
                this.socket.BeginReceive(this.readbuff, 0, 1024, SocketFlags.None, new AsyncCallback(this.ReceiveCallBack), (object)this.readbuff);
            }
            catch (Exception ex)
            {
                if (this.ConnectFeiledCallBack != null)
                    this.ConnectFeiledCallBack(ex);
                if (this.DebugCallBack != null)
                    this.DebugCallBack((object)("链接服务器" + this.IP + ":" + (object)this.Port + "失败" + "/n".ToString() + ex.ToString()));
            }
            this.isreconnect = false;
        }

        public bool CloseSocket()
        {
            if (this.isreconnect)
                return false;
            this.socket.Close();
            this.isreconnect = true;
            return true;
        }

        public int GetSocketMessageCount()
        {
            return this.messages.Count;
        }

        public SocketModel GetMessage()
        {
            if (this.GetSocketMessageCount() <= 0)
                return (SocketModel)null;
            SocketModel message = this.messages[0];
            this.messages.RemoveAt(0);
            return message;
        }
        /// <summary>
        /// 接收方法的回调
        /// </summary>
        /// <param name="ar"></param>
        private void ReceiveCallBack(IAsyncResult ar)
        {
            try
            {
                int count = this.socket.EndReceive(ar);
                byte[] numArray = new byte[count];
                Buffer.BlockCopy((Array)this.readbuff, 0, (Array)numArray, 0, count);
                this.cache.AddRange((IEnumerable<byte>)numArray);
                if (!this.isReading)
                {
                    this.isReading = true;
                    //伪循环，跳出以后继续监听服务器来的消息
                    this.OnData();
                }
                //开始接受，把自己作为回调放如socket中，伪无限接受
                this.socket.BeginReceive(this.readbuff, 0, 1024, SocketFlags.None, new AsyncCallback(this.ReceiveCallBack), (object)this.readbuff);
            }
            catch (Exception ex)
            {
                if (this.DebugCallBack != null)
                    this.DebugCallBack((object)("远程服务器主动断开连接" + "/n".ToString() + (object)ex));
                if (this.ReceiveFeiledCallBack == null)
                    return;
                this.ReceiveFeiledCallBack(ex);
            }
        }

        #region 发送报文
        public void Write(byte type, int command, object message)
        {
            ByteArray byteArray1 = new ByteArray();
            //填充协议
            byteArray1.write(type);
            byteArray1.write(command);
            //如过消息不为空，就序列化后发送
            if (message != null)
                //byteArray1.write(SerializeUtil.encode(message));
                byteArray1.write(SerializeUtil.ProteBufEncode(message));
            ByteArray byteArray2 = new ByteArray();
            byteArray2.write(byteArray1.Length);
            byteArray2.write(byteArray1.getBuff());
            try
            {
                //socket发送消息
                this.socket.Send(byteArray2.getBuff());
            }
            catch (Exception ex)
            {
                if (this.DebugCallBack != null)
                    this.DebugCallBack((object)("网络错误，请重新登录" + "/n".ToString() + ex.Message));
                if (this.WriteFeiledCallBack == null)
                    return;
                this.WriteFeiledCallBack(ex);
            }
        }
        
        #endregion

        #region 处理报文

        

        #endregion
        /// <summary>
        /// 循环处理接收报文,粘/拆包
        /// </summary>
        private void OnData()
        {
            byte[] numArray = this.Decode(ref this.cache);
            //为空就说明不再读取
            if (numArray == null)
            {
                this.isReading = false;
            }
            else
            {
                //新建一个协议基础模型,真正解码
                SocketModel socketModel = this.mDecode(numArray);
                if (socketModel == null)
                {
                    this.isReading = false;
                }
                else
                {
                    this.messages.Add(socketModel);
                    this.OnData();
                }
            }
        }        

        /// <summary>
        /// 解码/解译/反序列化
        /// </summary>
        /// <param name="cache"></param>
        /// <returns></returns>
        private byte[] Decode(ref List<byte> cache)
        {
            if (cache.Count < 3)
                return (byte[])null;
            byte[] numArray;
            using (MemoryStream memoryStream = new MemoryStream(cache.ToArray()))
            {
                BinaryReader binaryReader = new BinaryReader((Stream) memoryStream);
                int count = binaryReader.ReadInt32();
                if ((long) count > memoryStream.Length - memoryStream.Position)
                    return (byte[]) null;
                numArray = binaryReader.ReadBytes(count);
                cache.Clear();
                cache.AddRange(
                    (IEnumerable<byte>) binaryReader.ReadBytes((int) (memoryStream.Length - memoryStream.Position)));
                binaryReader.Close();
            }
            return numArray;
        }        

        /// <summary>
        /// 解码
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private SocketModel mDecode(byte[] value)
        {
            ByteArray byteArray = new ByteArray(value);
            SocketModel socketModel = new SocketModel();
            byte num1;
            byteArray.read(out num1);
            int num2;
            byteArray.read(out num2);
            socketModel.type = num1;
            socketModel.command = num2;
            if (byteArray.Readnable)
            {
                byte[] numArray;
                byteArray.read(out numArray, byteArray.Length - byteArray.Position);
                //序列化消息
                //socketModel.message = SerializeUtil.decode(numArray);
                socketModel.message = SerializeUtil.ProtoBufDecode<SocketModel>(numArray);
            }
            byteArray.Close();
            return socketModel;
        }
    }
}
