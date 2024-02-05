using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// �ؿ���ϸ��ϢUI
/// </summary>
public class UIGameLevelDetailView : UIWindowViewBase
{
    #region ����
    /// <summary>
    /// �ؿ�����
    /// </summary>
    [SerializeField]
    private Text lblGameLevelName;

    /// <summary>
    /// �ؿ�ͼ
    /// </summary>
    [SerializeField]
    private Image imgDetail;

    /// <summary>
    /// �ؿ������洢��
    /// </summary>
    [SerializeField]
    private Transform rewards_Panel;

    /// <summary>
    /// �����嵥
    /// </summary>
    [SerializeField]
    private List<UIGameLevelRewardView> rewards;

    /// <summary>
    /// ����ջ�
    /// </summary>
    [SerializeField]
    private Text lblGold;

    /// <summary>
    /// �����ջ�
    /// </summary>
    [SerializeField]
    private Text lblExp;

    /// <summary>
    /// �ؿ�����
    /// </summary>
    [SerializeField]
    private Text lblDescrption;

    /// <summary>
    /// ��������
    /// </summary>
    [SerializeField]
    private Text lblCondition;

    /// <summary>
    /// �Ƽ�ս��
    /// </summary>
    [SerializeField]
    private Text lblCommendFighting;

    /// <summary>
    /// ��ѡ����Ѷ�ѡ��ť��ɫ
    /// </summary>
    [SerializeField]
    private Color selectedGradeColor;

    /// <summary>
    /// Ĭ�ϵ��Ѷ�ѡ��ť��ɫ
    /// </summary>
    private Color normalGradeColor;

    /// <summary>
    /// ��ǰѡ�е��Ѷȵȼ�
    /// </summary>
    private GameLevelGrade m_CurrSelectGrade;

    /// <summary>
    /// �Ѷȵȼ���ť����
    /// </summary>
    [SerializeField]
    private Image[] btnGrades;

    /// <summary>
    /// �ؿ����
    /// </summary>
    private int m_GamelevelId;

    /// <summary>
    /// ����ؿ���ť���
    /// </summary>
    /// <param name="gameLevelId"></param>
    /// <param name="grade"></param>
    public delegate void OnBtnEnterClickHandler(int gameLevelId, GameLevelGrade grade);
    public OnBtnEnterClickHandler OnBtnEnterClick;


    public delegate void OnBtnGradeClickHandler(int gameLevelId, GameLevelGrade grade);

    /// <summary>
    /// �Ѷ�ѡ��ť���
    /// </summary>
    public OnBtnGradeClickHandler OnBtnGradeClick;

    #endregion

    protected override void OnStart()
    {
        base.OnStart();
        //Ĭ��Ϊͨ��ѡ��ɫ
        if (btnGrades.Length > 0)
        {
            normalGradeColor = btnGrades[0].color;
            btnGrades[0].color = selectedGradeColor;
        }
    }

    /// <summary>
    /// �����ѶȰ�ť��ɫ
    /// </summary>
    private void ResetBtnGradeColor()
    {
        for (int i = 0; i < btnGrades.Length; i++)
        {
            btnGrades[i].color = normalGradeColor;
        }
    }
    protected override void OnBtnClick(GameObject go)
    {
        base.OnBtnClick(go);
        switch (go.name)
        {
            case "Btn_Normal":
                BtnGradeClick(GameLevelGrade.Normal);
                break;
            case "Btn_Hard":
                BtnGradeClick(GameLevelGrade.Hard);
                break;
            case "Btn_Hell":
                BtnGradeClick(GameLevelGrade.Hell);
                break;
            case "Btn_Enter":
                if (OnBtnEnterClick != null)
                {
                    OnBtnEnterClick(m_GamelevelId, m_CurrSelectGrade);
                }
                break;
        }
    }

    /// <summary>
    /// ����ѶȰ�ť
    /// </summary>
    /// <param name="grade"></param>
    private void BtnGradeClick(GameLevelGrade grade)
    {
        if (m_CurrSelectGrade == grade)
        { return; }
        m_CurrSelectGrade = grade;

        //���ð�ť��ɫ
        ResetBtnGradeColor();

        if (OnBtnGradeClick != null)
        {
            OnBtnGradeClick(m_GamelevelId, grade);
        }

        //ѡ��İ�ť��ɫ�仯
        if (btnGrades.Length == 3)
        {
            btnGrades[(int)grade].color = selectedGradeColor;
        }
    }

    protected override void BeforeOnDestroy()
    {
        base.BeforeOnDestroy();
        lblGameLevelName = null;
        imgDetail = null;
        rewards = null;
        lblGold = null;
        lblExp = null;
        lblDescrption = null;
        lblCondition = null;
        lblCommendFighting = null;
    }

    /// <summary>
    /// ���ùؿ���ϢUI
    /// </summary>
    /// <param name="data"></param>
    public void SetUI(TransferData data)
    {
        //���ùؿ�����
        m_GamelevelId = data.GetValue<int>(ConstDefine.GameLevelId);
        lblGameLevelName.SetText(data.GetValue<string>(ConstDefine.GameLevelName));
        lblExp.SetText(data.GetValue<int>(ConstDefine.GameLevelExp).ToString());
        lblGold.SetText(data.GetValue<int>(ConstDefine.GameLevelGold).ToString());
        lblDescrption.SetText(data.GetValue<string>(ConstDefine.GameLevelDesc));
        lblCondition.SetText(data.GetValue<string>(ConstDefine.GameLevelConditionDesc));
        lblCommendFighting.SetText(data.GetValue<int>(ConstDefine.GameLevelCommendFighting).ToString());

        //���عؿ�ͼƬ
        imgDetail.overrideSprite = GameUtil.LoadSprite(SpriteSourceType.GameLevelDetail, data.GetValue<string>(ConstDefine.GameLevelDlgPic));

        //���ս�������Ʒ
        List<TransferData> listReward = data.GetValue<List<TransferData>>(ConstDefine.GameLevelReward);

        //��ʼ�����еĽ�����Ʒ�趨Ϊ����״̬
        for (int i = 0; i < rewards.Count; i++)
        {
            rewards[i].gameObject.SetActive(false);
        }

        if (listReward.Count > 0)
        {
            Debug.Log("�����ܽ�������" + listReward.Count);
            //���ùؿ�����
            for (int i = 0; i < listReward.Count; i++)
            {
                if (i > (rewards.Count-1) || rewards[i]==null)
                {
                    AssetBundleMgr.Instance.LoadOrDownload<GameObject>(string.Format("Download/Prefab/UIPrefab/UIWindow/GameLevel/ImgReward.assetbundle"), "ImgReward",
    (GameObject obj) =>
        {
            rewards.Add(GameObject.Instantiate(obj,rewards_Panel).GetComponent<UIGameLevelRewardView>());
        }, type: 0);
                }

                rewards[i].gameObject.SetActive(true);
                rewards[i].SetUI(listReward[i].GetValue<string>(ConstDefine.GoodsName),
                    listReward[i].GetValue<int>(ConstDefine.GoodsId),
                    listReward[i].GetValue<GoodsType>(ConstDefine.GoodsType));
            }
        }
    }
}
