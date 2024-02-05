using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ��Ϸ�ؿ�������
/// </summary>
public class GameLevelCtrl : SystemCtrlBase<GameLevelCtrl>, ISystemCtrl
{
    #region ����
    /// <summary>
    /// ����ؿ���ͼ��ͼ
    /// </summary>
    //С��ͼ
    private UIGameLevelMapView m_UIGameLevelMapView;
    //ϸ�ڴ���
    private UIGameLevelDetailView m_UIGameLevelDetailView;
    //ʤ������
    private UIGameLevelVictoryView m_UIGameLevelVictoryView;
    //ʧ�ܴ���
    private UIGameLevelFailView m_UIGameLevelFailView;

    /// <summary>
    /// ��ǰ��Ϸ�ؿ�Id
    /// </summary>
    public int CurrentGameLevelId;

    /// <summary>
    /// ��ǰ��Ϸ�ؿ��ȼ�
    /// </summary>
    public GameLevelGrade CurrentGameLevelGrade;

    /// <summary>
    /// ��ǰ��Ϸ�ؿ�ͨ��ʱ��
    /// </summary>
    public float CurrentGameLevelPassTime;

    /// <summary>
    /// ��ǰ��Ϸ�ؿ���õ��ܾ���
    /// </summary>
    public int currentGameLevelTotalExp;

    /// <summary>
    /// ��ǰ��Ϸ�ؿ���õ��ܽ��
    /// </summary>
    public int currentGameLevelTotalGold;

    /// <summary>
    /// ��ǰ�ؿ���ɱ������(�ֵı�ţ��ֵ�����)
    /// </summary>
    public Dictionary<int, int> currentGameLevelKillMonsterDic = new Dictionary<int, int>();

    /// <summary>
    /// ��ǰ��Ϸ�ؿ�����õ���Ʒ���
    /// </summary>
    public List<GetGoodsEntity> currentGameLevelGetGoodsList = new List<GetGoodsEntity>();

    private int m_GameLevelId;
    private GameLevelGrade m_Grade;

    #endregion

    /// <summary>
    /// ���캯��
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

    #region ������
    /// <summary>
    /// ���������ؽ�����Ϸ�ؿ���Ϣ
    /// </summary>
    /// <param name="buffer"></param>
    private void OnGameLevelEnterReturn(byte[] buffer)
    {
        GameLevel_EnterReturnProto proto = GameLevel_EnterReturnProto.GetProto(buffer);
        if (proto.IsSuccess)
        {
            //��ת����
            UILoadingCtrl.Instance.LoadToGameLevel(m_GameLevelId, m_Grade);
        }
    }

    /// <summary>
    /// ������������Ҹ�����Ϣ
    /// </summary>
    /// <param name="p"></param>
    /// <exception cref="NotImplementedException"></exception>
    private void OnGameLevelResurgenceReturn(byte[] buffer)
    {
        GameLevel_ResurgenceReturnProto proto = GameLevel_ResurgenceReturnProto.GetProto(buffer);
        if (proto.IsSuccess)
        {
            //��ҵ�������Ժ󣬹رո����
            m_UIGameLevelFailView.Close();
            //��Ҹ������״̬�ָ�
            GlobalInit.Instance.currentPlayer.ToResurgence();
        }
        else
        {
            //��ʾ��Ҹ���ʧ��
            MessageCtrl.Instance.Show("����ʧ��", "�����θ���ʧ�ܣ�������Դ�Ƿ���㣡");
        }
    }
    #endregion

