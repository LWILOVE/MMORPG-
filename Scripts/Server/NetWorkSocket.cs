using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System;
using System.Text;

/// <summary>
/// 网络传输Socket
/// </summary>
public class NetWorkSocket : SingletonMono<NetWorkSocket>
{
    #region 消息发送变量
    /// <summary>
    /// 消息发送队列
    /// </summary>
    private Queue<byte[]> m_SendQueue = new Queue<byte[]>();

    /// <summary>
    /// 消息发送队列检查委托
    /// </summary>
    private Action m_CheckSendQueue;

    /// <summary>
    /// 压缩数组的长度界限
    /// </summary>
    private const int m_CompressLen = 200000;
    #endregion 
    /// <summary>
    /// 是否连接成功
    /// </summary>
    private bool m_IsConnectedOk;

    #region 消息接收变量
    /// <summary>
    /// 接收数据包的缓冲数据流
    /// </summary>
    private MMO_MemoryStream m_ReceiveMemoryStream = new MMO_MemoryStream();

    /// <summary>
    /// 数据接收缓冲区
    /// </summary>
    private byte[] m_ReceiveBuffer = new byte[1024];

    /// <summary>
    /// 接收消息队列
    /// </summary>
    private Queue<byte[]> m_ReceiveQueue = new Queue<byte[]>();

    private int m_ReceiveCount = 0;
    #endregion
    /// <summary>
    /// 客户端socket
    /// </summary>
    private Socket m_Client;

    /// <summary>
    /// 连接成功委托
    /// </summary>
    public Action OnConnectOk;


    protected override void OnStart()
    {
        base.OnStart();
    }
    
    protected override void OnUpdate()
    {
        base.OnUpdate();

        if (m_IsConnectedOk)
        {
            m_IsConnectedOk = false;
            if (OnConnectOk != null)
            {
                OnConnectOk();
                Debug.Log("连接成功");
            }
        }

        #region 从队列中获取数据（从m_ReceiveQueue中取出数据，并派发数据指定的协议号）
        while (true)
        {
            //设定每帧的最大消息接收量
            if (m_ReceiveCount <= 50)
            {
                m_ReceiveCount++;
                lock (m_ReceiveQueue)
                {
                    if (m_ReceiveQueue.Count > 0)
                    {
                        //1.获取从服务器传输过来的数据包
                        byte[] buffer = m_ReceiveQueue.Dequeue();
                        //2.获取异或后的数组
                        byte[] bufferNew = new byte[buffer.Length - 3];

                        bool isCompress = false;
                        ushort crc = 0;

                        //获取一下接收到的消息
                        using (MMO_MemoryStream ms = new MMO_MemoryStream(buffer))
                        {
                            //获取包装后的数据
                            isCompress = ms.ReadBool();
                            crc = ms.ReadUShort();
                            ms.Read(bufferNew, 0, bufferNew.Length);
                        }
                        //开始解包
                        //1.进行CRC解析
                        int newCrc = Crc16.CalculateCrc16(bufferNew);

                        //若获取到的数组的CRC和传递过来的CRC一致则说明数据无误
                        if (newCrc == crc)
                        {
                            //将数据进行还原（异或）
                            bufferNew = SecurityUtil.Xor(bufferNew);
                            //如果数据曾压缩过则解压缩
                            if (isCompress)
                            {
                                bufferNew = ZlibHelper.DeCompressBytes(bufferNew);
                            }
                            ushort protoCode = 0;
                            byte[] protoContent = new byte[bufferNew.Length - 2];
                            using (MMO_MemoryStream ms = new MMO_MemoryStream(bufferNew))
                            {
                                //获取协议编号
                                protoCode = ms.ReadUShort();
                                //读取原数据内容
                                ms.Read(protoContent, 0, protoContent.Length);
                                Debug.Log("派发一份协议:"+protoCode);
                                //进行数据派发
                                SocketDispatcher.Instance.Dispatch(protoCode, protoContent);
                            }
                        }
                        else
                        {
                            break;
                        }

                    }
                    else
                    {
                        break;
                    }
                }
            }
            else
            {
                m_ReceiveCount = 0;
                break;
            }
        }
        #endregion
    }

