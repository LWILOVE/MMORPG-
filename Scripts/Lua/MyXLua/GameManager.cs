using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XLuaFramework
{
    /// <summary>
    /// xLua��ܵ������
    /// </summary>
    public class GameManager : MonoBehaviour
    {

        private void Awake()
        {
            //����ʱ����luaManager���ص�����
            //gameObject.AddComponent<LuaManager>();
        }

        // Start is called before the first frame update
        void Start()
        {
            //LuaManager.Instance.DoString("require'XLuaLogic/Main'");
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
