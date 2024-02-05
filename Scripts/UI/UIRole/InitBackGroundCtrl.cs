using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class InitBackGroundCtrl : MonoBehaviour
{
    /// <summary>
    /// 游戏物体
    /// </summary>
    public GameObject GBT;

    /// <summary>
    /// 幕布，所有UGUI的父类
    /// </summary>
    public Canvas Canvas;

    /// <summary>
    /// 要加载的背景图
    /// </summary>
    public SceneUIType SceneUIType;

    public GameObject obj;

    // Start is called before the first frame update
    void Start()
    {
        //角色初始化
        if (SceneUIType == SceneUIType.MainCity)
        {
            ////玩家加载
            //obj = RoleMgr.Instance.LoadRole("Player");
            ////玩家出生位置
            //obj.transform.position = this.transform.position;
            ////获取玩家控制器并完成玩家出生设定
            //GlobalInit.Instance.currentPlayer = obj.GetComponent<RoleCtrl>();
            //RoleInfoBase roleInfoBase = new RoleInfoBase() { NickName = GlobalInit.Instance.CurrRoleNickName, CurrHP = 1000, MaxHP = 1000 };
            //GlobalInit.Instance.currentPlayer.Init(RoleType.MainPlayer, roleInfoBase, new RoleMainPlayerCityAI(obj.GetComponent<RoleCtrl>()));
            //GlobalInit.Instance.currentPlayer.InitShow(GlobalInit.Instance.CurrRoleNickName);
        }
        else
        {            
            //登录OR注册场景加载及其初始化处理
            //UILoadingCtrl.Instance.LoadSceneUI(SceneUIType);
            //RectTransform tc = GBT.GetComponent<RectTransform>();
            //tc.SetParent(Canvas.transform);
            //tc.sizeDelta = Vector2.zero;
            //tc.anchoredPosition3D = Vector3.zero;
        }

    }
    private void Update()
    {
    }
}
