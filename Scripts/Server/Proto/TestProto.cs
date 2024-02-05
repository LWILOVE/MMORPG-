using System.Collections;
using System.Collections.Generic;
using UnityEngine;



/// <summary>
/// 测试协议
/// </summary>
public struct TestProto : IProto
{
    //为每个协议赋予编号，使得服务器可以感知
    public ushort ProtoCode
    { get{return 1001;}}

    public int Id;
    public string Name;
    public int Type;
    public float Price;

    

    /// <summary>
    /// 将该协议转化为Byte数组
    /// </summary>
    /// <returns></returns>
    public byte[] ToArray()
    {
        using (MMO_MemoryStream ms = new MMO_MemoryStream())
        {
            ms.WriteUShort(ProtoCode);
            ms.WriteInt(Id);
            ms.WriteUTF8String(Name);
            ms.WriteInt(Type);
            ms.WriteDouble(Price);
            return ms.ToArray();
        }
    }

    /// <summary>
    /// 将Byte数组转化为协议
    /// </summary>
    /// <param name="buffer"></param>
    /// <returns></returns>
    public static TestProto GetProto(byte[] buffer)
    {
        TestProto proto = new TestProto();
        using (MMO_MemoryStream ms = new MMO_MemoryStream(buffer))
        {
            proto.Id = ms.ReadInt();
            proto.Name = ms.ReadUTF8String();
            proto.Type = ms.ReadInt();
            proto.Price = ms.ReadFloat();
        }
            return proto;
    }
}