    #region Connect Socket服务器连接
    /// <summary>
    /// socket服务器连接
    /// </summary>
    /// <param name="ip">服务器ip地址</param>
    /// <param name="port">端口号</param>
    public void Connect(string ip, int port)
    {
        Debug.Log("开始连接");
        //若socket已经存在且已进入连接状态则返回
        if (m_Client != null && m_Client.Connected){return;}
        //开始连接
        m_Client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        try
        {
            //开始连接
            m_Client.BeginConnect(new IPEndPoint(IPAddress.Parse(ip), port), ConnectCallBack, m_Client);
        }
        catch (Exception ex)
        {
            Debug.Log("连接失败" + ex.Message);
        }
    }

    private void ConnectCallBack(IAsyncResult ar)
    {
        if (m_Client.Connected)
        {
            m_CheckSendQueue = OnCheckSendQueueCallBack;

            ReceiveMessage();
            m_IsConnectedOk = true;
        }
        else
        {
            Debug.Log("socket连接失败");
        }
        m_Client.EndConnect(ar);
    }
    #endregion



    protected override void BeforeOnDestroy()
    {
        base.BeforeOnDestroy();
        ////当游戏结束时（该游戏物体被销毁），关闭开启的客户端
        //if (m_Client != null && m_Client.Connected)
        //{
        //    m_Client.Shutdown(SocketShutdown.Both);
        //    m_Client.Close();
        //}
        DisConnect();
    }

    /// <summary>
    /// 断开服务器
    /// </summary>
    public void DisConnect()
    {
        if (m_Client != null)
        {
            m_Client.Shutdown(SocketShutdown.Both);
            m_Client.Close();
        }
    }

    //==================================================================消息发送区==================================================================//
    #region OnCheckSendQueueCallBack 检查队列委托回调
    /// <summary>
    /// 检查队列委托回调
    /// </summary>
    /// <exception cref="NotImplementedException"></exception>
    private void OnCheckSendQueueCallBack()
    {
        lock (m_SendQueue)
        {
            //若队列中含有数据包，那么发送数据包
            if (m_SendQueue.Count > 0)
            {
                //发送数据包:数据出队
                Send(m_SendQueue.Dequeue());
            }
        }
    }
    #endregion

    #region MakeData 封装数据包
    /// <summary>
    /// 封装数据包
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    private byte[] MakeData(byte[] data)
    {
        byte[] retBuffer = null;

        //1.判定数据包长是否达到压缩界线
        bool isCompress = data.Length > m_CompressLen ? true : false;
        if (isCompress)
        {
            data = ZlibHelper.CompressBytes(data);
        }

        //2.对数据进行异或
        data = SecurityUtil.Xor(data);

        //3.进行CRC校验（压缩后的）
        ushort crc = Crc16.CalculateCrc16(data);

        using (MMO_MemoryStream ms = new MMO_MemoryStream())
        {
            ms.WriteUShort((ushort)(data.Length + 3));
            ms.WriteBool(isCompress);
            ms.WriteUShort(crc);
            ms.Write(data, 0, data.Length);
            retBuffer = ms.ToArray();
        }
        return retBuffer;
    }
    #endregion

    #region SendMessage 消息发送-将消息加入队列    
    /// <summary>
    /// 消息发生-将消息加入队列
    /// </summary>
    /// <param name="buffer"></param>
    public void SendMessage(byte[] buffer)
    {
        Debug.Log("发生一份消息");
        //得到封装后的数据包
        byte[] sendBuffer = MakeData(buffer);
        lock (m_SendQueue)
        {
            //数据包入队
            m_SendQueue.Enqueue(sendBuffer);

            if (m_CheckSendQueue == null) return;
            //启动消息发送检查委托
            m_CheckSendQueue.BeginInvoke(null, null);
            Debug.Log("完成消息发送");
        }
    }
    #endregion

    #region Send 将消息发送到服务器
    /// <summary>
    /// 将消息发送到服务器
    /// </summary>
    /// <param name="buffer"></param>
    private void Send(byte[] buffer)
    {
        //进行消息发送
        m_Client.BeginSend(buffer, 0, buffer.Length, SocketFlags.None, SendCallBack, m_Client);
    }
    #endregion

    #region SendCallBack 消息发送到数据包回调
    /// <summary>
    /// 消息发送到数据包回调
    /// </summary>
    /// <param name="ar"></param>
    private void SendCallBack(IAsyncResult ar)
    {
        //当完成一次消息发送时应继续检查是否还有待发送消息
        m_Client.EndSend(ar);

        OnCheckSendQueueCallBack();
    }
    #endregion

