using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// UI���ڹ�����
/// </summary>
public class UIViewUtil : SingletonMiddle<UIViewUtil>
{
    ///����UI�ֵ�<�������ͣ�������>
    private Dictionary<string, UIWindowViewBase> m_DicWindow = new Dictionary<string, UIWindowViewBase>();
    //�㼶
    private int m_UIViewLayer = 1;

    /// <summary>
    /// �Ѿ��򿪵Ĵ�������
    /// </summary>
    public int OpenWindowCount
    {
        get
        {
            return m_DicWindow.Count;
        }
    }

    /// <summary>
    /// �ر����д���
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
    /// ����UI������
    /// </summary>
    /// <param name="OnCreate"></param>
    public void LoadUIRoot(Action OnCreate = null)
    {
        GameObject rootUI = ResourceMgr.Instance.Load(ResourcesType.UIScene,"UIRootView",cache:true);
        RootCtrl = rootUI.GetComponent<UIRoleCtrl>();

        ////Ϊ���ڵ����Lua����
        //if (rootUI.GetComponent<LuaBehaviour>() == null)
        //{
        //    rootUI.AddComponent<LuaBehaviour>();
        //}
        if (OnCreate != null)
        {
            OnCreate();
        }
    }

    #region OpenWindow �򿪴���

    public void LoadWindowForLua(string viewName,XLuaCustomExport.OnCreate OnCreate=null,string path = null)
    {
        LoadWindow(viewName,null,null,OnCreate,path);
    }

    /// <summary>
    /// �򿪴���
    /// Pan_ϵ�д���    
    /// </summary>
    /// <param name="type">��������</param>
    /// <returns></returns>
    public void LoadWindow(string viewName,Action<GameObject> onComplete, Action OnShow = null,XLuaCustomExport.OnCreate OnCreate=null,string path = null)
    {
        //������ڲ����� ��
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
                    //��ȡ��Ϸ�����ϵ���ͼ���
                    UIWindowViewBase windowBase = obj.GetComponent<UIWindowViewBase>();
                    if (windowBase == null)
                    {
                        Debug.Log("����Ϊ�գ�WindowUIMgr_OpenWindowUI");
                        return;
                    }
                    if (OnShow != null)
                    {
                        windowBase.Onshow = OnShow;
                    }
                    //��Ԥ��������ֵ乩�´�����
                    m_DicWindow[viewName]=windowBase;
                    
                    //�趨Ϊ��ǰ��UI����
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

    #region CloseWindow �رմ���
    /// <summary>
    /// �رմ���
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

    #region StartShowWindow ��ʼ�򿪴���
    /// <summary>
    /// ��ʼ�򿪴���
    /// </summary>
    /// <param name="windowBase"></param>
    /// <param name="isOpen">�Ƿ��</param>
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

    #region ���ִ�Ч��

    /// <summary>
    /// ������
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
    /// �м���
    /// </summary>
    /// <param name="windowBase"></param>
    /// <param name="isOpen"></param>
    private void ShowCenterToBig(UIWindowViewBase windowBase, bool isOpen)
    {
        windowBase.gameObject.SetActive(true);
        windowBase.transform.localScale = Vector3.zero;
        //DOTWEENһ���������:OnRewind:�����򲥷ŵ���
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
    /// �Ӳ�ͬ�ķ������
    /// </summary>
    /// <param name="windowBase"></param>
    /// <param name="dirType">0=���� 1=���� 2=���� 3=����</param>
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

    #region DestroyWindow ���ٴ���
    /// <summary>
    /// ���ٴ���
    /// </summary>
    /// <param name="obj"></param>
    private void DestroyWindow(UIWindowViewBase windowBase)
    {
        m_DicWindow.Remove(windowBase.ViewName);
        UnityEngine.Object.Destroy(windowBase.gameObject);
    }
    #endregion

    /// <summary>
    /// ��ƷCanvas�㼶����1
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
    /// ����
    /// </summary>
    public void Reset()
    {
        m_UIViewLayer = 1;
    }

    /// <summary>
    /// ��鴰������ ���û�д򿪵Ĵ��� ����
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


