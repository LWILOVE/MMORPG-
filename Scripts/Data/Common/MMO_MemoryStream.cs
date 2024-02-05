using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using System.Text;

/// <summary>
/// ����ת���ࣺbyte short int long float decimal bool string
/// </summary>
public class MMO_MemoryStream : MemoryStream
{
    /// <summary>
    /// �յĹ��캯��
    /// </summary>
    public MMO_MemoryStream()
    { }

    /// <summary>
    /// ���Ը���Ĺ��캯��
    /// </summary>
    /// <param name="buffer"></param>
    
    public MMO_MemoryStream(byte[] buffer) : base(buffer)
    { }

    #region short
    /// <summary>
    /// �����ж�ȡһ��short����
    /// </summary>
    /// <returns></returns>
    public short ReadShort()
    {
        byte[] arr = new byte[2];
        base.Read(arr, 0, 2);
        return BitConverter.ToInt16(arr, 0);
    }
    /// <summary>
    /// ��һ��short����д������
    /// </summary>
    /// <param name="value"></param>
    public void WriteShort(short value)
    {
        byte[] arr = BitConverter.GetBytes(value);
        base.Write(arr, 0, arr.Length);
    }
    #endregion

    #region ushort
    /// <summary>
    /// �����ж�ȡһ���޷���short����
    /// </summary>
    /// <returns></returns>
    public ushort ReadUShort()
    {
        byte[] arr = new byte[2];
        base.Read(arr,0,2);
        return BitConverter.ToUInt16(arr,0);
    }
    /// <summary>
    /// ��һ���޷���short����д������
    /// </summary>
    /// <param name="value"></param>
    public void WriteUShort(ushort value)
    {
        byte[] arr = BitConverter.GetBytes(value);
        base.Write(arr,0,arr.Length);
    }
    #endregion

    #region int
    /// <summary>
    /// �����ж�ȡһ��int����
    /// </summary>
    /// <returns></returns>
    public int ReadInt()
    {
        byte[] arr = new byte[4];
        base.Read(arr, 0, 4);
        return BitConverter.ToInt32(arr, 0);
    }
    /// <summary>
    /// ��һ��int����д������
    /// </summary>
    /// <param name="value"></param>
    public void WriteInt(int value)
    {
        byte[] arr = BitConverter.GetBytes(value);
        base.Write(arr, 0, arr.Length);
    }
    #endregion

    #region uint
    /// <summary>
    /// �����ж�ȡһ���޷���int����
    /// </summary>
    /// <returns></returns>
    public uint ReadUInt()
    {
        byte[] arr = new byte[4];
        base.Read(arr, 0, 4);
        return BitConverter.ToUInt32(arr, 0);
    }
    /// <summary>
    /// ��һ���޷���int����д������
    /// </summary>
    /// <param name="value"></param>
    public void WriteInt(uint value)
    {
        byte[] arr = BitConverter.GetBytes(value);
        base.Write(arr, 0, arr.Length);
    }
    #endregion

    #region long
    /// <summary>
    /// �����ж�ȡһ��Long����
    /// </summary>
    /// <returns></returns>
    public long ReadLong()
    {
        byte[] arr = new byte[8];
        base.Read(arr, 0, 8);
        return BitConverter.ToInt64(arr, 0);
    }
    /// <summary>
    /// ��һ��long����д������
    /// </summary>
    /// <param name="value"></param>
    public void WriteLong(long value)
    {
        byte[] arr = BitConverter.GetBytes(value);
        base.Write(arr, 0, arr.Length);
    }
    #endregion

    #region ulong
    /// <summary>
    /// �����ж�ȡһ���޷���long����
    /// </summary>
    /// <returns></returns>
    public ulong ReadULong()
    {
        byte[] arr = new byte[8];
        base.Read(arr, 0, 8);
        return BitConverter.ToUInt64(arr, 0);
    }
    /// <summary>
    /// ��һ���޷���long����д������
    /// </summary>
    /// <param name="value"></param>
    public void WriteULong(ulong value)
    {
        byte[] arr = BitConverter.GetBytes(value);
        base.Write(arr, 0, arr.Length);
    }
    #endregion

    #region float
    /// <summary>
    /// �����ж�ȡһ��float����
    /// </summary>
    /// <returns></returns>
    public float ReadFloat()
    {
        byte[] arr = new byte[4];
        base.Read(arr, 0, 4);
        return BitConverter.ToSingle(arr, 0);
    }
    /// <summary>
    /// ��һ��float����д������
    /// </summary>
    /// <param name="value"></param>
    public void WriteFloat(float value)
    {
        byte[] arr = BitConverter.GetBytes(value);
        base.Write(arr, 0, arr.Length);
    }
    #endregion

    #region double
    /// <summary>
    /// �����ж�ȡһ��double����
    /// </summary>
    /// <returns></returns>
    public double ReadDouble()
    {
        byte[] arr = new byte[8];
        base.Read(arr, 0, 8);
        return BitConverter.ToDouble(arr, 0);
    }
    /// <summary>
    /// ��һ��double����д������
    /// </summary>
    /// <param name="value"></param>
    public void WriteDouble(double value)
    {
        byte[] arr = BitConverter.GetBytes(value);
        base.Write(arr, 0, arr.Length);
    }
    #endregion

    #region bool
    /// <summary>
    /// �����ж�ȡһ��bool����
    /// </summary>
    /// <returns></returns>
    public bool ReadBool()
    {
        //����bool���ͷ��ؽ��
        return base.ReadByte() == 1;
    }
    /// <summary>
    /// ��һ��bool����д������
    /// </summary>
    /// <param name="value"></param>
    public void WriteBool(bool value)
    {
        base.WriteByte((byte)(value == true?1:0));
    }
    #endregion

    #region UIF8string
    /// <summary>
    /// �����ж�ȡһ��string����
    /// </summary>
    /// <returns></returns>
    public string ReadUTF8String()
    {
        //��Ԥ֪����Ҫ��ȡ�೤������
        //ԭ��������д�����ݵ�ʱ�����������short��С������
        //ͬʱ�����ǽ��ַ����ĳ��ȴ������ַ���������λ��
        //��˾�����ǰ��֪�ַ�����������Ҫ��ϵͳä̽
        ushort len = this.ReadUShort();
        //Ȼ���ٽ����ݷŽ�������
        byte[] arr = new byte[len];
        base.Read(arr,0,len);
        return Encoding.UTF8.GetString(arr);
    }
    /// <summary>
    /// ��һ�����Ȳ�����65535��string����д������
    /// </summary>
    /// <param name="str"></param>
    /// <exception cref="InvalidCastException"></exception>
    public void WriteUTF8String(string str)
    {
        //��stringת��Ϊbyte��������
        byte[] arr = Encoding.UTF8.GetBytes(str);
        if (arr.Length > 65535)
        {
            //��������������̫�����׳�һ���쳣������һ��
            throw new InvalidCastException("Ҫд���ַ�������");
        }
        WriteUShort((ushort)arr.Length);
        base.Write(arr,0,arr.Length);
    }
    #endregion
}
