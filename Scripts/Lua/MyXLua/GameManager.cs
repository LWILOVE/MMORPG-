using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XLuaFramework
{
    /// <summary>
    /// xLua框架的主入口
    /// </summary>
    public class GameManager : MonoBehaviour
    {

        private void Awake()
        {
            //启动时，将luaManager挂载到身上
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
