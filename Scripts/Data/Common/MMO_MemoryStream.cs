using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using System.Text;

/// <summary>
/// 数据转换类：byte short int long float decimal bool string
/// </summary>
public class MMO_MemoryStream : MemoryStream
{
    /// <summary>
    /// 空的构造函数
    /// </summary>
    public MMO_MemoryStream()
    { }

    /// <summary>
    /// 来自父类的构造函数
    /// </summary>
    /// <param name="buffer"></param>
    
    public MMO_MemoryStream(byte[] buffer) : base(buffer)
    { }

    #region short
    /// <summary>
    /// 从流中读取一个short数据
    /// </summary>
    /// <returns></returns>
    public short ReadShort()
    {
        byte[] arr = new byte[2];
        base.Read(arr, 0, 2);
        return BitConverter.ToInt16(arr, 0);
    }
    /// <summary>
    /// 将一个short数据写入流中
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
    /// 从流中读取一个无符号short数据
    /// </summary>
    /// <returns></returns>
    public ushort ReadUShort()
    {
        byte[] arr = new byte[2];
        base.Read(arr,0,2);
        return BitConverter.ToUInt16(arr,0);
    }
    /// <summary>
    /// 将一个无符号short数据写入流中
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
    /// 从流中读取一个int数据
    /// </summary>
    /// <returns></returns>
    public int ReadInt()
    {
        byte[] arr = new byte[4];
        base.Read(arr, 0, 4);
        return BitConverter.ToInt32(arr, 0);
    }
    /// <summary>
    /// 将一个int数据写入流中
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
    /// 从流中读取一个无符号int数据
    /// </summary>
    /// <returns></returns>
    public uint ReadUInt()
    {
        byte[] arr = new byte[4];
        base.Read(arr, 0, 4);
        return BitConverter.ToUInt32(arr, 0);
    }
    /// <summary>
    /// 将一个无符号int数据写入流中
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
    /// 从流中读取一个Long数据
    /// </summary>
    /// <returns></returns>
    public long ReadLong()
    {
        byte[] arr = new byte[8];
        base.Read(arr, 0, 8);
        return BitConverter.ToInt64(arr, 0);
    }
    /// <summary>
    /// 将一个long数据写入流中
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
    /// 从流中读取一个无符号long数据
    /// </summary>
    /// <returns></returns>
    public ulong ReadULong()
    {
        byte[] arr = new byte[8];
        base.Read(arr, 0, 8);
        return BitConverter.ToUInt64(arr, 0);
    }
    /// <summary>
    /// 将一个无符号long数据写入流中
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
    /// 从流中读取一个float数据
    /// </summary>
    /// <returns></returns>
    public float ReadFloat()
    {
        byte[] arr = new byte[4];
        base.Read(arr, 0, 4);
        return BitConverter.ToSingle(arr, 0);
    }
    /// <summary>
    /// 将一个float数据写入流中
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
    /// 从流中读取一个double数据
    /// </summary>
    /// <returns></returns>
    public double ReadDouble()
    {
        byte[] arr = new byte[8];
        base.Read(arr, 0, 8);
        return BitConverter.ToDouble(arr, 0);
    }
    /// <summary>
    /// 将一个double数据写入流中
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
    /// 从流中读取一个bool数据
    /// </summary>
    /// <returns></returns>
    public bool ReadBool()
    {
        //根据bool类型返回结果
        return base.ReadByte() == 1;
    }
    /// <summary>
    /// 将一个bool数据写入流中
    /// </summary>
    /// <param name="value"></param>
    public void WriteBool(bool value)
    {
        base.WriteByte((byte)(value == true?1:0));
    }
    #endregion

    #region UIF8string
    /// <summary>
    /// 从流中读取一个string数组
    /// </summary>
    /// <returns></returns>
    public string ReadUTF8String()
    {
        //先预知我们要读取多长的数据
        //原理：在我们写入数据的时候，限制最大是short大小的数据
        //同时，我们将字符串的长度存入其字符串的先两位中
        //因此就能提前得知字符串长而不需要让系统盲探
        ushort len = this.ReadUShort();
        //然后再将数据放进数组里
        byte[] arr = new byte[len];
        base.Read(arr,0,len);
        return Encoding.UTF8.GetString(arr);
    }
    /// <summary>
    /// 将一个长度不超过65535的string数据写入流中
    /// </summary>
    /// <param name="str"></param>
    /// <exception cref="InvalidCastException"></exception>
    public void WriteUTF8String(string str)
    {
        //将string转化为byte数组类型
        byte[] arr = Encoding.UTF8.GetBytes(str);
        if (arr.Length > 65535)
        {
            //若传过来的数据太大，则抛出一个异常，提醒一下
            throw new InvalidCastException("要写的字符串过大");
        }
        WriteUShort((ushort)arr.Length);
        base.Write(arr,0,arr.Length);
    }
    #endregion
}
