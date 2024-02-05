using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;   

/// <summary>
///加载进度条设置类
/// </summary>
public class UISceneLoadingCtrl : UIWindowViewBase
{
    //进度条
    public Slider m_Progress;
    //进度条文本
    public Text m_LblProgress;
    //下一场景
    public Text m_lblNextScene;


    /// <summary>
    /// 设置进度条的值 
    /// </summary>
    /// <param name="value"></param>
    public void SetProgressValue(float value, SceneType nextScene)
    {
        if (m_lblNextScene.text != ("正在前往的是" + nextScene.ToString()))
        {
            m_lblNextScene.text = "正在前往的是"+nextScene.ToString();
        }
        if (m_Progress == null || m_LblProgress == null)
        { return; }
        m_Progress.value = value;
        m_LblProgress.text = string.Format("{0}%", (int)(value * 100));
    }
    /// <summary>
    /// 当不需要时将UI销毁
    /// </summary>
    protected override void BeforeOnDestroy()
    {
        base.BeforeOnDestroy();
        m_Progress = null;
        m_LblProgress = null;
    }
}
