using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class InitBackGroundCtrl : MonoBehaviour
{
    /// <summary>
    /// ��Ϸ����
    /// </summary>
    public GameObject GBT;

    /// <summary>
    /// Ļ��������UGUI�ĸ���
    /// </summary>
    public Canvas Canvas;

    /// <summary>
    /// Ҫ���صı���ͼ
    /// </summary>
    public SceneUIType SceneUIType;

    public GameObject obj;

    // Start is called before the first frame update
    void Start()
    {
        //��ɫ��ʼ��
        if (SceneUIType == SceneUIType.MainCity)
        {
            ////��Ҽ���
            //obj = RoleMgr.Instance.LoadRole("Player");
            ////��ҳ���λ��
            //obj.transform.position = this.transform.position;
            ////��ȡ��ҿ������������ҳ����趨
            //GlobalInit.Instance.currentPlayer = obj.GetComponent<RoleCtrl>();
            //RoleInfoBase roleInfoBase = new RoleInfoBase() { NickName = GlobalInit.Instance.CurrRoleNickName, CurrHP = 1000, MaxHP = 1000 };
            //GlobalInit.Instance.currentPlayer.Init(RoleType.MainPlayer, roleInfoBase, new RoleMainPlayerCityAI(obj.GetComponent<RoleCtrl>()));
            //GlobalInit.Instance.currentPlayer.InitShow(GlobalInit.Instance.CurrRoleNickName);
        }
        else
        {            
            //��¼ORע�᳡�����ؼ����ʼ������
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
