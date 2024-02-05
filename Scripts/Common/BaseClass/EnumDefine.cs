using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ��ʼ����ʽ
/// </summary>
public enum InitWay
{
    /// <summary>
    /// �༭���ó�ʼ����ʽ�����������������ݴ���ķ�ʽ��
    /// </summary>
    AuthorUse,
    /// <summary>
    /// �û�ʹ�õĳ�ʼ����ʽ��Ҫ�����������ݴ���ķ�ʽ��
    /// </summary>
    UserUse
}

/// <summary>
/// ��ǰ����
/// </summary>
public enum Language
{
    /// <summary>
    /// ����
    /// </summary>
    CN,
    /// <summary>
    /// Ӣ��
    /// </summary>
    EN
}

/// <summary>
/// ��Ϸ�ؿ�������
/// </summary>
public enum ConfigCode
{
    /// <summary>
    /// ��Ϸ�ؿ�����
    /// </summary>
    GameLevelMenu,
    /// <summary>
    /// ��Ϸ�ؿ��޵�
    /// </summary>
    GameLevelSuperman
}

/// <summary>
/// ��Ч����:ע��������Ӧ�þ���ͬ��Ч��
/// </summary>
public enum UIAudioEffectType
{
    /// <summary>
    /// ��ť���
    /// </summary>
    ButtonClick,
    /// <summary>
    /// �ر�UI
    /// </summary>
    UIClose,
}

public enum PlayerType
{
    /// <summary>
    /// ����ģʽ
    /// </summary>
    PVE,
    /// <summary>
    /// ���˻�ս
    /// </summary>
    PVP
}

/// <summary>
/// ���ֱ仯ö��
/// </summary>
public enum ValueChangeType
{
    /// <summary>
    /// ����
    /// </summary>
    Add,
    /// <summary>
    /// ����
    /// </summary>
    Subtract
}

/// <summary>
/// ��Ʒ����
/// </summary>
public enum GoodsType
{
    /// <summary>
    /// װ��
    /// </summary>
    Equip,
    /// <summary>
    /// ����
    /// </summary>
    Item,
    /// <summary>
    /// ����
    /// </summary>
    Material
}

/// <summary>
/// ��Ϸ�ؿ��Ѷȵȼ�
/// </summary>
public enum GameLevelGrade
{
    /// <summary>
    /// ��ͨ�Ѷ�
    /// </summary>
    Normal,
    /// <summary>
    /// �����Ѷ�
    /// </summary>
    Hard,
    /// <summary>
    /// �����Ѷ�
    /// </summary>
    Hell
}

/// <summary>
/// ��Ϣ����
/// </summary>
public enum MessageViewType
{
    /// <summary>
    /// ֻ��ʾȷ����ť
    /// </summary>
    Ok,
    /// <summary>
    /// ��ʾȷ����ȡ����ť
    /// </summary>
    OkAndCancel
}

/// <summary>
/// ��������
/// </summary>
public enum SceneType
{
    /// <summary>
    /// ��ʼ������
    /// </summary>
    Init,
    /// <summary>
    /// ���س���
    /// </summary>
    Loading,
    /// <summary>
    /// ��¼����
    /// </summary>
    LogOn,
    /// <summary>
    /// ѡ���ɫ����
    /// </summary>
    SelectRole,
    /// <summary>
    /// ���ǳ���
    /// </summary>
    MainCity,
    /// <summary>
    /// ����
    /// </summary>
    HangZhou,
    /// <summary>
    /// ������
    /// </summary>
    ChangAn,
    /// <summary>
    /// ����ɽ
    /// </summary>
    WuYiShan,
    /// <summary>
    /// ÷��
    /// </summary>
    MeiLing,
    /// <summary>
    /// ����ɽ
    /// </summary>
    ChangBaiShan,
    /// <summary>
    /// ѩ�ǹ�
    /// </summary>
    XueLangGu,
    /// <summary>
    /// �ػ�
    /// </summary>
    DunHuang,
    /// <summary>
    /// ��Ȫ
    /// </summary>
    JiuQuan,
    /// <summary>
    /// ����Ĺ��һ��
    /// </summary>
    DiWangMuLV1,
    /// <summary>
    /// �ؿ�����
    /// </summary>
    ShanGu
}

/// <summary>
/// ����UI����
/// </summary>
public enum SceneUIType
{
    /// <summary>
    /// ��
    /// </summary>
    None,
    /// <summary>
    /// ��¼����
    /// </summary>
    LogOn,
    /// <summary>
    /// ���ؽ���
    /// </summary>
    Loading,
    /// <summary>
    /// ���ǽ���
    /// </summary>
    MainCity,
    /// <summary>
    /// ѡ�˳���
    /// </summary>
    SelectRole,
    /// <summary>
    /// ����UIRoot
    /// </summary>
    TestUIRoot,
    /// <summary>
    /// ��ʼ������ 
    /// </summary>
    Init
}

/// <summary>
/// ��Դ�������ļ���
/// </summary>
public enum ResourcesType
{
    /// <summary>
    /// ����UI
    /// </summary>
    UIScene,
    /// <summary>
    /// ����
    /// </summary>
    UIWindow,
    /// <summary>
    /// ��ɫ
    /// </summary>
    Role,
    /// <summary>
    /// ��Ч
    /// </summary>
    Effect,
    /// <summary>
    /// ����
    /// </summary>
    Other,
    /// <summary>
    /// ��������
    /// </summary>
    UIWindowsChild,
    /// <summary>
    /// ĳЩ���ڲ��Եĵ���
    /// </summary>
    Useless
}

