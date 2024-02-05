using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System;
using System.Text;

/// <summary>
/// ���紫��Socket
/// </summary>
public class NetWorkSocket : SingletonMono<NetWorkSocket>
{
    #region ��Ϣ���ͱ���
    /// <summary>
    /// ��Ϣ���Ͷ���
    /// </summary>
    private Queue<byte[]> m_SendQueue = new Queue<byte[]>();

    /// <summary>
    /// ��Ϣ���Ͷ��м��ί��
    /// </summary>
    private Action m_CheckSendQueue;

    /// <summary>
    /// ѹ������ĳ��Ƚ���
    /// </summary>
    private const int m_CompressLen = 200000;
    #endregion 
    /// <summary>
    /// �Ƿ����ӳɹ�
    /// </summary>
    private bool m_IsConnectedOk;

    #region ��Ϣ���ձ���
    /// <summary>
    /// �������ݰ��Ļ���������
    /// </summary>
    private MMO_MemoryStream m_ReceiveMemoryStream = new MMO_MemoryStream();

    /// <summary>
    /// ���ݽ��ջ�����
    /// </summary>
    private byte[] m_ReceiveBuffer = new byte[1024];

    /// <summary>
    /// ������Ϣ����
    /// </summary>
    private Queue<byte[]> m_ReceiveQueue = new Queue<byte[]>();

    private int m_ReceiveCount = 0;
    #endregion
    /// <summary>
    /// �ͻ���socket
    /// </summary>
    private Socket m_Client;

