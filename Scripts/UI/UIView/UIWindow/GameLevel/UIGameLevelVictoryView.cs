using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ��Ϸ�ؿ�ͨ��UI
/// </summary>
public class UIGameLevelVictoryView : UIWindowViewBase
{
    /// <summary>
    /// ͨ��ʱ��
    /// </summary>
    [SerializeField]
    private Text lblPassTime;
    /// <summary>
    /// ��ȡ����ֵ
    /// </summary>
    [SerializeField]
    private Text lblExp;
    /// <summary>
    /// ��ȡ���ֵ
    /// </summary>
    [SerializeField]
    private Text lblGold;
    /// <summary>
    /// �����������
    /// </summary>
    [SerializeField]
    private Transform[] m_Stars;
    /// <summary>
    /// ������Ʒ
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
        //��ʼ�����еĽ�����Ʒ�趨Ϊ����״̬
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
    /// ���ùؿ�ͨ��UI
    /// </summary>
    /// <param name="data">ͨ������</param>
    public void SetUI(TransferData data)
    {
        float time = data.GetValue<float>(ConstDefine.GameLevelPassTime);

        lblPassTime.SetText(string.Format("ͨ��ʱ�䣺{0}��",time.ToString("f0")));
        lblExp.SetText(data.GetValue<int>(ConstDefine.GameLevelExp).ToString());
        lblGold.SetText(data.GetValue<int>(ConstDefine.GameLevelGold).ToString());
        //��һ�ȡ��������Ŀ
        int star = data.GetValue<int>(ConstDefine.GameLevelStar);
        for (int i = 0; i < m_Stars.Length; i++)
        {
            if (i >= star)
            { break; }
            m_Stars[i].gameObject.SetActive(true);
        }

        //���ս�������Ʒ   
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
