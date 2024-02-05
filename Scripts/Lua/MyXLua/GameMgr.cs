using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMgr : MonoBehaviour
{
    private void Awake()
    {
        //启动时，将lua管理器附加到要执行lua组件的游戏物体上
        gameObject.AddComponent<LuaMgr>();
        DontDestroyOnLoad(gameObject);
    }
    
    //Start is called before the first frame update
    void Start()
    {
        //执行第一个lua脚本
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
