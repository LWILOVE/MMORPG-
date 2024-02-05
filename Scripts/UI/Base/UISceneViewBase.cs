using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UISceneViewBase : UIViewBase
{
    /// <summary>
    /// 容器——居中
    /// </summary>
    [SerializeField]
    public Transform Container_Center;

    /// <summary>
    /// 加载完毕委托
    /// </summary>
    public Action OnLoadComplete;

    /// <summary>
    /// 当前幕布(场景主画布) 
    /// </summary>
    public Canvas CurrCanvas;

    /// <summary>
    /// HUD
    /// </summary>
    public bl_HUDText HUDText;
}
