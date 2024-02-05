using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIWorldMapFailView : UIWindowViewBase
{
    /// <summary>
    /// ����ί��
    /// </summary>
    public System.Action OnResurgence;
    ///// <summary>
    ///// ��ʾ�ı�
    ///// </summary>
    //[SerializeField]
    //private Text lblTip;

    protected override void OnBtnClick(GameObject go)
    {
        base.OnBtnClick(go);

        switch (go.name)
        {
            case "Btn_Return":
                //�������ǰ�ť
                PlayerCtrl.Instance.LastInWorldMapPos = string.Empty;//������������������
                GlobalInit.Instance.currentPlayer.ToResurgence(RoleIdleState.IdleNormal);
                UILoadingCtrl.Instance.LoadToWorldMap(4); 
                break;
            case "Btn_Resurgence":
                //��ɫ���ť
                if (OnResurgence != null)
                { OnResurgence(); }
                break;
        }
    }
    //public void SetUI(string enemyNickName)
    //{
    //    lblTip.SetText(string.Format("�㱻{0}�����ˣ�Ψ��봽���������һ��֮�ϣ�", enemyNickName));
    //}
}
