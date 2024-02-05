using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ������������
/// </summary>
public class UISceneInitCtrl : MonoBehaviour
{
    /// <summary>
    /// ��������ʾ��
    /// </summary>
    [SerializeField]
    private Text txt_Load;
    /// <summary>
    /// ������������
    /// </summary>
    [SerializeField]
    private Slider slider_Load;

    public static UISceneInitCtrl Instance;

    private void Awake()
    {
        Instance = this;
    }
    
    /// <summary>
    /// ���ý�����
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
