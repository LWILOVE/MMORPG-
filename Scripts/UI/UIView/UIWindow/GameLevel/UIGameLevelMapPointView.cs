using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// �ؿ���ͼ���ߵ�UI
/// </summary>
public class UIGameLevelMapPointView : UISubViewBase
{
    /// <summary>
    /// ͨ��ͼ
    /// </summary>
    [SerializeField]
    private Image imgPass;

    /// <summary>
    /// δͨ��ͼ
    /// </summary>
    [SerializeField]
    private Image imgUnPass;

    public void SetUI(bool isPass)
    {
        //��ͨ�أ�����ʾͨ��ͼ��������ʾδͨ��
        if (isPass)
        {
            imgPass.gameObject.SetActive(true);
        }
        else
        {
            imgUnPass.gameObject.SetActive(true);
        }
    }
}