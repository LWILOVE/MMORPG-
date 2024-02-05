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
        //��ʱע��
        //AudioBackGroundMgr.Instance.Play("Audio_Bg_LogOn");
        //��ʱע��
    }
    // Start is called before the first frame update
    void Start()
    {
        //���ټ��س���
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
