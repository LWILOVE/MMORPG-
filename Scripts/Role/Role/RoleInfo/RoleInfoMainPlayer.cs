using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ������Ϣ��
/// </summary>
public class RoleInfoMainPlayer : RoleInfoBase
{
    public byte JobId;//ְҵ���
    public int Money;//Ԫ��
    public int TotalRechargeMoney;//�ۼƳ�ֵ��Ԫ������

    public RoleInfoMainPlayer():base()
    {
    }
    
    /// <summary>
    /// ���ݷ��������صĽ�ɫ��Ϣ�����Ǹ�ֵ
    /// </summary>
    /// <param name="proto"></param>
    public RoleInfoMainPlayer(RoleOperation_SelectRoleInfoReturnProto roleInfoProto)
    {

        JobId = roleInfoProto.JobId;//ְҵ
        Money = roleInfoProto.Money;//Ԫ��
        TotalRechargeMoney=roleInfoProto.TotalRechargeMoney;//�ۼƳ�ֵԪ��
        Gold = roleInfoProto.Gold;//���

        RoldId = roleInfoProto.RoldId; //��ɫ���
        RoleNickName = roleInfoProto.RoleNickName; //��ɫ�ǳ�
        CurrExp = roleInfoProto.Exp; //��ǰ����
        Level = roleInfoProto.Level;//�ȼ�
        MaxHP = roleInfoProto.MaxHP; //���HP
        MaxMP = roleInfoProto.MaxMP; //���MP
        CurrHP = roleInfoProto.CurrHP; //��ǰHP
        CurrMP = roleInfoProto.CurrMP; //��ǰMP
        Attack = roleInfoProto.Attack; //������
        Defense = roleInfoProto.Defense; //����
        Hit = roleInfoProto.Hit; //����
        Dodge = roleInfoProto.Dodge; //����
        Cri = roleInfoProto.Cri; //����
        Res = roleInfoProto.Res; //����
        Fighting = roleInfoProto.Fighting; //�ۺ�ս����

        skillList = new List<RoleInfoSkill>();//��ɫ�����б�
    }

    /// <summary>
    /// ��������ѧ��ļ���
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
