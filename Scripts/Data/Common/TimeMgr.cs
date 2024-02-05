using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeMgr : SingletonMono<TimeMgr>
{
    /// <summary>
    /// �Ƿ������������ڼ�
    /// </summary>
    private bool m_IsTimeScale;
    /// <summary>
    /// ʱ�����Ž���ʱ��
    /// </summary>
    private float m_TimeScaleEndTime = 0f;
    protected override void OnUpdate()
    {
        base.OnUpdate();
        if (m_IsTimeScale)
        {
            if (Time.realtimeSinceStartup > m_TimeScaleEndTime)
            {
                Time.timeScale = 1;
                m_IsTimeScale = false;
            }
        }
    }


    /// <summary>
    /// �޸�ʱ������
    /// </summary>
    /// <param name="TimeScaleRate"></param>
    /// <param name="continueTime"></param>
    public void ChangeTimeScale(float TimeScaleRate, float continueTime)
    {
        m_IsTimeScale = true;
        Time.timeScale = TimeScaleRate;
        m_TimeScaleEndTime = Time.realtimeSinceStartup + continueTime;
    }
}
