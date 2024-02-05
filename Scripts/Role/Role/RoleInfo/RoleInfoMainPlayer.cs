using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 主角信息类
/// </summary>
public class RoleInfoMainPlayer : RoleInfoBase
{
    public byte JobId;//职业编号
    public int Money;//元宝
    public int TotalRechargeMoney;//累计充值的元宝数量

    public RoleInfoMainPlayer():base()
    {
    }
    
    /// <summary>
    /// 根据服务器返回的角色信息给主角赋值
    /// </summary>
    /// <param name="proto"></param>
    public RoleInfoMainPlayer(RoleOperation_SelectRoleInfoReturnProto roleInfoProto)
    {

        JobId = roleInfoProto.JobId;//职业
        Money = roleInfoProto.Money;//元宝
        TotalRechargeMoney=roleInfoProto.TotalRechargeMoney;//累计充值元宝
        Gold = roleInfoProto.Gold;//金币

        RoldId = roleInfoProto.RoldId; //角色编号
        RoleNickName = roleInfoProto.RoleNickName; //角色昵称
        CurrExp = roleInfoProto.Exp; //当前经验
        Level = roleInfoProto.Level;//等级
        MaxHP = roleInfoProto.MaxHP; //最大HP
        MaxMP = roleInfoProto.MaxMP; //最大MP
        CurrHP = roleInfoProto.CurrHP; //当前HP
        CurrMP = roleInfoProto.CurrMP; //当前MP
        Attack = roleInfoProto.Attack; //攻击力
        Defense = roleInfoProto.Defense; //防御
        Hit = roleInfoProto.Hit; //命中
        Dodge = roleInfoProto.Dodge; //闪避
        Cri = roleInfoProto.Cri; //暴击
        Res = roleInfoProto.Res; //抗性
        Fighting = roleInfoProto.Fighting; //综合战斗力

        skillList = new List<RoleInfoSkill>();//角色技能列表
    }

    /// <summary>
    /// 加载主角学会的技能
    /// </summary>
    /// <param name="proto"></param>
    public void LoadSkill(RoleData_SkillReturnProto proto)
    {
        skillList.Clear();
        for (int i = 0; i < proto.CurrSkillDataList.Count; i++)
        {
            skillList.Add(new RoleInfoSkill() 
            {
                SkillId = proto.CurrSkillDataList[i].SkillId,
                SkillLevel = proto.CurrSkillDataList[i].SkillLevel,
                SlotsNo = proto.CurrSkillDataList[i].SlotsNo
            });
        }
    }
}
