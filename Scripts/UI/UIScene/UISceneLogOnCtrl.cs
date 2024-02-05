using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UISceneLogOnCtrl : UIWindowViewBase
{
    /// <summary>
    /// ע���ˣ�Start�����ȼ���Awake�Ͳ��ܱ�֤�����ޱ�����Awake���ܱ���
    /// </summary>
    protected override void OnStart()
    {
        base.OnStart();
        StartCoroutine(LogOnCtrl());
    }

    private void Update()
    {
    }

    //��¼����Э��
    public IEnumerator LogOnCtrl()
    {
        Debug.Log("����һ������");
        yield return new WaitForSeconds(0.75f);
        ////��¼����
        UIViewUtil.Instance.LoadWindow(WindowUIType.LogOn.ToString(),(GameObject obj) => { });
    }
}   
