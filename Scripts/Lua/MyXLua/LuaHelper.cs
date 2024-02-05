using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XLua;

[LuaCallCSharp]
public class LuaHelper : SingletonMiddle<LuaHelper>
{
    /// <summary>
    /// UIRoot管理器
    /// </summary>
    public UILoadingCtrl m_UISceneCtrl
    {
        get { return UILoadingCtrl.Instance; }
    }

    /// <summary>
    /// 视图管理器
    /// </summary>
    public UIViewUtil UIViewUtil
    {
        get { return UIViewUtil.Instance; }
    }

    /// <summary>
    /// 资源管理器
    /// </summary>
    public ResourceMgr ResourceMgr
    {
        get { return ResourceMgr.Instance; }
    }

    /// <summary>
    /// 打包管理器
    /// </summary>
    public AssetBundleMgr AssetBundleMgr
    {
        get { return AssetBundleMgr.Instance; }
    }

    /// <summary>
    /// 消息窗口控制器
    /// </summary>
    public MessageCtrl MessageCtrl
    {
        get { return MessageCtrl.Instance; }
    }

    /// <summary>
    /// UIRoot管理器
    /// </summary>
    public UILoadingCtrl UISceneCtrl
    {
        get { return UILoadingCtrl.Instance; }
    }

    /// <summary>
    /// 获取表格数据
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public GameDataTabelToLua GetData(string path)
    {
        GameDataTabelToLua data = new GameDataTabelToLua();
#if DISABLE_ASSETBUNDLE
        path = Application.dataPath + "/Download/DownTable" + path;
#else 
        path = Application.persistentDataPath + "Download/DataTable/" + path;
#endif
        using (GameDataTableParser parse = new GameDataTableParser(path))
        {
            data.Row = parse.Row;
            data.Column = parse.Column;

            //实例化交叉数组
            data.Data = new string[data.Row][];

            //将二维数组转化为交叉数组
            for (int i = 0; i < data.Row; i++)
            {
                string[] arr = new string[data.Column];
                for (int j = 0; j < data.Column; j++)
                {
                    arr[j] = parse.GameData[i, j];
                }
                data.Data[i] = arr;
            }
        }
        return data;
    }

    /// <summary>
    /// 创建一个协议发送流
    /// </summary>
    /// <returns></returns>
    public MMO_MemoryStream CreateMemoryStream()
    {
        return new MMO_MemoryStream();
    }

    /// <summary>
    /// 通过数据内容创建一个协议发送流
    /// </summary>
    /// <param name="buffer"></param>
    /// <returns></returns>
    public MMO_MemoryStream CreateMemoryStream(byte[] buffer)
    {
        return new MMO_MemoryStream(buffer);
    }

    /// <summary>
    /// 协议发送方法
    /// </summary>
    /// <param name="buffer"></param>
    public void SendProto(byte[] buffer)
    {
        NetWorkSocket.Instance.SendMessage(buffer);
    }

    /// <summary>
    /// 添加事件监听方法
    /// </summary>
    /// <param name="protoCode">协议编号</param>
    /// <param name="callBack">协议回调</param>
    public void AddEventListener(ushort protoCode, SocketDispatcher.OnActionHandler callBack)
    {
        SocketDispatcher.Instance.AddEventListener(protoCode, callBack);
    }

    /// <summary>
    /// 移除事件监听方法
    /// </summary>
    /// <param name="protoCode">协议编号</param>
    /// <param name="callBack">协议回调</param>
    public void RemoveEventListener(ushort protoCode, SocketDispatcher.OnActionHandler callBack)
    {
        SocketDispatcher.Instance.RemoveEventListener(protoCode, callBack);
    }


    private LuaTable scriptEnv;
    private LuaEnv luaEnv;
    [CSharpCallLua]
    public delegate void delLuaLoadView(string ctrlName);
    LuaHelper.delLuaLoadView luaLoadView;

    /// <summary>
    /// 加载由LUA生成的窗口
    /// </summary>
    /// <param name="ctrlName"></param>
    public void LoadLuaView(string ctrlName)
    {
        luaEnv = LuaMgr.luaEnv;
        if (luaEnv == null)
        { return; }
        scriptEnv = luaEnv.NewTable();
        LuaTable meta = luaEnv.NewTable();
        meta.Set("__index", luaEnv.Global);
        scriptEnv.SetMetaTable(meta);
        meta.Dispose();
        luaLoadView = scriptEnv.GetInPath<LuaHelper.delLuaLoadView>("GameInit.LoadView");
        if (luaLoadView != null)
        {
            luaLoadView(ctrlName);
        }
        scriptEnv = null;
    }

    /// <summary>
    /// 自动加载图片
    /// </summary>
    /// <param name="go"></param>
    /// <param name="imgPath"></param>
    /// <param name="imgName"></param>
    public void AutoLoadTexture(GameObject go, string imgPath, string imgName)
    {
        AutoLoadTexture component = go.GetOrCreatComponent<AutoLoadTexture>();
        if (component != null)
        {
            component.ImgPath = imgPath;
            component.ImgName = imgName;
        }
    }

    /// <summary>
    /// 发送Http数据
    /// </summary>
    /// <param name="url"></param>
    /// <param name="callBack"></param>
    /// <param name="isPost"></param>
    /// <param name="param"></param>
    public void SendHttpData(string url, XLuaCustomExport.NetWorkSendDataCallBack callBack, bool isPost, string[][] param)
    {
        Dictionary<string, object> dic = null;
        if (param != null)
        {
            dic = new Dictionary<string, object>();
            for (int i = 0; i < param.Length; i++)
            {
                if (param[i].Length >= 2)
                {
                    string key = param[i][0];
                    object value = param[i][1];
                    dic[key] = value;
                }
            }
        }
        NetWorkHttp.Instance.SendData(url, callBack, isPost, dic);
    }

    public string GetLanguageText(int id)
    {
        return LanguageDBModel.Instance.GetText(id);
    }


}
