using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 游戏关卡控制类
/// </summary>
public class GameLevelCtrl : SystemCtrlBase<GameLevelCtrl>, ISystemCtrl
{
    #region 变量
    /// <summary>
    /// 剧情关卡地图视图
    /// </summary>
    //小地图
    private UIGameLevelMapView m_UIGameLevelMapView;
    //细节窗口
    private UIGameLevelDetailView m_UIGameLevelDetailView;
    //胜利窗口
    private UIGameLevelVictoryView m_UIGameLevelVictoryView;
    //失败窗口
    private UIGameLevelFailView m_UIGameLevelFailView;

    /// <summary>
    /// 当前游戏关卡Id
    /// </summary>
    public int CurrentGameLevelId;

    /// <summary>
    /// 当前游戏关卡等级
    /// </summary>
    public GameLevelGrade CurrentGameLevelGrade;

    /// <summary>
    /// 当前游戏关卡通关时间
    /// </summary>
    public float CurrentGameLevelPassTime;

    /// <summary>
    /// 当前游戏关卡获得的总经验
    /// </summary>
    public int currentGameLevelTotalExp;

    /// <summary>
    /// 当前游戏关卡获得的总金币
    /// </summary>
    public int currentGameLevelTotalGold;

    /// <summary>
    /// 当前关卡的杀怪详情(怪的编号，怪的数量)
    /// </summary>
    public Dictionary<int, int> currentGameLevelKillMonsterDic = new Dictionary<int, int>();

    /// <summary>
    /// 当前游戏关卡所获得的物品类别
    /// </summary>
    public List<GetGoodsEntity> currentGameLevelGetGoodsList = new List<GetGoodsEntity>();

    private int m_GameLevelId;
    private GameLevelGrade m_Grade;

    #endregion

    /// <summary>
    /// 构造函数
    /// </summary>
    public GameLevelCtrl()
    {
        SocketDispatcher.Instance.AddEventListener(ProtoCodeDef.GameLevel_EnterReturn, OnGameLevelEnterReturn);
        SocketDispatcher.Instance.AddEventListener(ProtoCodeDef.GameLevel_ResurgenceReturn, OnGameLevelResurgenceReturn);
    }

    ~GameLevelCtrl()
    {
        SocketDispatcher.Instance.RemoveEventListener(ProtoCodeDef.GameLevel_EnterReturn, OnGameLevelEnterReturn);
        SocketDispatcher.Instance.RemoveEventListener(ProtoCodeDef.GameLevel_ResurgenceReturn, OnGameLevelResurgenceReturn);
    }

    #region 服务器
    /// <summary>
    /// 服务器返回进入游戏关卡消息
    /// </summary>
    /// <param name="buffer"></param>
    private void OnGameLevelEnterReturn(byte[] buffer)
    {
        GameLevel_EnterReturnProto proto = GameLevel_EnterReturnProto.GetProto(buffer);
        if (proto.IsSuccess)
        {
            //跳转场景
            UILoadingCtrl.Instance.LoadToGameLevel(m_GameLevelId, m_Grade);
        }
    }

    /// <summary>
    /// 服务器返回玩家复活消息
    /// </summary>
    /// <param name="p"></param>
    /// <exception cref="NotImplementedException"></exception>
    private void OnGameLevelResurgenceReturn(byte[] buffer)
    {
        GameLevel_ResurgenceReturnProto proto = GameLevel_ResurgenceReturnProto.GetProto(buffer);
        if (proto.IsSuccess)
        {
            //玩家点击复活以后，关闭复活窗口
            m_UIGameLevelFailView.Close();
            //玩家复活进行状态恢复
            GlobalInit.Instance.currentPlayer.ToResurgence();
        }
        else
        {
            //提示玩家复活失败
            MessageCtrl.Instance.Show("复活失败", "您本次复活失败，请检查资源是否充足！");
        }
    }
    #endregion

