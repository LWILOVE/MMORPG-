using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 初始化方式
/// </summary>
public enum InitWay
{
    /// <summary>
    /// 编辑器用初始化方式（即不进行网络数据传输的方式）
    /// </summary>
    AuthorUse,
    /// <summary>
    /// 用户使用的初始化方式（要进行网络数据传输的方式）
    /// </summary>
    UserUse
}

/// <summary>
/// 当前语言
/// </summary>
public enum Language
{
    /// <summary>
    /// 中文
    /// </summary>
    CN,
    /// <summary>
    /// 英文
    /// </summary>
    EN
}

/// <summary>
/// 游戏关卡配置码
/// </summary>
public enum ConfigCode
{
    /// <summary>
    /// 游戏关卡功能
    /// </summary>
    GameLevelMenu,
    /// <summary>
    /// 游戏关卡无敌
    /// </summary>
    GameLevelSuperman
}

/// <summary>
/// 音效类型:注：类型名应该尽量同音效名
/// </summary>
public enum UIAudioEffectType
{
    /// <summary>
    /// 按钮点击
    /// </summary>
    ButtonClick,
    /// <summary>
    /// 关闭UI
    /// </summary>
    UIClose,
}

public enum PlayerType
{
    /// <summary>
    /// 副本模式
    /// </summary>
    PVE,
    /// <summary>
    /// 多人混战
    /// </summary>
    PVP
}

/// <summary>
/// 数字变化枚举
/// </summary>
public enum ValueChangeType
{
    /// <summary>
    /// 增加
    /// </summary>
    Add,
    /// <summary>
    /// 减少
    /// </summary>
    Subtract
}

/// <summary>
/// 物品类型
/// </summary>
public enum GoodsType
{
    /// <summary>
    /// 装备
    /// </summary>
    Equip,
    /// <summary>
    /// 道具
    /// </summary>
    Item,
    /// <summary>
    /// 材料
    /// </summary>
    Material
}

/// <summary>
/// 游戏关卡难度等级
/// </summary>
public enum GameLevelGrade
{
    /// <summary>
    /// 普通难度
    /// </summary>
    Normal,
    /// <summary>
    /// 困难难度
    /// </summary>
    Hard,
    /// <summary>
    /// 地狱难度
    /// </summary>
    Hell
}

/// <summary>
/// 消息类型
/// </summary>
public enum MessageViewType
{
    /// <summary>
    /// 只显示确定按钮
    /// </summary>
    Ok,
    /// <summary>
    /// 显示确定和取消按钮
    /// </summary>
    OkAndCancel
}

/// <summary>
/// 场景类型
/// </summary>
public enum SceneType
{
    /// <summary>
    /// 初始化场景
    /// </summary>
    Init,
    /// <summary>
    /// 加载场景
    /// </summary>
    Loading,
    /// <summary>
    /// 登录场景
    /// </summary>
    LogOn,
    /// <summary>
    /// 选择角色场景
    /// </summary>
    SelectRole,
    /// <summary>
    /// 主城场景
    /// </summary>
    MainCity,
    /// <summary>
    /// 杭州
    /// </summary>
    HangZhou,
    /// <summary>
    /// 长安城
    /// </summary>
    ChangAn,
    /// <summary>
    /// 武夷山
    /// </summary>
    WuYiShan,
    /// <summary>
    /// 梅岭
    /// </summary>
    MeiLing,
    /// <summary>
    /// 长白山
    /// </summary>
    ChangBaiShan,
    /// <summary>
    /// 雪狼谷
    /// </summary>
    XueLangGu,
    /// <summary>
    /// 敦煌
    /// </summary>
    DunHuang,
    /// <summary>
    /// 酒泉
    /// </summary>
    JiuQuan,
    /// <summary>
    /// 帝王墓第一层
    /// </summary>
    DiWangMuLV1,
    /// <summary>
    /// 关卡界面
    /// </summary>
    ShanGu
}

/// <summary>
/// 场景UI类型
/// </summary>
public enum SceneUIType
{
    /// <summary>
    /// 空
    /// </summary>
    None,
    /// <summary>
    /// 登录界面
    /// </summary>
    LogOn,
    /// <summary>
    /// 加载界面
    /// </summary>
    Loading,
    /// <summary>
    /// 主城界面
    /// </summary>
    MainCity,
    /// <summary>
    /// 选人场景
    /// </summary>
    SelectRole,
    /// <summary>
    /// 测试UIRoot
    /// </summary>
    TestUIRoot,
    /// <summary>
    /// 初始化界面 
    /// </summary>
    Init
}