    //==================================================================消息接收区==================================================================//
    #region ReceiveMessage 数据接收
    /// <summary>
    /// 接收数据
    /// </summary>
    private void ReceiveMessage()
    {
        //进行异步接收
        m_Client.BeginReceive(m_ReceiveBuffer, 0, m_ReceiveBuffer.Length, SocketFlags.None, ReceiveCallBack, m_Client);
    }
    #endregion

    #region ReceiveCallBack 客户端数据异步接收回调(纯数据接收，将数据放入此m_ReceiveQueue)
    /// <summary>
    /// 客户端数据异步接收回调(纯数据接收，将数据放入此m_ReceiveQueue)
    /// </summary>
    /// <param name="ar"></param>
    /// <exception cref="NotImplementedException"></exception>
    private void ReceiveCallBack(IAsyncResult ar)
    {
        //本try的意义为：若客户端突然断开时（如利用任务管理器结束连接）则使其结束
        try
        {
            //数据接收到的长度
            int length_Receive = m_Client.EndReceive(ar);
            //若接收到的数据>0则表明有数据接收
            if (length_Receive > 0)
            {
                Debug.Log("socket连接成功，正在检查服务器返回的数据");

                //讲接收到的数据写入缓冲数据流的尾部
                //Position:本次数据写入位置
                //Length：当前流中已有的数据长
                m_ReceiveMemoryStream.Position = m_ReceiveMemoryStream.Length;
                //将接收到的数据写入数据流中
                m_ReceiveMemoryStream.Write(m_ReceiveBuffer, 0, length_Receive);

                //当收到的消息长度大于2时，表明本次收集到的消息是不完整的(ushort长为2)
                if (m_ReceiveMemoryStream.Length > 2)
                {
                    //对不完整的数据包进行拆分
                    while (true)
                    {
                        //将数据流指针放到0处
                        m_ReceiveMemoryStream.Position = 0;
                        //读取一个数据包包体长
                        int currMessageLength = m_ReceiveMemoryStream.ReadUShort();
                        //获取总的数据包长：包头长+包体长
                        int currFullMessageLength = 2 + currMessageLength;
                        //当读入的缓冲流大于一个数据包长则表面本次接收到的包至少有一个是完整的
                        if (m_ReceiveMemoryStream.Length >= currFullMessageLength)
                        {
                            //定义包体byte[]数组
                            byte[] buffer = new byte[currMessageLength];
                            //将数据流指针指定到2的位置，既包体位置
                            m_ReceiveMemoryStream.Position = 2;
                            //将包体读入byte数组中
                            m_ReceiveMemoryStream.Read(buffer, 0, currMessageLength);

                            lock (m_ReceiveQueue)
                            {
                                //将收到的消息入队
                                m_ReceiveQueue.Enqueue(buffer);
                            }

                            //针对剩余字节数组的处理
                            //获取读后剩余的字节长
                            int remainLength = (int)m_ReceiveMemoryStream.Length - currFullMessageLength;

                            if (remainLength > 0)
                            {
                                //将指针放到首包尾部
                                m_ReceiveMemoryStream.Position = currFullMessageLength;
                                //将剩下的内容都读出来
                                byte[] remainBuffer = new byte[remainLength];
                                m_ReceiveMemoryStream.Read(remainBuffer, 0, remainLength);
                                //将数据流清空
                                m_ReceiveMemoryStream.Position = 0;
                                m_ReceiveMemoryStream.SetLength(0);
                                //再重新写入使得所有的数据能够完全处理
                                m_ReceiveMemoryStream.Write(remainBuffer, 0, remainBuffer.Length);
                                remainBuffer = null;
                            }
                            else
                            {
                                //若恰好读完，则清空数据流,并终止
                                m_ReceiveMemoryStream.Position = 0;
                                m_ReceiveMemoryStream.SetLength(0);
                                break;
                            }
                        }
                        else
                        {
                            //否则表明本次收到的包不够一个数据包长，那么本次读取结束
                            break;
                        }
                    }
                }

                //进行下一次数据包的接收
                ReceiveMessage();
            }
            else
            {
                //当接收到的信息为0时，表明客户端已经断开连接了,打印断开信息，并将对应玩家从玩家列表中移除
                Debug.Log(string.Format("服务器{0}断开连接", m_Client.RemoteEndPoint.ToString()));
            }
        }
        catch
        {
            //当接收到的信息为0时，表明客户端已经断开连接了,打印断开信息，并将对应玩家从玩家列表中移除
            Debug.Log(string.Format("服务器{0}断开连接", m_Client.RemoteEndPoint.ToString()));
        }
    }
    #endregion
}