    #region 按钮事件
    /// <summary>
    /// 关卡子项点击回调
    /// </summary>
    /// <param name="obj">关卡ID</param>
    private void OnGameLevelItemClick(int obj)
    {
        //打开游戏窗口，并取得视图
        UIViewUtil.Instance.LoadWindow(WindowUIType.GameLevelDetail.ToString(),
            (GameObject obj) =>
            {
                m_UIGameLevelDetailView = obj.GetComponent<UIGameLevelDetailView>();
            });
        m_UIGameLevelDetailView.OnBtnGradeClick = OnGameLevelChangeGradeClick;
        m_UIGameLevelDetailView.OnBtnEnterClick = EnterGameLeveClick;

        SetGameLevelDetailData(obj, GameLevelGrade.Normal);
    }

    /// <summary>
    /// 玩家点击切换难度按钮时触发
    /// </summary>
    /// <param name="gameLevelId"></param>
    /// <param name="grade"></param>
    private void OnGameLevelChangeGradeClick(int gameLevelId, GameLevelGrade grade)
    {
        //切换难度时重新设置关卡数据
        SetGameLevelDetailData(gameLevelId, grade);
    }

    /// <summary>
    /// 进入游戏关卡
    /// </summary>
    /// <param name="gameLevelId"></param>
    /// <param name="grade"></param>
    /// <exception cref="NotImplementedException"></exception>
    private void EnterGameLeveClick(int gameLevelId, GameLevelGrade grade)
    {
        //通知服务器要进入某个关卡
        GameLevel_EnterProto proto = new GameLevel_EnterProto();
        proto.GameLevelId = gameLevelId;
        proto.Grade = (byte)grade;
        NetWorkSocket.Instance.SendMessage(proto.ToArray());

        m_GameLevelId = gameLevelId;
        m_Grade = grade;
    }
    #endregion

    #region UI有关事件方法
    /// <summary>
    /// 打开有关关卡的视图
    /// </summary>
    /// <param name="type"></param>
    public void OpenView(WindowUIType type)
    {
        switch (type)
        {
            case WindowUIType.GameLevelMap:
                OpenGameLevelMapView();
                break;
            case WindowUIType.GameLevelVictory:
                OpenGameLevelVictoryView();
                break;
            case WindowUIType.GameLevelFail:
                OpenGameLevelFailView();
                break;

        }
    }

    /// <summary>
    /// 打开剧情关卡窗口
    /// </summary>
    /// <exception cref="NotImplementedException"></exception>
    private void OpenGameLevelMapView()
    {
        UIViewUtil.Instance.LoadWindow(WindowUIType.GameLevelMap.ToString(),
            (GameObject obj) =>
            {
                m_UIGameLevelMapView = obj.GetComponent<UIGameLevelMapView>();
            });
        TransferData data = new TransferData();
        //读表
        //1.获取章节数据
        ChapterEntity entity = ChapterDBModel.Instance.Get(1);

        if (entity == null)
        {
            return;
        }

        //读图
        data.SetValue(ConstDefine.ChapterId, entity.Id);
        data.SetValue(ConstDefine.ChapterName, entity.ChapterName);
        data.SetValue(ConstDefine.ChapterBG, entity.BG_Pic);

        //读关卡
        List<GameLevelEntity> gameLevelList = GameLevelDBModel.Instance.GetListByChapterId(entity.Id);

        if (gameLevelList != null && gameLevelList.Count > 0)
        {
            List<TransferData> list = new List<TransferData>();

            for (int i = 0; i < gameLevelList.Count; i++)
            {
                TransferData childData = new TransferData();
                childData.SetValue(ConstDefine.GameLevelId, gameLevelList[i].Id);
                childData.SetValue(ConstDefine.GameLevelName, gameLevelList[i].Name);
                childData.SetValue(ConstDefine.GameLevelPosition, gameLevelList[i].Position);
                childData.SetValue(ConstDefine.GameLevelIsBoss, gameLevelList[i].isBoss);
                childData.SetValue(ConstDefine.GameLevelIco, gameLevelList[i].Ico);
                list.Add(childData);
            }

            data.SetValue(ConstDefine.GameLevelList, list);
        }

        m_UIGameLevelMapView.SetUI(data, OnGameLevelItemClick);
    }