/// <summary>
/// 资源归属的文件夹
/// </summary>
public enum ResourcesType
{
    /// <summary>
    /// 场景UI
    /// </summary>
    UIScene,
    /// <summary>
    /// 窗口
    /// </summary>
    UIWindow,
    /// <summary>
    /// 角色
    /// </summary>
    Role,
    /// <summary>
    /// 特效
    /// </summary>
    Effect,
    /// <summary>
    /// 其他
    /// </summary>
    Other,
    /// <summary>
    /// 窗口子项
    /// </summary>
    UIWindowsChild,
    /// <summary>
    /// 某些用于测试的道具
    /// </summary>
    Useless
}

/// <summary>
/// 精灵图集资源种类
/// </summary>
public enum SpriteSourceType
{
    /// <summary>
    /// 游戏关卡图
    /// </summary>
    GameLevelIco,
    /// <summary>
    /// 游戏关卡细节图
    /// </summary>
    GameLevelDetail,
    /// <summary>
    /// 世界地图图
    /// </summary>
    WorldMapIco,
    /// <summary>
    /// 小地图图
    /// </summary>
    WorldMapSmall
}

/// <summary>
/// 窗口UI类型
/// </summary>
public enum WindowUIType
{
    /// <summary>
    /// 登录窗口
    /// </summary>
    LogOn,
    /// <summary>
    /// 注册窗口
    /// </summary>
    LogReg,
    /// <summary>
    /// 注册窗口
    /// </summary>
    Reg,
    /// <summary>
    /// 角色信息窗口
    /// </summary>
    RoleInfo,
    /// <summary>
    /// 角色基础信息窗口
    /// </summary>
    RoleBaseNews,
    /// <summary>
    /// 区服进入窗口
    /// </summary>
    GameServerEnter,
    /// <summary>
    /// 区服选择窗口 
    /// </summary>
    GameServerSelect,
    /// <summary>
    /// 剧情关卡地图
    /// </summary>
    GameLevelMap,
    /// <summary>
    /// 剧情关卡详情  
    /// </summary>
    GameLevelDetail,
    /// <summary>
    /// 游戏通关
    /// </summary>
    GameLevelVictory,
    /// <summary>
    /// 游戏失败
    /// </summary>
    GameLevelFail,
    /// <summary>
    /// 世界地图
    /// </summary>
    WorldMap,
    /// <summary>
    /// 世界地图战败
    /// </summary>
    WorldMapFail,
    None
}
/// <summary>
/// UI容器类型
/// </summary>
public enum WindowUIContainerType
{
    /// <summary>
    /// 左上
    /// </summary>
    TopLeft,
    /// <summary>
    /// 右上
    /// </summary>
    TopRight,
    /// <summary>
    /// 左下
    /// </summary>
    BottomLeft,
    /// <summary>
    /// 右下
    /// </summary>
    BottomRight,
    /// <summary>
    /// 居中
    /// </summary>
    Center
}

/// <summary>
/// 窗口出厂动画
/// </summary>
public enum WindowShowStyle
{
    Normal,//正常
    CenterToBig,//从中间慢慢变大
    FromTop,//从顶部降落
    FromDown,//从底部升起
    FromLeft,//从左到右
    FromRight//从右到左
}

/// <summary>
/// 角色类型枚举（从1开始）
/// </summary>
public enum RoleType
{
    /// <summary>
    /// 无类型
    /// </summary>
    None,
    /// <summary>
    /// 当前玩家
    /// </summary>
    MainPlayer,
    /// <summary>
    /// 怪物
    /// </summary>
    Monster,
    /// <summary>
    /// 其他玩家
    /// </summary>
    OtherPlayer
}
/// <summary>
/// 角色状态枚举
/// </summary>
public enum RoleState
{
    /// <summary>
    /// 无状态
    /// </summary>
    None,
    /// <summary>
    /// 待机状态
    /// </summary>
    Idle,
    /// <summary>
    /// 跑动状态
    /// </summary>
    Run,
    /// <summary>
    /// 攻击状态
    /// </summary>
    Attack,
    /// <summary>
    /// 受伤状态
    /// </summary>
    Hurt,
    /// <summary>
    /// 死亡状态
    /// </summary>
    Die,
    /// <summary>
    /// 选角OR胜利状态
    /// </summary>
    Select
}

/// <summary>
/// 角色攻击类型
/// </summary>
public enum RoleAttackType
{
    /// <summary>
    /// 物理攻击
    /// </summary>
    PhyAttack,
    /// <summary>
    /// 技能攻击
    /// </summary>
    SkillAttack
}
/// <summary>
/// 角色动画状态名
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
/// 角色待机状态
/// </summary>
public enum RoleIdleState
{
    IdleNormal,
    IdleFight
}
/// <summary>
/// 角色动画状态参数名
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