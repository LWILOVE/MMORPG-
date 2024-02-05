using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UISceneLogOnCtrl : UIWindowViewBase
{
    /// <summary>
    /// 注意了，Start的优先级比Awake低才能保证创建无报错，用Awake可能报错
    /// </summary>
    protected override void OnStart()
    {
        base.OnStart();
        StartCoroutine(LogOnCtrl());
    }

    private void Update()
    {
    }

    //登录出现协程
    public IEnumerator LogOnCtrl()
    {
        Debug.Log("发现一坨垃圾");
        yield return new WaitForSeconds(0.75f);
        ////登录窗口
        UIViewUtil.Instance.LoadWindow(WindowUIType.LogOn.ToString(),(GameObject obj) => { });
    }
}   