    /// <summary>
    /// 打开游戏关卡胜利窗口
    /// </summary>
    public void OpenGameLevelVictoryView()
    {
        UIViewUtil.Instance.LoadWindow(WindowUIType.GameLevelVictory.ToString(),
            (GameObject obj) =>
            {
                m_UIGameLevelVictoryView = obj.GetComponent<UIGameLevelVictoryView>();
            });

        //读表
        GameLevelEntity gameLevelEntity = GameLevelDBModel.Instance.Get(CurrentGameLevelId);
        GameLevelGradeEntity gameLevelGradeEntity = GameLevelGradeDBModel.Instance.GetEntityByGameLevelIdAndGrade(CurrentGameLevelId, CurrentGameLevelGrade);

        TransferData data = new TransferData();
        data.SetValue(ConstDefine.GameLevelExp, gameLevelGradeEntity.Exp);
        data.SetValue(ConstDefine.GameLevelGold, gameLevelGradeEntity.Gold);
        data.SetValue(ConstDefine.GameLevelPassTime, CurrentGameLevelPassTime);
        int star = 1;
        if (CurrentGameLevelPassTime <= gameLevelGradeEntity.Star2)
        {
            star = 3;
        }
        else if (CurrentGameLevelPassTime <= gameLevelGradeEntity.Star1)
        {
            star = 2;
        }

        data.SetValue(ConstDefine.GameLevelStar, star);

        //获取奖励清单
        List<TransferData> listReward = new List<TransferData>();
        //奖励
        #region 装备

        if (gameLevelGradeEntity.EquipList.Count > 0)
        {
            //对奖励按概率进行倒序排序（概率大的排后面）
            gameLevelGradeEntity.EquipList.Sort(
                (GoodsEntity entity1, GoodsEntity entity2) =>
                {
                    if (entity1.Probability < entity2.Probability)
                    {
                        return -1;
                    }
                    else if (entity1.Probability < entity2.Probability)
                    {
                        return 0;
                    }
                    else
                    {
                        return 1;
                    }
                });
            GoodsEntity entity = gameLevelGradeEntity.EquipList[0];

            TransferData EquipReward = new TransferData();
            EquipReward.SetValue(ConstDefine.GoodsId, entity.Id);
            EquipReward.SetValue(ConstDefine.GoodsName, entity.Name);
            EquipReward.SetValue(ConstDefine.GoodsType, GoodsType.Equip);
            listReward.Add(EquipReward);

            currentGameLevelGetGoodsList.Add(new GetGoodsEntity() { GoodsType = 0, GoodId = entity.Id, GoodsCount = 1 });
        }
        #endregion

        #region 道具
        if (gameLevelGradeEntity.ItemList.Count > 0)
        {
            //对奖励按概率进行倒序排序（概率大的排后面）
            gameLevelGradeEntity.ItemList.Sort(
                (GoodsEntity entity1, GoodsEntity entity2) =>
                {
                    if (entity1.Probability < entity2.Probability)
                    {
                        return -1;
                    }
                    else if (entity1.Probability < entity2.Probability)
                    {
                        return 0;
                    }
                    else
                    {
                        return 1;
                    }
                });
            GoodsEntity entity = gameLevelGradeEntity.ItemList[0];

            TransferData ItemReward = new TransferData();
            ItemReward.SetValue(ConstDefine.GoodsId, entity.Id);
            ItemReward.SetValue(ConstDefine.GoodsName, entity.Name);
            ItemReward.SetValue(ConstDefine.GoodsType, GoodsType.Item);
            listReward.Add(ItemReward);

            currentGameLevelGetGoodsList.Add(new GetGoodsEntity() { GoodsType = 1, GoodId = entity.Id, GoodsCount = 1 });
        }
        #endregion

        #region 材料
        if (gameLevelGradeEntity.MaterialList.Count > 0)
        {
            //对奖励按概率进行倒序排序（概率大的排后面）
            gameLevelGradeEntity.MaterialList.Sort(
                (GoodsEntity entity1, GoodsEntity entity2) =>
                {
                    if (entity1.Probability < entity2.Probability)
                    {
                        return -1;
                    }
                    else if (entity1.Probability < entity2.Probability)
                    {
                        return 0;
                    }
                    else
                    {
                        return 1;
                    }
                });
            GoodsEntity entity = gameLevelGradeEntity.MaterialList[0];

            TransferData MaterialReward = new TransferData();
            MaterialReward.SetValue(ConstDefine.GoodsId, entity.Id);
            MaterialReward.SetValue(ConstDefine.GoodsName, entity.Name);
            MaterialReward.SetValue(ConstDefine.GoodsType, GoodsType.Material);
            listReward.Add(MaterialReward);
            currentGameLevelGetGoodsList.Add(new GetGoodsEntity() { GoodsType = 2, GoodId = entity.Id, GoodsCount = 1 });
        }
        #endregion
        data.SetValue(ConstDefine.GameLevelReward, listReward);
        m_UIGameLevelVictoryView.SetUI(data);

        //=========================
        //获取通过送的经验与金币
        currentGameLevelTotalExp += gameLevelGradeEntity.Exp;
        currentGameLevelTotalGold += gameLevelGradeEntity.Gold;
        //将战斗结果发送给服务器
        GameLevel_VictoryProto proto = new GameLevel_VictoryProto();
        proto.GameLevelId = CurrentGameLevelId;
        proto.Grade = (byte)CurrentGameLevelGrade;
        proto.Star = (byte)star;
        proto.Exp = currentGameLevelTotalExp;
        proto.Gold = currentGameLevelTotalGold;
        //杀怪数量与列表
        proto.KillTotalMonsterCount = currentGameLevelKillMonsterDic.Count;
        proto.KillMonsterList = new List<GameLevel_VictoryProto.MonsterItem>();
        foreach (var pair in currentGameLevelKillMonsterDic)
        {
            GameLevel_VictoryProto.MonsterItem monItem = new GameLevel_VictoryProto.MonsterItem();
            monItem.MonsterId = pair.Key;
            monItem.MonsterCount = pair.Value;
            proto.KillMonsterList.Add(monItem);
        }
        //道具数量与列表
        proto.GoodsTotalCount = currentGameLevelGetGoodsList.Count;
        proto.GetGoodsList = new List<GameLevel_VictoryProto.GoodsItem>();
        for (int i = 0; i < currentGameLevelGetGoodsList.Count; i++)
        {
            GameLevel_VictoryProto.GoodsItem item = new GameLevel_VictoryProto.GoodsItem();
            item.GoodsType = (byte)currentGameLevelGetGoodsList[i].GoodsType;
            item.GoodsId = currentGameLevelGetGoodsList[i].GoodId;
            item.GoodsCount = currentGameLevelGetGoodsList[i].GoodsCount;
            proto.GetGoodsList.Add(item);
        }
        //发送信息到服务器
        NetWorkSocket.Instance.SendMessage(proto.ToArray());
    }

