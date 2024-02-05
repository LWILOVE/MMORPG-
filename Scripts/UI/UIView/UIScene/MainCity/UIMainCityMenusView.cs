using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;
using UnityEngine.UI;

public class UIMainCityMenusView : UISubViewBase
{
    public static UIMainCityMenusView Instance;

    /// <summary>
    /// �ƶ���Ŀ���
    /// </summary>
    private Vector3 m_MoveTargetPos;

    /// <summary>
    /// �Ƿ���ʾ
    /// </summary>
    private bool m_IsShow = false;
    /// <summary>
    /// �Ƿ��������л���æ��
    /// </summary>
    private bool m_IsBusy = false;

    /// <summary>
    /// ���˵�״̬ת���ɹ�ʱ����
    /// </summary>
    private Action m_OnChangeSuccess;

    protected override void OnAwake()
    {
        base.OnAwake();
        Instance = this;

        //ͨ�����������ý��й��ܲ˵��Ŀ���
        Button[] btnArr = transform.GetComponentsInChildren<Button>();
        for (int i = 0; i < btnArr.Length; i++)
        {
            if (btnArr[i].gameObject.name.Equals("Btn_Menu_GameLevel", StringComparison.CurrentCultureIgnoreCase) &&
                UIGameServerConfigMgr.Instance.Get(ConfigCode.GameLevelMenu).IsOpen == false)
            {
                btnArr[i].enabled = false;
            }
        }
    }

    protected override void OnStart()
    {
        base.OnStart();
        if (GlobalInit.Instance == null)
        { return; }
        m_IsShow = true;

        m_MoveTargetPos = transform.localPosition + new Vector3(0, 70, 0);

        transform.DOLocalMove(m_MoveTargetPos, 0.2f).SetAutoKill(false).SetEase(GlobalInit.Instance.UIAnimationCurve).Pause().OnComplete(() =>
        {
            if (m_OnChangeSuccess != null)
            {
                m_OnChangeSuccess();
            }
            m_IsBusy = false;
        }).OnRewind(() =>
        {
            if (m_OnChangeSuccess != null)
            {
                m_OnChangeSuccess();
            }
            m_IsBusy = false;
        });
    }

    public void ChangeState(Action OnChangeSuccess)
    {
        if (m_IsBusy)
        {
            return;
        }
        m_IsBusy = true;
        m_OnChangeSuccess = OnChangeSuccess;
        if (m_IsShow)
        {
            transform.DOPlayForward();
        }
        else
        {
            transform.DOPlayBackwards();
        }
        m_IsShow = !m_IsShow;
    }
}
