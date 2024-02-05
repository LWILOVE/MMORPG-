using System.Collections;
using System.Collections.Generic;
using UnityEngine;



/// <summary>
/// ����Э��
/// </summary>
public struct TestProto : IProto
{
    //Ϊÿ��Э�鸳���ţ�ʹ�÷��������Ը�֪
    public ushort ProtoCode
    { get{return 1001;}}

    public int Id;
    public string Name;
    public int Type;
    public float Price;

    

    /// <summary>
    /// ����Э��ת��ΪByte����
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
    /// ��Byte����ת��ΪЭ��
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
