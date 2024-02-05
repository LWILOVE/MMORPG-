using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSceneCtrlMgr : MonoBehaviour
{

    /// <summary>
    /// 游戏关卡场景控制器
    /// </summary>
    [SerializeField]
    private GameLevelSceneCtrl gameLevelSceneCtrl;

    /// <summary>
    /// 世界地图场景控制器
    /// </summary>
    [SerializeField]
    private WorldMapSceneCtrl worldMapSceneCtrl;

    private Dictionary<SceneType, GameObject> m_Dic = new Dictionary<SceneType, GameObject>();

    [SerializeField]
    private Transform Ground;
    private void Awake()
    {
        if (gameLevelSceneCtrl != null)
        {
            m_Dic[SceneType.ShanGu] = gameLevelSceneCtrl.gameObject;
        }
        if (worldMapSceneCtrl != null)
        {
            m_Dic[SceneType.MainCity] = worldMapSceneCtrl.gameObject;
        }

        //开启有用的控制器，销毁无用的控制器

        GameObject obj = m_Dic[UILoadingCtrl.Instance.CurrentSceneType];
        if (obj != null)
        {
            obj.SetActive(true);
        }

        foreach (var item in m_Dic)
        {
            if (item.Key != UILoadingCtrl.Instance.CurrentSceneType)
            {
                Destroy(item.Value);
            }
        }

        //禁用地面Render
        Renderer[] groundRenders = Ground.GetComponentsInChildren<Renderer>();
        if (groundRenders != null && groundRenders.Length > 0)
        {
            for (int i = 0; i < groundRenders.Length; i++)
            {
                groundRenders[i].enabled = false;
            }
        }
    }
}
