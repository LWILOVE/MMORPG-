using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIWorldMapView : UIWindowViewBase
{
    /// <summary>
    /// 容器
    /// </summary>
    [SerializeField]
    private Transform Container;

    protected override void BeforeOnDestroy()
    {
        base.BeforeOnDestroy();
        Container = null;
    }
    
    /// <summary>
    /// 设置地图子项UI
    /// </summary>
    /// <param name="data"></param>
    /// <param name="onWorldMapItemClick"></param>
    public void SetUI(TransferData data, Action<int> onWorldMapItemClick)
    {
        if (Container == null)
        { return; }
        List<TransferData> list = data.GetValue<List<TransferData>>(ConstDefine.WorldMapList);
        for (int i = 0; i < list.Count; i++)
        {
            //加载世界地图子项
            AssetBundleMgr.Instance.LoadOrDownload<GameObject>(string.Format("Download/Prefab/UIPrefab/UIWindow/GameLevel/GameLevelMapItem/WorldMapItem.assetbundle"), "WorldMapItem",
                (GameObject obj) =>
                {
                    obj = GameObject.Instantiate(obj);
                    obj.SetParent(Container);
                    Vector2 pos = list[i].GetValue<Vector2>(ConstDefine.WorldMapPosition);
                    obj.transform.localPosition = new Vector3(pos.x, pos.y, 0);
                    UIWorldMapItemView view = obj.GetComponent<UIWorldMapItemView>();
                    view.SetUI(list[i], onWorldMapItemClick);
                }, type: 0);

        }
    }
}
