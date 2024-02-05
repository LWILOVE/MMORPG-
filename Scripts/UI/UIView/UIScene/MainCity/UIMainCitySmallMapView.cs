using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ����С��ͼ
/// </summary>
public class UIMainCitySmallMapView : UISubViewBase
{
    public static UIMainCitySmallMapView Instance;

    /// <summary>
    /// С��ͼ
    /// </summary>
    public Image SmallMap;

    /// <summary>
    /// С��ͷ
    /// </summary>
    public Image SmallMapPointer;
    protected override void OnAwake()
    {
        base.OnAwake();
        Instance = this;
    }

    protected override void OnStart()
    {
        base.OnStart();
        WorldMapEntity entity = WorldMapDBModel.Instance.Get((UILoadingCtrl.Instance.CurrentWorldMapId-3));
        //�����ӵ�ͼ
        if (entity != null)
        {
            string picName = entity.SmallMapImg;
            AssetBundleMgr.Instance.LoadOrDownload<Texture2D>(string.Format("Download/Source/UISource/SmallMap/{0}.assetbundle", picName), picName,
                (Texture2D obj) =>
                {
                    if (obj == null)
                    { return; }
                    var iconRect = new Rect(0, 0, obj.width, obj.height);
                    var iconSprite = Sprite.Create(obj, iconRect, new Vector2(0.5f, 0.5f));
                    SmallMap.overrideSprite = iconSprite;
                    Invoke("SetSmallMapPointer",0.05f);
                }, type: 1);

        }

    }

    private void SetSmallMapPointer()
    {
        SmallMapPointer.overrideSprite = UIMainCityRoleInfoView.Instance.imgHead.overrideSprite;
    }
    protected override void BeforeDestroy()
    {
        base.BeforeDestroy();
        SmallMap = null;
        SmallMapPointer = null;
    }
}
