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
    /// hud����������
    /// </summary>
    /// <param name="HUDValue">����������</param>
    /// <param name="color">������ɫ</param>
    /// <param name="stayDuration">���ֳ���ʱ��</param>
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
