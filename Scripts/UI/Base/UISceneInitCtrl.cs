using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 进度条加载类
/// </summary>
public class UISceneInitCtrl : MonoBehaviour
{
    /// <summary>
    /// 进度条提示语
    /// </summary>
    [SerializeField]
    private Text txt_Load;
    /// <summary>
    /// 进度条加载条
    /// </summary>
    [SerializeField]
    private Slider slider_Load;

    public static UISceneInitCtrl Instance;

    private void Awake()
    {
        Instance = this;
    }
    
    /// <summary>
    /// 设置进度条
    /// </summary>
    /// <param name="text"></param>
    /// <param name="value"></param>
    public void SetProgress(string text, float value)
    {
        txt_Load.SetText(text);
        slider_Load.value = value;
    }

    private void OnDestroy()
    {
        txt_Load = null;
        slider_Load = null;
        Instance = null;
    }
}