    /// <summary>
    /// ���ӳɹ�ί��
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
                Debug.Log("���ӳɹ�");
            }
        }

        #region �Ӷ����л�ȡ���ݣ���m_ReceiveQueue��ȡ�����ݣ����ɷ�����ָ����Э��ţ�
        while (true)
        {
            //�趨ÿ֡�������Ϣ������
            if (m_ReceiveCount <= 50)
            {
                m_ReceiveCount++;
                lock (m_ReceiveQueue)
                {
                    if (m_ReceiveQueue.Count > 0)
                    {
                        //1.��ȡ�ӷ�����������������ݰ�
                        byte[] buffer = m_ReceiveQueue.Dequeue();
                        //2.��ȡ���������
                        byte[] bufferNew = new byte[buffer.Length - 3];

                        bool isCompress = false;
                        ushort crc = 0;

                        //��ȡһ�½��յ�����Ϣ
                        using (MMO_MemoryStream ms = new MMO_MemoryStream(buffer))
                        {
                            //��ȡ��װ�������
                            isCompress = ms.ReadBool();
                            crc = ms.ReadUShort();
                            ms.Read(bufferNew, 0, bufferNew.Length);
                        }
                        //��ʼ���
                        //1.����CRC����
                        int newCrc = Crc16.CalculateCrc16(bufferNew);

                        //����ȡ���������CRC�ʹ��ݹ�����CRCһ����˵����������
                        if (newCrc == crc)
                        {
                            //�����ݽ��л�ԭ�����
                            bufferNew = SecurityUtil.Xor(bufferNew);
                            //���������ѹ�������ѹ��
                            if (isCompress)
                            {
                                bufferNew = ZlibHelper.DeCompressBytes(bufferNew);
                            }
                            ushort protoCode = 0;
                            byte[] protoContent = new byte[bufferNew.Length - 2];
                            using (MMO_MemoryStream ms = new MMO_MemoryStream(bufferNew))
                            {
                                //��ȡЭ����
                                protoCode = ms.ReadUShort();
                                //��ȡԭ��������
                                ms.Read(protoContent, 0, protoContent.Length);
                                Debug.Log("�ɷ�һ��Э��:"+protoCode);
                                //���������ɷ�
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

    #region Connect Socket����������
    /// <summary>
    /// socket����������
    /// </summary>
    /// <param name="ip">������ip��ַ</param>
    /// <param name="port">�˿ں�</param>
    public void Connect(string ip, int port)
    {
        Debug.Log("��ʼ����");
        //��socket�Ѿ��������ѽ�������״̬�򷵻�
        if (m_Client != null && m_Client.Connected){return;}
        //��ʼ����
        m_Client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        try
        {
            //��ʼ����
            m_Client.BeginConnect(new IPEndPoint(IPAddress.Parse(ip), port), ConnectCallBack, m_Client);
        }
        catch (Exception ex)
        {
            Debug.Log("����ʧ��" + ex.Message);
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
            Debug.Log("socket����ʧ��");
        }
        m_Client.EndConnect(ar);
    }
    #endregion



    protected override void BeforeOnDestroy()
    {
        base.BeforeOnDestroy();
        ////����Ϸ����ʱ������Ϸ���屻���٣����رտ����Ŀͻ���
        //if (m_Client != null && m_Client.Connected)
        //{
        //    m_Client.Shutdown(SocketShutdown.Both);
        //    m_Client.Close();
        //}
        DisConnect();
    }

    /// <summary>
    /// �Ͽ�������
    /// </summary>
    public void DisConnect()
    {
        if (m_Client != null)
        {
            m_Client.Shutdown(SocketShutdown.Both);
            m_Client.Close();
        }
    }

    //==================================================================��Ϣ������==================================================================//
    #region OnCheckSendQueueCallBack ������ί�лص�
    /// <summary>
    /// ������ί�лص�
    /// </summary>
    /// <exception cref="NotImplementedException"></exception>
    private void OnCheckSendQueueCallBack()
    {
        lock (m_SendQueue)
        {
            //�������к������ݰ�����ô�������ݰ�
            if (m_SendQueue.Count > 0)
            {
                //�������ݰ�:���ݳ���
                Send(m_SendQueue.Dequeue());
            }
        }
    }
    #endregion

    #region MakeData ��װ���ݰ�
    /// <summary>
    /// ��װ���ݰ�
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    private byte[] MakeData(byte[] data)
    {
        byte[] retBuffer = null;

        //1.�ж����ݰ����Ƿ�ﵽѹ������
        bool isCompress = data.Length > m_CompressLen ? true : false;
        if (isCompress)
        {
            data = ZlibHelper.CompressBytes(data);
        }

        //2.�����ݽ������
        data = SecurityUtil.Xor(data);

        //3.����CRCУ�飨ѹ����ģ�
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

    #region SendMessage ��Ϣ����-����Ϣ�������    
    /// <summary>
    /// ��Ϣ����-����Ϣ�������
    /// </summary>
    /// <param name="buffer"></param>
    public void SendMessage(byte[] buffer)
    {
        Debug.Log("����һ����Ϣ");
        //�õ���װ������ݰ�
        byte[] sendBuffer = MakeData(buffer);
        lock (m_SendQueue)
        {
            //���ݰ����
            m_SendQueue.Enqueue(sendBuffer);

            if (m_CheckSendQueue == null) return;
            //������Ϣ���ͼ��ί��
            m_CheckSendQueue.BeginInvoke(null, null);
            Debug.Log("�����Ϣ����");
        }
    }
    #endregion

    #region Send ����Ϣ���͵�������
    /// <summary>
    /// ����Ϣ���͵�������
    /// </summary>
    /// <param name="buffer"></param>
    private void Send(byte[] buffer)
    {
        //������Ϣ����
        m_Client.BeginSend(buffer, 0, buffer.Length, SocketFlags.None, SendCallBack, m_Client);
    }
    #endregion

    #region SendCallBack ��Ϣ���͵����ݰ��ص�
    /// <summary>
    /// ��Ϣ���͵����ݰ��ص�
    /// </summary>
    /// <param name="ar"></param>
    private void SendCallBack(IAsyncResult ar)
    {
        //�����һ����Ϣ����ʱӦ��������Ƿ��д�������Ϣ
        m_Client.EndSend(ar);

        OnCheckSendQueueCallBack();
    }
    #endregion

    //==================================================================��Ϣ������==================================================================//
    #region ReceiveMessage ���ݽ���
    /// <summary>
    /// ��������
    /// </summary>
    private void ReceiveMessage()
    {
        //�����첽����
        m_Client.BeginReceive(m_ReceiveBuffer, 0, m_ReceiveBuffer.Length, SocketFlags.None, ReceiveCallBack, m_Client);
    }
    #endregion

    #region ReceiveCallBack �ͻ��������첽���ջص�(�����ݽ��գ������ݷ����m_ReceiveQueue)
    /// <summary>
    /// �ͻ��������첽���ջص�(�����ݽ��գ������ݷ����m_ReceiveQueue)
    /// </summary>
    /// <param name="ar"></param>
    /// <exception cref="NotImplementedException"></exception>
    private void ReceiveCallBack(IAsyncResult ar)
    {
        //��try������Ϊ�����ͻ���ͻȻ�Ͽ�ʱ������������������������ӣ���ʹ�����
        try
        {
            //���ݽ��յ��ĳ���
            int length_Receive = m_Client.EndReceive(ar);
            //�����յ�������>0����������ݽ���
            if (length_Receive > 0)
            {
                Debug.Log("socket���ӳɹ������ڼ����������ص�����");

                //�����յ�������д�뻺����������β��
                //Position:��������д��λ��
                //Length����ǰ�������е����ݳ�
                m_ReceiveMemoryStream.Position = m_ReceiveMemoryStream.Length;
                //�����յ�������д����������
                m_ReceiveMemoryStream.Write(m_ReceiveBuffer, 0, length_Receive);

                //���յ�����Ϣ���ȴ���2ʱ�����������ռ�������Ϣ�ǲ�������(ushort��Ϊ2)
                if (m_ReceiveMemoryStream.Length > 2)
                {
                    //�Բ����������ݰ����в��
                    while (true)
                    {
                        //��������ָ��ŵ�0��
                        m_ReceiveMemoryStream.Position = 0;
                        //��ȡһ�����ݰ����峤
                        int currMessageLength = m_ReceiveMemoryStream.ReadUShort();
                        //��ȡ�ܵ����ݰ�������ͷ��+���峤
                        int currFullMessageLength = 2 + currMessageLength;
                        //������Ļ���������һ�����ݰ�������汾�ν��յ��İ�������һ����������
                        if (m_ReceiveMemoryStream.Length >= currFullMessageLength)
                        {
                            //�������byte[]����
                            byte[] buffer = new byte[currMessageLength];
                            //��������ָ��ָ����2��λ�ã��Ȱ���λ��
                            m_ReceiveMemoryStream.Position = 2;
                            //���������byte������
                            m_ReceiveMemoryStream.Read(buffer, 0, currMessageLength);

                            lock (m_ReceiveQueue)
                            {
                                //���յ�����Ϣ���
                                m_ReceiveQueue.Enqueue(buffer);
                            }

                            //���ʣ���ֽ�����Ĵ���
                            //��ȡ����ʣ����ֽڳ�
                            int remainLength = (int)m_ReceiveMemoryStream.Length - currFullMessageLength;

                            if (remainLength > 0)
                            {
                                //��ָ��ŵ��װ�β��
                                m_ReceiveMemoryStream.Position = currFullMessageLength;
                                //��ʣ�µ����ݶ�������
                                byte[] remainBuffer = new byte[remainLength];
                                m_ReceiveMemoryStream.Read(remainBuffer, 0, remainLength);
                                //�����������
                                m_ReceiveMemoryStream.Position = 0;
                                m_ReceiveMemoryStream.SetLength(0);
                                //������д��ʹ�����е������ܹ���ȫ����
                                m_ReceiveMemoryStream.Write(remainBuffer, 0, remainBuffer.Length);
                                remainBuffer = null;
                            }
                            else
                            {
                                //��ǡ�ö��꣬�����������,����ֹ
                                m_ReceiveMemoryStream.Position = 0;
                                m_ReceiveMemoryStream.SetLength(0);
                                break;
                            }
                        }
                        else
                        {
                            //������������յ��İ�����һ�����ݰ�������ô���ζ�ȡ����
                            break;
                        }
                    }
                }

                //������һ�����ݰ��Ľ���
                ReceiveMessage();
            }
            else
            {
                //�����յ�����ϢΪ0ʱ�������ͻ����Ѿ��Ͽ�������,��ӡ�Ͽ���Ϣ��������Ӧ��Ҵ�����б����Ƴ�
                Debug.Log(string.Format("������{0}�Ͽ�����", m_Client.RemoteEndPoint.ToString()));
            }
        }
        catch
        {
            //�����յ�����ϢΪ0ʱ�������ͻ����Ѿ��Ͽ�������,��ӡ�Ͽ���Ϣ��������Ӧ��Ҵ�����б����Ƴ�
            Debug.Log(string.Format("������{0}�Ͽ�����", m_Client.RemoteEndPoint.ToString()));
        }
    }
    #endregion
}
