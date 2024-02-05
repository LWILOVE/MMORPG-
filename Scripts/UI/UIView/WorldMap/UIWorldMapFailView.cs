using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIWorldMapFailView : UIWindowViewBase
{
    /// <summary>
    /// 复活委托
    /// </summary>
    public System.Action OnResurgence;
    ///// <summary>
    ///// 提示文本
    ///// </summary>
    //[SerializeField]
    //private Text lblTip;

    protected override void OnBtnClick(GameObject go)
    {
        base.OnBtnClick(go);

        switch (go.name)
        {
            case "Btn_Return":
                //返回主城按钮
                PlayerCtrl.Instance.LastInWorldMapPos = string.Empty;//清空最后进入的世界坐标
                GlobalInit.Instance.currentPlayer.ToResurgence(RoleIdleState.IdleNormal);
                UILoadingCtrl.Instance.LoadToWorldMap(4); 
                break;
            case "Btn_Resurgence":
                //角色复活按钮
                if (OnResurgence != null)
                { OnResurgence(); }
                break;
        }
    }
    //public void SetUI(string enemyNickName)
    //{
    //    lblTip.SetText(string.Format("你被{0}给噶了，唯有氪金才能凌驾于一切之上！", enemyNickName));
    //}
}
