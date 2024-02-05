using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ��ɫͷ��UI��
/// </summary>
public class RoleHeadBarView : MonoBehaviour
{
    /// <summary>
    /// ƮѪ��ʾ����
    /// </summary>
    [SerializeField]
    private UIHUDText hudText;

    /// <summary>
    /// �����˺���ֵ
    /// </summary>
    public Slider HurtHPSlider;

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
    /// Ѫ��
    /// </summary>
    [SerializeField]
    private Slider sliderHP;

    // Start is called before the first frame update
    void Start()
    {
        hudText = GetComponent<UIHUDText>();
        m_Trans = UILoadingCtrl.Instance.CurrentUIScene.CurrCanvas.GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
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
    }
    /// <summary>
    /// �ϵ��˺�����
    /// </summary>
    /// <param name="hurtValue"></param>
    public void Hurt(int hurtValue,float pbHPValue = 0)
    {
        hudText.Add(hurtValue,Color.red,0.5f);
        HurtHPSlider.value = pbHPValue;
    }

    /// <summary>
    /// ��ɫUI��Ϣ��ʼ��
    /// </summary>
    /// <param name="target"></param>
    /// <param name="nickName"></param>
    /// <param name="isShowHPBar">�Ƿ���ʾѪ��</param>
    /// <param name="sliderValue">Ѫ����ֵ</param>
    public void Init(Transform target, string nickName, bool isShowHPBar = false,float sliderValue = 1f)
    {
        //TODO�����и���ʾBUG����û�м��ص�Ѫ���޷���ʾ�ڽ�ɫͷ����(���д�����)
        m_Target = target;
        lblNickName.text = nickName;
        sliderHP.gameObject.SetActive(isShowHPBar?true:false);

        Image[] imgArr = sliderHP.GetComponentsInChildren<Image>();
        AssetBundleMgr.Instance.LoadOrDownload<Texture2D>(string.Format("Download/Source/UISource/UICommon/slider_bg.assetbundle"), "slider_bg",
    (Texture2D pic) =>
    {
        var iconRect = new Rect(0, 0, pic.width, pic.height);
        imgArr[0].sprite = Sprite.Create(pic, iconRect,new Vector2(.5f, .5f));
    }, type: 0);
        AssetBundleMgr.Instance.LoadOrDownload<Texture2D>(string.Format("Download/Source/UISource/UICommon/slider_hp.assetbundle"), "slider_hp",
(Texture2D pic) =>
{
var iconRect = new Rect(0, 0, pic.width, pic.height);
imgArr[1].sprite = Sprite.Create(pic, iconRect, new Vector2(.5f, .5f));
}, type: 0);
        sliderHP.value = sliderValue;
    }

    /// <summary>
    /// ����Ѫ���ٷֱ�
    /// </summary>
    /// <param name="sliderValue"></param>
    public void SetSliderHP(float sliderValue)
    {
        sliderHP.value = sliderValue;
    }
}
