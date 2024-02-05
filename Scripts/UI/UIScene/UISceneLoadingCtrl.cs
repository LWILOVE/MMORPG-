using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;   

/// <summary>
///���ؽ�����������
/// </summary>
public class UISceneLoadingCtrl : UIWindowViewBase
{
    //������
    public Slider m_Progress;
    //�������ı�
    public Text m_LblProgress;
    //��һ����
    public Text m_lblNextScene;


    /// <summary>
    /// ���ý�������ֵ 
    /// </summary>
    /// <param name="value"></param>
    public void SetProgressValue(float value, SceneType nextScene)
    {
        if (m_lblNextScene.text != ("����ǰ������" + nextScene.ToString()))
        {
            m_lblNextScene.text = "����ǰ������"+nextScene.ToString();
        }
        if (m_Progress == null || m_LblProgress == null)
        { return; }
        m_Progress.value = value;
        m_LblProgress.text = string.Format("{0}%", (int)(value * 100));
    }
    /// <summary>
    /// ������Ҫʱ��UI����
    /// </summary>
    protected override void BeforeOnDestroy()
    {
        base.BeforeOnDestroy();
        m_Progress = null;
        m_LblProgress = null;
    }
}
