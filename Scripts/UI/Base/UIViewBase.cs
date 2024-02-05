using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// 所有UI视图的基类
/// </summary>
public class UIViewBase : MonoBehaviour
{
    public Action Onshow;

    private void Awake()
    {
        //为子类提供一个唤醒时附带的功能,确保了基类先被执行然后采用子类的唤醒功能启动
        OnAwake();
    }
    void Start()
    {
        Button[] btnArr = GetComponentsInChildren<Button>(true);
        for (int i = 0; i < btnArr.Length; i++)
        {
            //执行各个按钮的功能
            EventTriggerListener.Get(btnArr[i].gameObject).onClick += BtnClick; ;
        }

        OnStart();
        if (Onshow != null)
        {
            Onshow();
        }
    }

    private void BtnClick(GameObject go)
    {
        //按钮点击时播放按钮点击声音TODO
        //AudioEffectMgr.Instance.PlayUIAudioEffect(UIAudioEffectType.ButtonClick);
        OnBtnClick(go);
    }
    
    private void OnDestroy()
    {
        BeforeOnDestroy();
    }
    //虚方法
    protected virtual void OnAwake()
    { }
    protected virtual void OnStart()
    { }
    protected virtual void BeforeOnDestroy()
    { }
    protected virtual void OnBtnClick(GameObject go)
    { }
}
