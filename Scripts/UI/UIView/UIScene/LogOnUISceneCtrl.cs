    using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogOnUISceneCtrl : MonoBehaviour
{
    private GameObject obj;

    private void Awake() 
    {
        
        UILoadingCtrl.Instance.LoadSceneUI(SceneUIType.LogOn,
            (GameObject obj)=>
            {
            });
        //临时注释
        //AudioBackGroundMgr.Instance.Play("Audio_Bg_LogOn");
        //临时注释
    }
    // Start is called before the first frame update
    void Start()
    {
        //销毁加载场景
        if (DelegateDefine.Instance.OnSceneLoadOk != null)
        {
            DelegateDefine.Instance.OnSceneLoadOk();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
