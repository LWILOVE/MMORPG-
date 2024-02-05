using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 数据管理基类
/// </summary>
/// <typeparam name="T">子类</typeparam>
/// <typeparam name="P">子类对应的实体类</typeparam>
public abstract class AbstractDBModel<T,P>
    where T : class,new()
    where P : AbstractEntity
{
    protected List<P> m_List;
    protected Dictionary<int,P> m_Dict;
    public AbstractDBModel()
    {
        m_Dict = new Dictionary<int,P>();
        m_List = new List<P>();
        LoadData();
    }
    #region 单例
    private static T instance;

    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new T();
            }
            return instance;
        }
    }
    #endregion

    #region 子类必须实现的属性或方法
    /// <summary>
    /// 数据文件名
    /// </summary>
    protected abstract string FileName
    { get;}
    /// <summary>
    /// 创建实体
    /// </summary>
    /// <param name="parse"></param>
    /// <returns></returns>
    protected abstract P MakeEntity(GameDataTableParser parse);
    #endregion

    #region 加载数据 - LoadData
    /// <summary>
    /// 数据加载
    /// </summary>
    private void LoadData()
    {
        //数据包所在的绝对路径
        string path = null;
        path = DownloadMgr.Instance.localFilePath + "Download/DataTable/" + FileName;
        using (GameDataTableParser parse = new GameDataTableParser(path))
        {
            while (!parse.Eof)
            {
                //创建实体
                P p = MakeEntity(parse);
                m_List.Add(p);
                m_Dict[p.Id] = p;
                //查看下一个
                parse.Next();
            }
        }
    }
#endregion

#region 获取集合 - GetList
    /// <summary>
    /// 获取集合
    /// </summary>
    /// <returns></returns>
    public List<P> GetList()
    {
        return m_List;
    }
#endregion
    
#region 根据编号获取实体 - Get
    /// <summary>
    /// 根据编号获取实体
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public P Get(int id)
    {
        if (m_Dict.ContainsKey(id))
        {
            return m_Dict[id];
        }
        return null;
    }
#endregion
}
