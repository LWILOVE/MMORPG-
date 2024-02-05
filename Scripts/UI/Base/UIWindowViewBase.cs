using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
/// <summary>
/// 所有UI窗口视图的基类
/// </summary>
public class UIWindowViewBase : UIViewBase
{
    /// <summary>
    /// 挂点类型
    /// </summary>
    [SerializeField]
    public WindowUIContainerType containerType = WindowUIContainerType.Center;

    /// <summary>
    /// 打开方式
    /// </summary>
    [SerializeField]
    public WindowShowStyle showStyle = WindowShowStyle.Normal;

    /// <summary>
    /// 打开或关闭动画效果持续时间
    /// </summary>
    [SerializeField]
    public float delayTime = 0.25f;

    /// <summary>
    /// 下一个要打开的窗口
    /// </summary>
    private WindowUIType m_NextOpenType;

    /// <summary>
    /// 要传送的窗口名
    /// </summary>
    [HideInInspector]
    public string ViewName;
    /// <summary>
    /// 点击关闭按钮
    /// </summary>
    /// <param name="go"></param>
    protected override void OnBtnClick(GameObject go)
    {
        base.OnBtnClick(go);
        if (go.name.Equals("Btn_Close", System.StringComparison.CurrentCultureIgnoreCase))
        {
            Close();
        }
    }

    /// <summary>
    /// 关闭窗口
    /// </summary>
    public virtual void Close()
    {
        //当点击关闭按钮时，播放关闭音效TODO
        //AudioEffectMgr.Instance.PlayUIAudioEffect(UIAudioEffectType.UIClose);
        UIViewUtil.Instance.CloseWindow(ViewName);
    }
    /// <summary>
    /// 关闭窗口同时打开下一窗口
    /// </summary>
    /// <param name="nextType"></param>
    public virtual void CloseAndOpenNext(WindowUIType nextType)
    {
        this.Close();
        m_NextOpenType = nextType;
        UIViewMgr.Instance.OpenWindow(m_NextOpenType);
    }

    /// <summary>
    /// 销毁之前执行
    /// </summary>
    protected override void BeforeOnDestroy()
    {
        UIViewUtil.Instance.CheckOpenWindow();
        if (m_NextOpenType != WindowUIType.None)
        {
            ////这是一个BUG，但是不知道为什么这个傻逼教程要写上去
            //UIViewMgr.Instance.OpenWindow(m_NextOpenType);
        }
    }
}
