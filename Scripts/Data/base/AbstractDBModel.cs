using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ���ݹ������
/// </summary>
/// <typeparam name="T">����</typeparam>
/// <typeparam name="P">�����Ӧ��ʵ����</typeparam>
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
    #region ����
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

    #region �������ʵ�ֵ����Ի򷽷�
    /// <summary>
    /// �����ļ���
    /// </summary>
    protected abstract string FileName
    { get;}
    /// <summary>
    /// ����ʵ��
    /// </summary>
    /// <param name="parse"></param>
    /// <returns></returns>
    protected abstract P MakeEntity(GameDataTableParser parse);
    #endregion

    #region �������� - LoadData
    /// <summary>
    /// ���ݼ���
    /// </summary>
    private void LoadData()
    {
        //���ݰ����ڵľ���·��
        string path = null;
        path = DownloadMgr.Instance.localFilePath + "Download/DataTable/" + FileName;
        using (GameDataTableParser parse = new GameDataTableParser(path))
        {
            while (!parse.Eof)
            {
                //����ʵ��
                P p = MakeEntity(parse);
                m_List.Add(p);
                m_Dict[p.Id] = p;
                //�鿴��һ��
                parse.Next();
            }
        }
    }
#endregion

#region ��ȡ���� - GetList
    /// <summary>
    /// ��ȡ����
    /// </summary>
    /// <returns></returns>
    public List<P> GetList()
    {
        return m_List;
    }
#endregion
    
#region ���ݱ�Ż�ȡʵ�� - Get
    /// <summary>
    /// ���ݱ�Ż�ȡʵ��
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
