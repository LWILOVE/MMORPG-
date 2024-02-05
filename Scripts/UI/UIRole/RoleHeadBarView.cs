using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 角色头顶UI条
/// </summary>
public class RoleHeadBarView : MonoBehaviour
{
    /// <summary>
    /// 飘血显示文字
    /// </summary>
    [SerializeField]
    private UIHUDText hudText;

    /// <summary>
    /// 受伤伤害数值
    /// </summary>
    public Slider HurtHPSlider;

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
    /// 血条
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
    }
    /// <summary>
    /// 上弹伤害文字
    /// </summary>
    /// <param name="hurtValue"></param>
    public void Hurt(int hurtValue,float pbHPValue = 0)
    {
        hudText.Add(hurtValue,Color.red,0.5f);
        HurtHPSlider.value = pbHPValue;
    }

    /// <summary>
    /// 角色UI信息初始化
    /// </summary>
    /// <param name="target"></param>
    /// <param name="nickName"></param>
    /// <param name="isShowHPBar">是否显示血条</param>
    /// <param name="sliderValue">血条的值</param>
    public void Init(Transform target, string nickName, bool isShowHPBar = false,float sliderValue = 1f)
    {
        //TODO：这有个显示BUG，即没有加载的血条无法显示在角色头顶上(即有待配置)
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
    /// 设置血条百分比
    /// </summary>
    /// <param name="sliderValue"></param>
    public void SetSliderHP(float sliderValue)
    {
        sliderHP.value = sliderValue;
    }
}