/// <summary>
/// ����ͼ����Դ����
/// </summary>
public enum SpriteSourceType
{
    /// <summary>
    /// ��Ϸ�ؿ�ͼ
    /// </summary>
    GameLevelIco,
    /// <summary>
    /// ��Ϸ�ؿ�ϸ��ͼ
    /// </summary>
    GameLevelDetail,
    /// <summary>
    /// �����ͼͼ
    /// </summary>
    WorldMapIco,
    /// <summary>
    /// С��ͼͼ
    /// </summary>
    WorldMapSmall
}

/// <summary>
/// ����UI����
/// </summary>
public enum WindowUIType
{
    /// <summary>
    /// ��¼����
    /// </summary>
    LogOn,
    /// <summary>
    /// ע�ᴰ��
    /// </summary>
    LogReg,
    /// <summary>
    /// ע�ᴰ��
    /// </summary>
    Reg,
    /// <summary>
    /// ��ɫ��Ϣ����
    /// </summary>
    RoleInfo,
    /// <summary>
    /// ��ɫ������Ϣ����
    /// </summary>
    RoleBaseNews,
    /// <summary>
    /// �������봰��
    /// </summary>
    GameServerEnter,
    /// <summary>
    /// ����ѡ�񴰿� 
    /// </summary>
    GameServerSelect,
    /// <summary>
    /// ����ؿ���ͼ
    /// </summary>
    GameLevelMap,
    /// <summary>
    /// ����ؿ�����  
    /// </summary>
    GameLevelDetail,
    /// <summary>
    /// ��Ϸͨ��
    /// </summary>
    GameLevelVictory,
    /// <summary>
    /// ��Ϸʧ��
    /// </summary>
    GameLevelFail,
    /// <summary>
    /// �����ͼ
    /// </summary>
    WorldMap,
    /// <summary>
    /// �����ͼս��
    /// </summary>
    WorldMapFail,
    None
}
/// <summary>
/// UI��������
/// </summary>
public enum WindowUIContainerType
{
    /// <summary>
    /// ����
    /// </summary>
    TopLeft,
    /// <summary>
    /// ����
    /// </summary>
    TopRight,
    /// <summary>
    /// ����
    /// </summary>
    BottomLeft,
    /// <summary>
    /// ����
    /// </summary>
    BottomRight,
    /// <summary>
    /// ����
    /// </summary>
    Center
}

/// <summary>
/// ���ڳ�������
/// </summary>
public enum WindowShowStyle
{
    Normal,//����
    CenterToBig,//���м��������
    FromTop,//�Ӷ�������
    FromDown,//�ӵײ�����
    FromLeft,//������
    FromRight//���ҵ���
}

/// <summary>
/// ��ɫ����ö�٣���1��ʼ��
/// </summary>
public enum RoleType
{
    /// <summary>
    /// ������
    /// </summary>
    None,
    /// <summary>
    /// ��ǰ���
    /// </summary>
    MainPlayer,
    /// <summary>
    /// ����
    /// </summary>
    Monster,
    /// <summary>
    /// �������
    /// </summary>
    OtherPlayer
}
/// <summary>
/// ��ɫ״̬ö��
/// </summary>
public enum RoleState
{
    /// <summary>
    /// ��״̬
    /// </summary>
    None,
    /// <summary>
    /// ����״̬
    /// </summary>
    Idle,
    /// <summary>
    /// �ܶ�״̬
    /// </summary>
    Run,
    /// <summary>
    /// ����״̬
    /// </summary>
    Attack,
    /// <summary>
    /// ����״̬
    /// </summary>
    Hurt,
    /// <summary>
    /// ����״̬
    /// </summary>
    Die,
    /// <summary>
    /// ѡ��ORʤ��״̬
    /// </summary>
    Select
}

/// <summary>
/// ��ɫ��������
/// </summary>
public enum RoleAttackType
{
    /// <summary>
    /// ������
    /// </summary>
    PhyAttack,
    /// <summary>
    /// ���ܹ���
    /// </summary>
    SkillAttack
}
/// <summary>
/// ��ɫ����״̬��
/// </summary>
public enum RoleAnimatorState
{
    Idle_Normal = 1,
    Idle_Fight,
    Run,
    Hurt,
    Die,
    Select,
    XiuXian,
    Died,
    PhyAttack1 =11,
    PhyAttack2,
    PhyAttack3,
    Skill1,
    Skill2,
    Skill3,
    Skill4,
    Skill5,
    Skill6=19
}

/// <summary>
/// ��ɫ����״̬
/// </summary>
public enum RoleIdleState
{
    IdleNormal,
    IdleFight
}
/// <summary>
/// ��ɫ����״̬������
/// </summary>
public enum ToAnimatorCondition
{
    ToIdleNormal,
    ToIdleFight,
    ToRun,
    ToHurt,
    ToDie,
    ToDied,
    ToSelect,
    ToXiuXian,
    ToPhyAttack,
    ToSkill,
    CurrState
}