using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// UI窗口功能类
/// </summary>
public class UIViewUtil : SingletonMiddle<UIViewUtil>
{
    ///窗口UI字典<窗口类型，窗口类>
    private Dictionary<string, UIWindowViewBase> m_DicWindow = new Dictionary<string, UIWindowViewBase>();
    //层级
    private int m_UIViewLayer = 1;

    /// <summary>
    /// 已经打开的窗口数量
    /// </summary>
    public int OpenWindowCount
    {
        get
        {
            return m_DicWindow.Count;
        }
    }

    /// <summary>
    /// 关闭所有窗口
    /// </summary>
    public void CloseAll()
    {
        if (m_DicWindow != null)
        {
            m_DicWindow.Clear();
        }
    }

    public UIRoleCtrl RootCtrl;
    /// <summary>
    /// 加载UI根物体
    /// </summary>
    /// <param name="OnCreate"></param>
    public void LoadUIRoot(Action OnCreate = null)
    {
        GameObject rootUI = ResourceMgr.Instance.Load(ResourcesType.UIScene,"UIRootView",cache:true);
        RootCtrl = rootUI.GetComponent<UIRoleCtrl>();

        ////为根节点添加Lua方法
        //if (rootUI.GetComponent<LuaBehaviour>() == null)
        //{
        //    rootUI.AddComponent<LuaBehaviour>();
        //}
        if (OnCreate != null)
        {
            OnCreate();
        }
    }

    #region OpenWindow 打开窗口

    public void LoadWindowForLua(string viewName,XLuaCustomExport.OnCreate OnCreate=null,string path = null)
    {
        LoadWindow(viewName,null,null,OnCreate,path);
    }

    /// <summary>
    /// 打开窗口
    /// Pan_系列窗口    
    /// </summary>
    /// <param name="type">窗口类型</param>
    /// <returns></returns>
    public void LoadWindow(string viewName,Action<GameObject> onComplete, Action OnShow = null,XLuaCustomExport.OnCreate OnCreate=null,string path = null)
    {
        //如果窗口不存在 则
        if (!m_DicWindow.ContainsKey(viewName) || m_DicWindow[viewName] == null)
        {
            string newPath = string.Empty;
            if (string.IsNullOrEmpty(path))
            {
                newPath = string.Format("Download/Prefab/UIPrefab/UIWindow/Pan_{0}.assetbundle", viewName);
            }
            else
            {
                newPath = path;
            }
            AssetBundleMgr.Instance.LoadOrDownload(newPath, string.Format("Pan_{0}", viewName),
                (GameObject obj) =>
                {
                    obj = UnityEngine.Object.Instantiate(obj);
                    //获取游戏物体上的视图组件
                    UIWindowViewBase windowBase = obj.GetComponent<UIWindowViewBase>();
                    if (windowBase == null)
                    {
                        Debug.Log("传入为空：WindowUIMgr_OpenWindowUI");
                        return;
                    }
                    if (OnShow != null)
                    {
                        windowBase.Onshow = OnShow;
                    }
                    //将预制体放入字典供下次下载
                    m_DicWindow[viewName]=windowBase;
                    
                    //设定为当前的UI类型
                    windowBase.ViewName = viewName;

                    Transform transParent = null;

                    switch (windowBase.containerType)
                    {
                        case WindowUIContainerType.Center:
                            transParent = UILoadingCtrl.Instance.CurrentUIScene.Container_Center;
                            break;
                    }

                    if (transParent != null)
                    {
                        obj.transform.SetParent(transParent);
                    }
                    obj.transform.localPosition = Vector3.zero;
                    obj.transform.localScale = Vector3.one;
                    obj.GetComponent<RectTransform>().sizeDelta = Vector2.zero;
                    obj.gameObject.SetActive(false);
                    SetLayer(obj);
                    StartShowWindow(windowBase, true);
                    if (onComplete != null)
                    {
                        onComplete(obj);
                    }
                    if(OnCreate != null)
                    {
                        OnCreate(obj);
                    }
                });
        }
        else
        {
            if (onComplete != null)
            {
                GameObject obj = m_DicWindow[viewName].gameObject;
                SetLayer(obj);
                onComplete(obj);
            }
        }
    }
    #endregion

    #region CloseWindow 关闭窗口
    /// <summary>
    /// 关闭窗口
    /// </summary>
    /// <param name="type"></param>
    public void CloseWindow(string viewName)
    {
        if (m_DicWindow.ContainsKey(viewName))
        {
            StartShowWindow(m_DicWindow[viewName], false);
        }
    }
    #endregion

