using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMgr : MonoBehaviour
{
    private void Awake()
    {
        //����ʱ����lua���������ӵ�Ҫִ��lua�������Ϸ������
        gameObject.AddComponent<LuaMgr>();
        DontDestroyOnLoad(gameObject);
    }
    
    //Start is called before the first frame update
    void Start()
    {
        //ִ�е�һ��lua�ű�
        LuaMgr.Instance.DoString("require'Download/XLuaLogic/LuaProject/LuaProject/Main'");
    }

    public void cd()
    {
    }
    //Update is called once per frame
    void Update()
    {

    }
}
