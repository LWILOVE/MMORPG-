using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeMgr : SingletonMono<TimeMgr>
{
    /// <summary>
    /// 是否正处于缩放期间
    /// </summary>
    private bool m_IsTimeScale;
    /// <summary>
    /// 时间缩放结束时间
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
    /// 修改时间缩放
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