    #region StartShowWindow 开始打开窗口
    /// <summary>
    /// 开始打开窗口
    /// </summary>
    /// <param name="windowBase"></param>
    /// <param name="isOpen">是否打开</param>
    private void StartShowWindow(UIWindowViewBase windowBase, bool isOpen)
    {
        switch (windowBase.showStyle)
        {
            case WindowShowStyle.Normal:
                ShowNormal(windowBase, isOpen);
                break;
            case WindowShowStyle.CenterToBig:
                ShowCenterToBig(windowBase, isOpen);
                break;
            case WindowShowStyle.FromTop:
                ShowFromDir(windowBase, 0, isOpen);
                break;
            case WindowShowStyle.FromDown:
                ShowFromDir(windowBase, 1, isOpen);
                break;
            case WindowShowStyle.FromLeft:
                ShowFromDir(windowBase, 2, isOpen);
                break;
            case WindowShowStyle.FromRight:
                ShowFromDir(windowBase, 3, isOpen);
                break;
        }
    }
    #endregion

    #region 各种打开效果

    /// <summary>
    /// 正常打开
    /// </summary>
    /// <param name="windowBase"></param>
    /// <param name="isOpen"></param>
    private void ShowNormal(UIWindowViewBase windowBase, bool isOpen)
    {
        if (isOpen)
        {
            windowBase.gameObject.SetActive(true);
        }
        else
        {
            DestroyWindow(windowBase);
        }
    }

    /// <summary>
    /// 中间变大
    /// </summary>
    /// <param name="windowBase"></param>
    /// <param name="isOpen"></param>
    private void ShowCenterToBig(UIWindowViewBase windowBase, bool isOpen)
    {
        windowBase.gameObject.SetActive(true);
        windowBase.transform.localScale = Vector3.zero;
        //DOTWEEN一个外来组件:OnRewind:当反向播放调用
        Tweener ts = windowBase.transform.DOScale(Vector3.one, windowBase.delayTime).
            SetAutoKill(false).
            SetEase(GlobalInit.Instance.UIAnimationCurve).
            OnRewind(() =>
            {
                DestroyWindow(windowBase);
            });
        if (isOpen)
        {
            windowBase.transform.DOPlayForward();
        }
        else
        {
            windowBase.transform.DOPlayBackwards();
        }
    }

    /// <summary>
    /// 从不同的方向加载
    /// </summary>
    /// <param name="windowBase"></param>
    /// <param name="dirType">0=从上 1=从下 2=从左 3=从右</param>
    /// <param name="isOpen"></param>
    private void ShowFromDir(UIWindowViewBase windowBase, int dirType, bool isOpen)
    {
        windowBase.gameObject.SetActive(true);
        Vector3 from = Vector3.zero;
        switch (dirType)
        {
            case 0:
                from = new Vector3(0, 1000, 0);
                break;
            case 1:
                from = new Vector3(0, -1000, 0);
                break;
            case 2:
                from = new Vector3(-1400, 0, 0);
                break;
            case 3:
                from = new Vector3(1400, 0, 0);
                break;
        }
        windowBase.transform.localPosition = from;
        windowBase.transform.DOLocalMove(Vector3.zero, windowBase.delayTime).
            SetAutoKill(false).
            SetEase(GlobalInit.Instance.UIAnimationCurve).
            OnRewind(() =>
            {
                DestroyWindow(windowBase);
            });
        if (isOpen)
        {
            windowBase.transform.DOPlayForward();
        }
        else
        {
            windowBase.transform.DOPlayBackwards();
        }

    }

    #endregion

    #region DestroyWindow 销毁窗口
    /// <summary>
    /// 销毁窗口
    /// </summary>
    /// <param name="obj"></param>
    private void DestroyWindow(UIWindowViewBase windowBase)
    {
        m_DicWindow.Remove(windowBase.ViewName);
        UnityEngine.Object.Destroy(windowBase.gameObject);
    }
    #endregion

    /// <summary>
    /// 物品Canvas层级上升1
    /// </summary>
    /// <param name="obj"></param>
    public void SetLayer(GameObject obj)
    {
        m_UIViewLayer++;
        Canvas m_Canvas = obj.GetComponent<Canvas>();
        m_Canvas.overrideSorting = true;
        m_Canvas.sortingOrder = m_UIViewLayer;
    }

    /// <summary>
    /// 重置
    /// </summary>
    public void Reset()
    {
        m_UIViewLayer = 1;
    }

    /// <summary>
    /// 检查窗口数量 如果没有打开的窗口 重置
    /// </summary>
    public void CheckOpenWindow()
    {
        m_UIViewLayer--;
        if (UIViewUtil.Instance.OpenWindowCount == 0)
        {
            Reset();
        }
    }
}


