//===================================================
//作    者：边涯  http://www.u3dol.com  QQ群：87481002
//创建时间：2018-02-25 22:40:38
//备    注：
//===================================================
using System.Collections;
using System.Collections.Generic;
using System;

/// <summary>
/// 客户端发送进入世界地图场景消息
/// </summary>
public struct WorldMap_RoleEnterProto : IProto
{
    public ushort ProtoCode { get { return 13001; } }

    public int WorldMapSceneId; //世界地图场景编号

    public byte[] ToArray()
    {
        using (MMO_MemoryStream ms = new MMO_MemoryStream())
        {
            ms.WriteUShort(ProtoCode);
            ms.WriteInt(WorldMapSceneId);
            return ms.ToArray();
        }
    }

    public static WorldMap_RoleEnterProto GetProto(byte[] buffer)
    {
        WorldMap_RoleEnterProto proto = new WorldMap_RoleEnterProto();
        using (MMO_MemoryStream ms = new MMO_MemoryStream(buffer))
        {
            proto.WorldMapSceneId = ms.ReadInt();
        }
        return proto;
    }
}