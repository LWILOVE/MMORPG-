using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XLua;

/// <summary>
/// Lua环境管理器
/// </summary>
public class LuaMgr : SingletonMono<LuaMgr>
{
    /// <summary>
    /// 全局的xlua引擎
    /// </summary>
    public static LuaEnv luaEnv;


    private void Awake()
    {
        //进行lua引擎实例化
        luaEnv = new LuaEnv();
        //初始化xlua的脚本路径，即Application.dataPath文件夹下，lua的文件均会被初始化加载 
        //格式待学习
        luaEnv.DoString(string.Format("package.path = '{0}/?.lua'", Application.dataPath));
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //时刻回收
        //luaEnv.GC();
    }

    private void OnDestroy()
    {
        //释放
        //luaEnv.Dispose();
    }

    /// <summary>
    /// 执行lua脚本
    /// </summary>
    /// <param name="str"></param>
    public void DoString(string str)
    {
        luaEnv.DoString(str);
    }
}
