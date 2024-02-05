using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UISceneViewBase : UIViewBase
{
    /// <summary>
    /// ������������
    /// </summary>
    [SerializeField]
    public Transform Container_Center;

    /// <summary>
    /// �������ί��
    /// </summary>
    public Action OnLoadComplete;

    /// <summary>
    /// ��ǰĻ��(����������) 
    /// </summary>
    public Canvas CurrCanvas;

    /// <summary>
    /// HUD
    /// </summary>
    public bl_HUDText HUDText;
}
