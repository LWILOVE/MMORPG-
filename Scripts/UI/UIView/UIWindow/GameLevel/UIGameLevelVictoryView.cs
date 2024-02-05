using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 游戏关卡通关UI
/// </summary>
public class UIGameLevelVictoryView : UIWindowViewBase
{
    /// <summary>
    /// 通关时间
    /// </summary>
    [SerializeField]
    private Text lblPassTime;
    /// <summary>
    /// 获取经验值
    /// </summary>
    [SerializeField]
    private Text lblExp;
    /// <summary>
    /// 获取金币值
    /// </summary>
    [SerializeField]
    private Text lblGold;
    /// <summary>
    /// 达成条件数量
    /// </summary>
    [SerializeField]
    private Transform[] m_Stars;
    /// <summary>
    /// 奖励物品
    /// </summary>
    [SerializeField]
    private UIGameLevelRewardView[] m_RewardView;

    protected override void OnAwake()
    {
        base.OnAwake();
        for (int i = 0; i < m_Stars.Length; i++)
        {
            m_Stars[i].gameObject.SetActive(false);
        }
        //开始将所有的奖励物品设定为禁用状态
        for (int i = 0; i < m_RewardView.Length; i++)
        {
            m_RewardView[i].gameObject.SetActive(false);
        }

    }

    protected override void OnStart()
    {
        base.OnStart();
    }

    protected override void OnBtnClick(GameObject go)
    {
        base.OnBtnClick(go);
        switch (go.name)
        {
            case "Btn_Return":
                GlobalInit.Instance.currentPlayer.ToIdle();
                GlobalInit.Instance.currentPlayer.Attack.IsAutoFight = false;
                GlobalInit.Instance.currentPlayer.LockEnemy = null;
                UILoadingCtrl.Instance.LoadToWorldMap(PlayerCtrl.Instance.LastInWorldMapId);    
                break;
        }
    }

    /// <summary>
    /// 设置关卡通关UI
    /// </summary>
    /// <param name="data">通关数据</param>
    public void SetUI(TransferData data)
    {
        float time = data.GetValue<float>(ConstDefine.GameLevelPassTime);

        lblPassTime.SetText(string.Format("通关时间：{0}秒",time.ToString("f0")));
        lblExp.SetText(data.GetValue<int>(ConstDefine.GameLevelExp).ToString());
        lblGold.SetText(data.GetValue<int>(ConstDefine.GameLevelGold).ToString());
        //玩家获取的星星数目
        int star = data.GetValue<int>(ConstDefine.GameLevelStar);
        for (int i = 0; i < m_Stars.Length; i++)
        {
            if (i >= star)
            { break; }
            m_Stars[i].gameObject.SetActive(true);
        }

        //接收奖励的物品   
        List<TransferData> listReward = data.GetValue<List<TransferData>>(ConstDefine.GameLevelReward);
        if (listReward.Count > 0)
        {
            for (int i = 0; i < listReward.Count; i++)
            {
                m_RewardView[i].gameObject.SetActive(true);
                m_RewardView[i].SetUI(listReward[i].GetValue<string>(ConstDefine.GoodsName),
                    listReward[i].GetValue<int>(ConstDefine.GoodsId),
                    listReward[i].GetValue<GoodsType>(ConstDefine.GoodsType));
            }
        }
    }
}
