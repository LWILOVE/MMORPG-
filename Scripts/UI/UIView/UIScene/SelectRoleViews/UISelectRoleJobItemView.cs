using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

/// <summary>
/// ְҵ��
/// </summary>
public class UISelectRoleJobItemView : MonoBehaviour
{
    #region ����
    /// <summary>
    /// ְҵ���
    /// </summary>
    public int m_JobId;

    /// <summary>
    /// ��ת��Ŀ��Ƕ�
    /// </summary>
    public int m_RotateAngle;

    /// <summary>
    /// ְҵѡ��ί��
    /// </summary>
    /// <param name="jobId"></param>
    /// <param name="rotateAngle"></param>
    public delegate void OnSelectJobHandler(int jobId, int rotateAngle);

    /// <summary>
    /// ְҵѡ��ί��
    /// </summary>
    public OnSelectJobHandler OnSelectJob;

    /// <summary>
    /// ��ǰѡ�е�ְҵ��ID
    /// </summary>
    private int m_SelectJobId;

    /// <summary>
    /// �ƶ�Ŀ���
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
            //ͻ����ʾ
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
