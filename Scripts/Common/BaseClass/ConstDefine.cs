using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 基本常量定义类
/// 字段特点：值固定
/// 命名特点：字段功能_字段名（功能通常是所用的地方（脚本），字段名则多是游戏物体名）
/// </summary>
[XLua.LuaCallCSharp]
public class ConstDefine
{
    //用户ID
    public const string LogOn_AccountID = "LogOn_AccountID";
    //用户账号
    public const string LogOn_AccountUserName = "LogOn_AccountUserName";
    //用户密码
    public const string LogOn_AccountPwd = "LogOn_AccountPwd";

    //登录按钮
    public const string UILogOnView_Btn_LogOn = "UILogOnView_Btn_LogOn";
    //前往注册界面按钮
    public const string UILogOnView_Btn_ToReg = "UILogOnView_Btn_ToReg";

    //注册按钮
    public const string UIRegView_Btn_Reg = "UILogOnView_Btn_Reg";
    //返回登录界面按钮
    public const string UIRegView_Btn_ToLogOn = "UILogOnView_Btn_ToLogOn";

    //选择区服按钮
    public const string UIGameServerEnterView_Btn_SelectGameServer = "UIGameServerEnterView_Btn_SelectGameServer";
    //进入游戏按钮
    public const string UIGameServerEnterView_Btn_EnterGame = "UIGameServerEnterView_Btn_EnterGame";

    //属性名称术语================================================
    public const string JobId = "JobId";
    public const string NickName = "NickName";
    public const string Level = "Level";
    public const string Fighting = "Fighting";
    public const string Money = "Money";
    public const string Gold = "Gold";
    public const string CurrHP = "CurrHP";
    public const string MaxHP = "MaxHP";
    public const string CurrMP = "CurrMP";
    public const string MaxMP = "MaxMP";
    public const string CurrExp = "CurrExp";
    public const string MaxExp = "MaxExp";
    public const string Attack = "Attack";
    public const string Defense = "Defense";
    public const string Dodge = "Dodge";
    public const string Hit = "Hit";
    public const string Cri = "Cri";
    public const string Res = "Res";

    //关卡相关##################################################
    public const string ChapterId = "ChapterId";
    public const string ChapterName = "ChapterName";
    public const string ChapterBG = "ChapterBG";
    public const string GameLevelList = "GameLevelList";

    public const string GameLevelId = "GameLeveld";
    public const string GameLevelName= "GameLevelName";
    public const string GameLevelPosition = "GameLevelPosition";
    public const string GameLevelIsBoss = "GameLevelIsBoss";
    public const string GameLevelIco = "GameLevelIco";
    public const string GameLevelDlgPic = "GameLevelDlgPic";
    public const string GameLevelExp = "GameLevelExp";
    public const string GameLevelGold = "GameLevelGold";
    public const string GameLevelDesc= "GameLevelDesc";
    public const string GameLevelConditionDesc = "GameLevelConditionDesc";
    public const string GameLevelCommendFighting = "GameLevelCommendFighting";
    public const string GameLevelPassTime = "GameLevelPassTime";

    public const string GameLevelReward = "GameLevelReward";
    public const string GoodsId = "GoodsId";
    public const string GoodsName = "GoodsName";
    public const string GoodsType = "GoodsType";
    public const string GameLevelStar = "GameLevelStar";

    //###############角色技能相关
    public const string SkillSlotsNo = "SkillSlotsNo";
    public const string SkillId = "SkillId";
    public const string SkillLevel = "SkillLevel";
    public const string SkillPic = "SkillPic";
    public const string SkillCDTime = "SkillCDTime";

    //=================世界地图
    public const string WorldMapList = "WorldMapList";
    public const string WorldMapPosition = "WorldMapPosition";
    public const string WorldMapId = "WorldMapId";
    public const string WorldMapName = "WorldMapName";
    public const string WorldMapIco = "WorldMapIco";
    
    //观察者相关
    //充值完成
    public const string RechargeOK = "RechargeOK";
}
