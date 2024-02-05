using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 关卡详细信息UI
/// </summary>
public class UIGameLevelDetailView : UIWindowViewBase
{
    #region 变量
    /// <summary>
    /// 关卡名称
    /// </summary>
    [SerializeField]
    private Text lblGameLevelName;

    /// <summary>
    /// 关卡图
    /// </summary>
    [SerializeField]
    private Image imgDetail;

    /// <summary>
    /// 关卡奖励存储器
    /// </summary>
    [SerializeField]
    private Transform rewards_Panel;

    /// <summary>
    /// 奖励清单
    /// </summary>
    [SerializeField]
    private List<UIGameLevelRewardView> rewards;

    /// <summary>
    /// 金币收获
    /// </summary>
    [SerializeField]
    private Text lblGold;

    /// <summary>
    /// 经验收获
    /// </summary>
    [SerializeField]
    private Text lblExp;

    /// <summary>
    /// 关卡描述
    /// </summary>
    [SerializeField]
    private Text lblDescrption;

    /// <summary>
    /// 过关条件
    /// </summary>
    [SerializeField]
    private Text lblCondition;

    /// <summary>
    /// 推荐战力
    /// </summary>
    [SerializeField]
    private Text lblCommendFighting;

    /// <summary>
    /// 已选择的难度选择按钮颜色
    /// </summary>
    [SerializeField]
    private Color selectedGradeColor;

    /// <summary>
    /// 默认的难度选择按钮颜色
    /// </summary>
    private Color normalGradeColor;

    /// <summary>
    /// 当前选中的难度等级
    /// </summary>
    private GameLevelGrade m_CurrSelectGrade;

    /// <summary>
    /// 难度等级按钮数组
    /// </summary>
    [SerializeField]
    private Image[] btnGrades;

    /// <summary>
    /// 关卡编号
    /// </summary>
    private int m_GamelevelId;

    /// <summary>
    /// 进入关卡按钮点击
    /// </summary>
    /// <param name="gameLevelId"></param>
    /// <param name="grade"></param>
    public delegate void OnBtnEnterClickHandler(int gameLevelId, GameLevelGrade grade);
    public OnBtnEnterClickHandler OnBtnEnterClick;


    public delegate void OnBtnGradeClickHandler(int gameLevelId, GameLevelGrade grade);

    /// <summary>
    /// 难度选择按钮点击
    /// </summary>
    public OnBtnGradeClickHandler OnBtnGradeClick;

    #endregion

    protected override void OnStart()
    {
        base.OnStart();
        //默认为通常选中色
        if (btnGrades.Length > 0)
        {
            normalGradeColor = btnGrades[0].color;
            btnGrades[0].color = selectedGradeColor;
        }
    }

    /// <summary>
    /// 重置难度按钮颜色
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
    /// 点击难度按钮
    /// </summary>
    /// <param name="grade"></param>
    private void BtnGradeClick(GameLevelGrade grade)
    {
        if (m_CurrSelectGrade == grade)
        { return; }
        m_CurrSelectGrade = grade;

        //重置按钮颜色
        ResetBtnGradeColor();

        if (OnBtnGradeClick != null)
        {
            OnBtnGradeClick(m_GamelevelId, grade);
        }

        //选择的按钮颜色变化
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
    /// 设置关卡信息UI
    /// </summary>
    /// <param name="data"></param>
    public void SetUI(TransferData data)
    {
        //设置关卡属性
        m_GamelevelId = data.GetValue<int>(ConstDefine.GameLevelId);
        lblGameLevelName.SetText(data.GetValue<string>(ConstDefine.GameLevelName));
        lblExp.SetText(data.GetValue<int>(ConstDefine.GameLevelExp).ToString());
        lblGold.SetText(data.GetValue<int>(ConstDefine.GameLevelGold).ToString());
        lblDescrption.SetText(data.GetValue<string>(ConstDefine.GameLevelDesc));
        lblCondition.SetText(data.GetValue<string>(ConstDefine.GameLevelConditionDesc));
        lblCommendFighting.SetText(data.GetValue<int>(ConstDefine.GameLevelCommendFighting).ToString());

        //加载关卡图片
        imgDetail.overrideSprite = GameUtil.LoadSprite(SpriteSourceType.GameLevelDetail, data.GetValue<string>(ConstDefine.GameLevelDlgPic));

        //接收奖励的物品
        List<TransferData> listReward = data.GetValue<List<TransferData>>(ConstDefine.GameLevelReward);

        //开始将所有的奖励物品设定为禁用状态
        for (int i = 0; i < rewards.Count; i++)
        {
            rewards[i].gameObject.SetActive(false);
        }

        if (listReward.Count > 0)
        {
            Debug.Log("本关总奖励数：" + listReward.Count);
            //设置关卡奖励
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
