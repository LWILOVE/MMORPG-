using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ��ɫ������Ϣ�����๥����Ϣ��
/// </summary>
[System.Serializable]
public class RoleAttackInfo
{
    [Header("��Ч�����ƣ�����ʱ��")]
    /// <summary>
    /// ��Ч���ƣ���ɫ��������ս��������ʹ�ã�
    /// </summary>
    public string EffectName;

    /// <summary>
    /// ��Ч�Ĵ��ʱ��
    /// </summary>
    public float EffectLiftTime;

    [Header("�Ƿ�����������ʱ��")]
    /// <summary>
    /// �Ƿ�����
    /// </summary>
    public bool IsDOCameraShake = false;

    /// <summary>
    /// �����ӳ�ʱ��
    /// </summary>
    public float CameraShakeDelay = 0.2f;

    [Header("������Χ���˺�ʱ��")]
    /// <summary>
    /// ������Χ
    /// </summary>
    public float AttackRange = 0f;

    /// <summary>
    /// �õ������˵��ӳ�ʱ��
    /// </summary>
    public float HurtDelayTime = 0f;

    [Header("�ڼ������ܣ�����ID��")]
    /// <summary>
    /// ������
    /// </summary>
    public int Index;

    /// <summary>
    /// ����ID
    /// </summary>
    public int SkillId;

#if DEBUG_ROLESTATE
    /// <summary>
    /// ��ЧԤ�裨�ڲ��Ի��������ã�
    /// </summary>
    public GameObject EffectObject;
#endif

    /// <summary>
    /// ��������
    /// </summary>
    public DelayAudioClip FireAudio;

    /// <summary>
    /// �������Ľ���
    /// </summary>
    public DelayAudioClip AttactRoleAudio;

    public bool isUse = false;
}


