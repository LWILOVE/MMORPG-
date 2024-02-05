using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIHUDText : MonoBehaviour
{
    public Text text;
    [HideInInspector]
    public Text text_Use;
    /// <summary>
    /// hud文字生成器
    /// </summary>
    /// <param name="HUDValue">待生成文字</param>
    /// <param name="color">文字颜色</param>
    /// <param name="stayDuration">文字持续时间</param>
    public void Add(object HUDValue,Color color,float stayDuration)
    {
        if (HUDValue == null || color == null || stayDuration <= 0)
        {
            return;
        }
        text_Use = Instantiate(text,this.gameObject.transform);
        text_Use.text = HUDValue.ToString();
        text_Use.color = color;
        UITextForHUD uITextForHUD = text_Use.GetComponent<UITextForHUD>();
        uITextForHUD.duration = stayDuration;
    }
}
