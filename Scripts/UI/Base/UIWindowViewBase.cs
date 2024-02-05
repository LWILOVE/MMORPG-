using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
/// <summary>
/// ����UI������ͼ�Ļ���
/// </summary>
public class UIWindowViewBase : UIViewBase
{
    /// <summary>
    /// �ҵ�����
    /// </summary>
    [SerializeField]
    public WindowUIContainerType containerType = WindowUIContainerType.Center;

    /// <summary>
    /// �򿪷�ʽ
    /// </summary>
    [SerializeField]
    public WindowShowStyle showStyle = WindowShowStyle.Normal;

    /// <summary>
    /// �򿪻�رն���Ч������ʱ��
    /// </summary>
    [SerializeField]
    public float delayTime = 0.25f;

    /// <summary>
    /// ��һ��Ҫ�򿪵Ĵ���
    /// </summary>
    private WindowUIType m_NextOpenType;

    /// <summary>
    /// Ҫ���͵Ĵ�����
    /// </summary>
    [HideInInspector]
    public string ViewName;
    /// <summary>
    /// ����رհ�ť
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
    /// �رմ���
    /// </summary>
    public virtual void Close()
    {
        //������رհ�ťʱ�����Źر���ЧTODO
        //AudioEffectMgr.Instance.PlayUIAudioEffect(UIAudioEffectType.UIClose);
        UIViewUtil.Instance.CloseWindow(ViewName);
    }
    /// <summary>
    /// �رմ���ͬʱ����һ����
    /// </summary>
    /// <param name="nextType"></param>
    public virtual void CloseAndOpenNext(WindowUIType nextType)
    {
        this.Close();
        m_NextOpenType = nextType;
        UIViewMgr.Instance.OpenWindow(m_NextOpenType);
    }

    /// <summary>
    /// ����֮ǰִ��
    /// </summary>
    protected override void BeforeOnDestroy()
    {
        UIViewUtil.Instance.CheckOpenWindow();
        if (m_NextOpenType != WindowUIType.None)
        {
            ////����һ��BUG�����ǲ�֪��Ϊʲô���ɵ�ƽ̳�Ҫд��ȥ
            //UIViewMgr.Instance.OpenWindow(m_NextOpenType);
        }
    }
}
