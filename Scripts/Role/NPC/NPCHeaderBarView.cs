using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

/// <summary>
/// NPCUI��
/// </summary>
public class NPCHeaderBarView : MonoBehaviour
{
    /// <summary>
    /// �ǳ�
    /// </summary>
    [SerializeField]
    private Text lblNickName;

    /// <summary>
    /// �����Ŀ���
    /// </summary>
    private Transform m_Target;

    private RectTransform m_Trans;

    /// <summary>
    /// ˵������ͼ
    /// </summary>
    [SerializeField]
    private Image imgTalkBG;

    /// <summary>
    /// ˵�����ı�
    /// </summary>
    [SerializeField]
    private Text lblTalkText;

    /// <summary>
    /// ˵�����Ŷ���
    /// </summary>
    private Tween m_ScaleTween;

    /// <summary>
    /// ˵������ҡ�ζ���
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
    /// ˵������ʱ��
    /// </summary>
    private float m_TalkStopTime = 0;

    /// <summary>
    /// �Ƿ�˵��
    /// </summary>
    private bool m_IsTalk;

    private string m_TalkText;
    /// <summary>
    /// NPC��ʼ˵��
    /// </summary>
    /// <param name="text">����</param>
    /// <param name="time">����ʱ��</param>
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

        //�������ָ���
        //��ȡ��Ļ����
        Vector2 screenPos = Camera.main.WorldToScreenPoint(m_Target.position);

        //���յ�UI��������
        Vector3 pos;

        //����Ļ����ת��ΪUGUI����������
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
