using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XLua;

/// <summary>
/// Lua����������
/// </summary>
public class LuaMgr : SingletonMono<LuaMgr>
{
    /// <summary>
    /// ȫ�ֵ�xlua����
    /// </summary>
    public static LuaEnv luaEnv;


    private void Awake()
    {
        //����lua����ʵ����
        luaEnv = new LuaEnv();
        //��ʼ��xlua�Ľű�·������Application.dataPath�ļ����£�lua���ļ����ᱻ��ʼ������ 
        //��ʽ��ѧϰ
        luaEnv.DoString(string.Format("package.path = '{0}/?.lua'", Application.dataPath));
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //ʱ�̻���
        //luaEnv.GC();
    }

    private void OnDestroy()
    {
        //�ͷ�
        //luaEnv.Dispose();
    }

    /// <summary>
    /// ִ��lua�ű�
    /// </summary>
    /// <param name="str"></param>
    public void DoString(string str)
    {
        luaEnv.DoString(str);
    }
}
