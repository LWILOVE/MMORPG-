using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

/// <summary>
/// NPCUI条
/// </summary>
public class NPCHeaderBarView : MonoBehaviour
{
    /// <summary>
    /// 昵称
    /// </summary>
    [SerializeField]
    private Text lblNickName;

    /// <summary>
    /// 对齐的目标点
    /// </summary>
    private Transform m_Target;

    private RectTransform m_Trans;

    /// <summary>
    /// 说话背景图
    /// </summary>
    [SerializeField]
    private Image imgTalkBG;

    /// <summary>
    /// 说话的文本
    /// </summary>
    [SerializeField]
    private Text lblTalkText;

    /// <summary>
    /// 说话缩放动画
    /// </summary>
    private Tween m_ScaleTween;

    /// <summary>
    /// 说话背景摇晃动画
    /// </summary>
    private Tween m_RotateTween;
    private void Awake()
    {
        imgTalkBG.gameObject.SetActive(false);

        imgTalkBG.transform.localScale = Vector3.zero;
        m_ScaleTween = imgTalkBG.transform.DOScale(Vector3.one, 0.2f).SetAutoKill(false).Pause().SetEase(GlobalInit.Instance.UIAnimationCurve).OnComplete(
            ()=>
            {
                lblTalkText.DOText(m_TalkText, 0.5f);
            }).OnRewind(
            ()=>
            {
                imgTalkBG.gameObject.SetActive(false);
            });

        imgTalkBG.transform.localEulerAngles = new Vector3(0, 0, -10);
        m_RotateTween = imgTalkBG.transform.DOLocalRotate(new Vector3(0, 0, 20), 1f, RotateMode.LocalAxisAdd).SetAutoKill(false).Pause().SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutBack);
        
    }


    /// <summary>
    /// 说话结束时长
    /// </summary>
    private float m_TalkStopTime = 0;

    /// <summary>
    /// 是否说话
    /// </summary>
    private bool m_IsTalk;

    private string m_TalkText;
    /// <summary>
    /// NPC开始说话
    /// </summary>
    /// <param name="text">内容</param>
    /// <param name="time">存在时间</param>
    public void Talk(string text, float time)
    {
        m_TalkStopTime = Time.time + time;

        m_IsTalk = true;
        lblTalkText.text = "";
        m_TalkText = text;

        imgTalkBG.gameObject.SetActive(true);

        m_ScaleTween.PlayForward();
        m_RotateTween.Play();
    }

    private void Start()
    {
        m_Trans = UILoadingCtrl.Instance.CurrentUIScene.CurrCanvas.GetComponent<RectTransform>();
        
    }

    private void Update()
    {
        if (m_Trans == null || m_Target == null)
        {
            return;
        }

        //设置名字跟随
        //获取屏幕坐标
        Vector2 screenPos = Camera.main.WorldToScreenPoint(m_Target.position);

        //接收的UI世界坐标
        Vector3 pos;

        //将屏幕坐标转化为UGUI的世界坐标
        if (RectTransformUtility.ScreenPointToWorldPointInRectangle(m_Trans, screenPos, UICamera.Instance.Camera, out pos))
        {
            transform.position = pos;
        }

        if( m_IsTalk&&Time.time > m_TalkStopTime)
        {
            m_IsTalk = false;
            m_ScaleTween.PlayBackwards();
        }
    }

    public void Init(Transform target, string nickName) 
    { 
        m_Target = target;
        lblNickName.text = nickName;
    }
}
