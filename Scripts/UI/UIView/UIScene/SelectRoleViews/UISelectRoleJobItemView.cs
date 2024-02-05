using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

/// <summary>
/// 职业项
/// </summary>
public class UISelectRoleJobItemView : MonoBehaviour
{
    #region 变量
    /// <summary>
    /// 职业编号
    /// </summary>
    public int m_JobId;

    /// <summary>
    /// 旋转的目标角度
    /// </summary>
    public int m_RotateAngle;

    /// <summary>
    /// 职业选择委托
    /// </summary>
    /// <param name="jobId"></param>
    /// <param name="rotateAngle"></param>
    public delegate void OnSelectJobHandler(int jobId, int rotateAngle);

    /// <summary>
    /// 职业选择委托
    /// </summary>
    public OnSelectJobHandler OnSelectJob;

    /// <summary>
    /// 当前选中的职业的ID
    /// </summary>
    private int m_SelectJobId;

    /// <summary>
    /// 移动目标点
    /// </summary>
    private Vector3 m_MoveTargetPos;
    #endregion
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Button>().onClick.AddListener(OnButtonClick);
        m_MoveTargetPos = transform.localPosition + new Vector3(50,0,0);
        transform.DOLocalMove(m_MoveTargetPos,1f).SetAutoKill(false).SetEase(GlobalInit.Instance.UIAnimationCurve).Pause();
        SetSelected(m_SelectJobId);
    }

    public void SetSelected(int selectJobId)
    {
        m_SelectJobId = selectJobId;
        if (m_JobId == selectJobId)
        {
            //突出显示
            transform.DOPlayForward();
        }
        else
        {
            transform.DOPlayBackwards();
        }
    }

    private void OnButtonClick()
    {
        if(OnSelectJob != null)
        {
            OnSelectJob(m_JobId, m_RotateAngle);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDestroy()
    {
        OnSelectJob = null;

    }
}
