using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// ����UI��ͼ�Ļ���
/// </summary>
public class UIViewBase : MonoBehaviour
{
    public Action Onshow;

    private void Awake()
    {
        //Ϊ�����ṩһ������ʱ�����Ĺ���,ȷ���˻����ȱ�ִ��Ȼ���������Ļ��ѹ�������
        OnAwake();
    }
    void Start()
    {
        Button[] btnArr = GetComponentsInChildren<Button>(true);
        for (int i = 0; i < btnArr.Length; i++)
        {
            //ִ�и�����ť�Ĺ���
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
        //��ť���ʱ���Ű�ť�������TODO
        //AudioEffectMgr.Instance.PlayUIAudioEffect(UIAudioEffectType.ButtonClick);
        OnBtnClick(go);
    }
    
    private void OnDestroy()
    {
        BeforeOnDestroy();
    }
    //�鷽��
    protected virtual void OnAwake()
    { }
    protected virtual void OnStart()
    { }
    protected virtual void BeforeOnDestroy()
    { }
    protected virtual void OnBtnClick(GameObject go)
    { }
}