    /// <summary>
    /// 打开游戏关卡失败窗口
    /// </summary>
    public void OpenGameLevelFailView()
    {
        UIViewUtil.Instance.LoadWindow(WindowUIType.GameLevelFail.ToString(),
            (GameObject obj) =>
            {
                m_UIGameLevelFailView = obj.GetComponent<UIGameLevelFailView>();
            });
        //向服务器发送战斗失败消息
        GameLevel_FailProto proto = new GameLevel_FailProto();
        proto.GameLevelId = CurrentGameLevelId;
        proto.Grade = (byte)CurrentGameLevelGrade;
        NetWorkSocket.Instance.SendMessage(proto.ToArray());


        m_UIGameLevelFailView.OnResurgence = () =>
        {
            //如果玩家复活，也应该发送消息给服务器
            GameLevel_ResurgenceProto mProto = new GameLevel_ResurgenceProto();
            mProto.GameLevelId = CurrentGameLevelId;
            mProto.Grade = (byte)CurrentGameLevelGrade;
            mProto.Type = 0;
            NetWorkSocket.Instance.SendMessage(mProto.ToArray());

        };
    }
    #endregion

    /// <summary>
    /// 打开剧情窗口（未知方法）
    /// </summary>
    /// <exception cref="NotImplementedException"></exception>
    private void OpenGameLevelDetailView()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// 设置游戏关卡内容的数据
    /// </summary>
    /// <param name="gameLevelId"></param>
    /// <param name="grade"></param>
    private void SetGameLevelDetailData(int gameLevelId, GameLevelGrade grade)
    {
        //读取游戏关卡表
        GameLevelEntity gameLevelEntity = GameLevelDBModel.Instance.Get(gameLevelId);
        //读取关卡难度等级表
        GameLevelGradeEntity gameLevelGradeEntity = GameLevelGradeDBModel.Instance.GetEntityByGameLevelIdAndGrade(gameLevelId, grade);

        if (gameLevelEntity == null || gameLevelGradeEntity == null)
        { return; }

        TransferData data = new TransferData();
        data.SetValue(ConstDefine.GameLevelId, gameLevelEntity.Id);
        data.SetValue(ConstDefine.GameLevelDlgPic, gameLevelEntity.DlgPic);
        data.SetValue(ConstDefine.GameLevelName, gameLevelEntity.Name);
        data.SetValue(ConstDefine.GameLevelExp, gameLevelGradeEntity.Exp);
        data.SetValue(ConstDefine.GameLevelGold, gameLevelGradeEntity.Gold);
        data.SetValue(ConstDefine.GameLevelDesc, gameLevelGradeEntity.Desc);
        data.SetValue(ConstDefine.GameLevelConditionDesc, gameLevelGradeEntity.ConditionDesc);
        data.SetValue(ConstDefine.GameLevelCommendFighting, gameLevelGradeEntity.CommendFighting);
        //获取奖励清单
        List<TransferData> listReward = new List<TransferData>();
        //奖励
        #region 装备
        if (gameLevelGradeEntity.EquipList.Count > 0)
        {
            //对奖励按概率进行倒序排序（概率大的排后面）
            gameLevelGradeEntity.EquipList.Sort(
                (GoodsEntity entity1, GoodsEntity entity2) =>
                {
                    if (entity1.Probability < entity2.Probability)
                    {
                        return -1;
                    }
                    else if (entity1.Probability < entity2.Probability)
                    {
                        return 0;
                    }
                    else
                    {
                        return 1;
                    }
                });
            GoodsEntity entity = gameLevelGradeEntity.EquipList[0];

            TransferData EquipReward = new TransferData();
            EquipReward.SetValue(ConstDefine.GoodsId, entity.Id);
            EquipReward.SetValue(ConstDefine.GoodsName, entity.Name);
            EquipReward.SetValue(ConstDefine.GoodsType, GoodsType.Equip);
            listReward.Add(EquipReward);
        }
        #endregion

        #region 道具
        if (gameLevelGradeEntity.ItemList.Count > 0)
        {
            //对奖励按概率进行倒序排序（概率大的排后面）
            gameLevelGradeEntity.ItemList.Sort(
                (GoodsEntity entity1, GoodsEntity entity2) =>
                {
                    if (entity1.Probability < entity2.Probability)
                    {
                        return -1;
                    }
                    else if (entity1.Probability < entity2.Probability)
                    {
                        return 0;
                    }
                    else
                    {
                        return 1;
                    }
                });
            GoodsEntity entity = gameLevelGradeEntity.ItemList[0];

            TransferData ItemReward = new TransferData();
            ItemReward.SetValue(ConstDefine.GoodsId, entity.Id);
            ItemReward.SetValue(ConstDefine.GoodsName, entity.Name);
            ItemReward.SetValue(ConstDefine.GoodsType, GoodsType.Item);
            listReward.Add(ItemReward);
        }
        #endregion

        #region 材料

        if (gameLevelGradeEntity.MaterialList.Count > 0)
        {
            //对奖励按概率进行倒序排序（概率大的排后面）
            gameLevelGradeEntity.MaterialList.Sort(
                (GoodsEntity entity1, GoodsEntity entity2) =>
                {
                    if (entity1.Probability < entity2.Probability)
                    {
                        return -1;
                    }
                    else if (entity1.Probability < entity2.Probability)
                    {
                        return 0;
                    }
                    else
                    {
                        return 1;
                    }
                });
            GoodsEntity entity = gameLevelGradeEntity.MaterialList[0];

            TransferData MaterialReward = new TransferData();
            MaterialReward.SetValue(ConstDefine.GoodsId, entity.Id);
            MaterialReward.SetValue(ConstDefine.GoodsName, entity.Name);
            MaterialReward.SetValue(ConstDefine.GoodsType, GoodsType.Material);
            listReward.Add(MaterialReward);
        }
        #endregion
        data.SetValue(ConstDefine.GameLevelReward, listReward);

        m_UIGameLevelDetailView.SetUI(data);
    }
}