    #region ��ť�¼�
    /// <summary>
    /// �ؿ��������ص�
    /// </summary>
    /// <param name="obj">�ؿ�ID</param>
    private void OnGameLevelItemClick(int obj)
    {
        //����Ϸ���ڣ���ȡ����ͼ
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
    /// ��ҵ���л��ѶȰ�ťʱ����
    /// </summary>
    /// <param name="gameLevelId"></param>
    /// <param name="grade"></param>
    private void OnGameLevelChangeGradeClick(int gameLevelId, GameLevelGrade grade)
    {
        //�л��Ѷ�ʱ�������ùؿ�����
        SetGameLevelDetailData(gameLevelId, grade);
    }

    /// <summary>
    /// ������Ϸ�ؿ�
    /// </summary>
    /// <param name="gameLevelId"></param>
    /// <param name="grade"></param>
    /// <exception cref="NotImplementedException"></exception>
    private void EnterGameLeveClick(int gameLevelId, GameLevelGrade grade)
    {
        //֪ͨ������Ҫ����ĳ���ؿ�
        GameLevel_EnterProto proto = new GameLevel_EnterProto();
        proto.GameLevelId = gameLevelId;
        proto.Grade = (byte)grade;
        NetWorkSocket.Instance.SendMessage(proto.ToArray());

        m_GameLevelId = gameLevelId;
        m_Grade = grade;
    }
    #endregion

    #region UI�й��¼�����
    /// <summary>
    /// ���йعؿ�����ͼ
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
    /// �򿪾���ؿ�����
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
        //����
        //1.��ȡ�½�����
        ChapterEntity entity = ChapterDBModel.Instance.Get(1);

        if (entity == null)
        {
            return;
        }

        //��ͼ
        data.SetValue(ConstDefine.ChapterId, entity.Id);
        data.SetValue(ConstDefine.ChapterName, entity.ChapterName);
        data.SetValue(ConstDefine.ChapterBG, entity.BG_Pic);

        //���ؿ�
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
    /// ����Ϸ�ؿ�ʤ������
    /// </summary>
    public void OpenGameLevelVictoryView()
    {
        UIViewUtil.Instance.LoadWindow(WindowUIType.GameLevelVictory.ToString(),
            (GameObject obj) =>
            {
                m_UIGameLevelVictoryView = obj.GetComponent<UIGameLevelVictoryView>();
            });

        //����
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

        //��ȡ�����嵥
        List<TransferData> listReward = new List<TransferData>();
        //����
        #region װ��

        if (gameLevelGradeEntity.EquipList.Count > 0)
        {
            //�Խ��������ʽ��е������򣨸��ʴ���ź��棩
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

        #region ����
        if (gameLevelGradeEntity.ItemList.Count > 0)
        {
            //�Խ��������ʽ��е������򣨸��ʴ���ź��棩
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

        #region ����
        if (gameLevelGradeEntity.MaterialList.Count > 0)
        {
            //�Խ��������ʽ��е������򣨸��ʴ���ź��棩
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
        //��ȡͨ���͵ľ�������
        currentGameLevelTotalExp += gameLevelGradeEntity.Exp;
        currentGameLevelTotalGold += gameLevelGradeEntity.Gold;
        //��ս��������͸�������
        GameLevel_VictoryProto proto = new GameLevel_VictoryProto();
        proto.GameLevelId = CurrentGameLevelId;
        proto.Grade = (byte)CurrentGameLevelGrade;
        proto.Star = (byte)star;
        proto.Exp = currentGameLevelTotalExp;
        proto.Gold = currentGameLevelTotalGold;
        //ɱ���������б�
        proto.KillTotalMonsterCount = currentGameLevelKillMonsterDic.Count;
        proto.KillMonsterList = new List<GameLevel_VictoryProto.MonsterItem>();
        foreach (var pair in currentGameLevelKillMonsterDic)
        {
            GameLevel_VictoryProto.MonsterItem monItem = new GameLevel_VictoryProto.MonsterItem();
            monItem.MonsterId = pair.Key;
            monItem.MonsterCount = pair.Value;
            proto.KillMonsterList.Add(monItem);
        }
        //�����������б�
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
        //������Ϣ��������
        NetWorkSocket.Instance.SendMessage(proto.ToArray());
    }

    /// <summary>
    /// ����Ϸ�ؿ�ʧ�ܴ���
    /// </summary>
    public void OpenGameLevelFailView()
    {
        UIViewUtil.Instance.LoadWindow(WindowUIType.GameLevelFail.ToString(),
            (GameObject obj) =>
            {
                m_UIGameLevelFailView = obj.GetComponent<UIGameLevelFailView>();
            });
        //�����������ս��ʧ����Ϣ
        GameLevel_FailProto proto = new GameLevel_FailProto();
        proto.GameLevelId = CurrentGameLevelId;
        proto.Grade = (byte)CurrentGameLevelGrade;
        NetWorkSocket.Instance.SendMessage(proto.ToArray());


        m_UIGameLevelFailView.OnResurgence = () =>
        {
            //�����Ҹ��ҲӦ�÷�����Ϣ��������
            GameLevel_ResurgenceProto mProto = new GameLevel_ResurgenceProto();
            mProto.GameLevelId = CurrentGameLevelId;
            mProto.Grade = (byte)CurrentGameLevelGrade;
            mProto.Type = 0;
            NetWorkSocket.Instance.SendMessage(mProto.ToArray());

        };
    }
    #endregion

    /// <summary>
    /// �򿪾��鴰�ڣ�δ֪������
    /// </summary>
    /// <exception cref="NotImplementedException"></exception>
    private void OpenGameLevelDetailView()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// ������Ϸ�ؿ����ݵ�����
    /// </summary>
    /// <param name="gameLevelId"></param>
    /// <param name="grade"></param>
    private void SetGameLevelDetailData(int gameLevelId, GameLevelGrade grade)
    {
        //��ȡ��Ϸ�ؿ���
        GameLevelEntity gameLevelEntity = GameLevelDBModel.Instance.Get(gameLevelId);
        //��ȡ�ؿ��Ѷȵȼ���
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
        //��ȡ�����嵥
        List<TransferData> listReward = new List<TransferData>();
        //����
        #region װ��
        if (gameLevelGradeEntity.EquipList.Count > 0)
        {
            //�Խ��������ʽ��е������򣨸��ʴ���ź��棩
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

        #region ����
        if (gameLevelGradeEntity.ItemList.Count > 0)
        {
            //�Խ��������ʽ��е������򣨸��ʴ���ź��棩
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

        #region ����

        if (gameLevelGradeEntity.MaterialList.Count > 0)
        {
            //�Խ��������ʽ��е������򣨸��ʴ���ź��棩
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
