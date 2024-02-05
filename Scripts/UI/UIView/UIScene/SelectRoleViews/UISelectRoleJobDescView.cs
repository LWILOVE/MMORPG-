using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

/// <summary>
/// ְҵ����
/// </summary>
public class UISelectRoleJobDescView : MonoBehaviour
{
    /// <summary>
    /// ְҵ��
    /// </summary>
    public Text lblJobName;
    /// <summary>
    /// ְҵ����
    /// </summary>
    public Text lblJobDescription;

    private Vector3 m_MoveTargetPos;

    /// <summary>
    /// �Ƿ��һ����ʾ
    /// </summary>
    private bool m_IsShow = false;

    private void Start()
    {
        //m_MoveTargetPos = transform.localPosition;
        //Vector3 from = m_MoveTargetPos + new Vector3(0, 500, 0);
        //transform.localPosition = from;
        //transform.DOLocalMove(m_MoveTargetPos, 1f).SetAutoKill(false).SetEase(GlobalInit.Instance.UIAnimationCurve).Pause()
        //    .OnComplete(() =>
        //    {
        //        m_IsShow = true;
        //    })
        //    .OnRewind(() =>
        //    {
        //        transform.DOPlayForward();
        //    });
        //DoAnim();
    }

    /// <summary>
    /// ����ְҵ����
    /// </summary>
    /// <param name="jobName"></param>
    /// <param name="jobDesc"></param>
    public void SetUI(string jobName, string jobDesc)
    {
        lblJobName.text = jobName;
        lblJobDescription.text = jobDesc;
        DoAnim();
    }

    private void DoAnim()
    {
        if (!m_IsShow)
        {
            transform.DOPlayForward();
        }
        else
        {
            transform.DOPlayBackwards();
        }
    }

    private void OnDestroy()
    {
        lblJobDescription = null;
        lblJobName = null;
    }
}
