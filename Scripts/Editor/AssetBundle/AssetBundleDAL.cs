using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;

/// <summary>
/// ���·�������
/// </summary>
public class AssetBundleDAL
{
    /// <summary>
    /// xml·��
    /// </summary>
    private string m_Path;
    /// <summary>
    /// �������ʵ�弯��
    /// ʹ��:m_List.PathList[i]Ϊ��ʵ��ĵ�i�������·��
    /// </summary>
    private List<AssetBundleEntity> m_List = null;
    /// <summary>
    /// ·�����乹�캯��
    /// ���ܣ�
    /// ·����ʼ��+�����ʵ��
    /// </summary>
    /// <param name="path">Ŀ��XML�ļ�����·��</param>
    public AssetBundleDAL(string path)
    {
        m_Path = path;
        m_List = new List<AssetBundleEntity>();
    }
    /// <summary>
    /// ����XML����
    /// </summary>
    /// <returns>XML�м�¼�ĸ���Ԫ������</returns>
    public List<AssetBundleEntity> GetList()
    {
        m_List.Clear();

        //xml��ȡ :�����ص�������ӵ������б�(m_List)��
        //XDocument�ࣺ������System.Xml.Linq����ѧϰ�����C#֪ʶ
        //xml�ļ����ݶ�ȡ
        XDocument xDoc = XDocument.Load(m_Path);
        //��ȡxml�ĸ��ڵ�
        XElement root = xDoc.Root;
        //��ȡ���ڵ��µ��ӽ��
        XElement assetBundleNode = root.Element("AssetBundle");
        //��ȡָ�����������ӽڵ�
        IEnumerable<XElement> lst = assetBundleNode.Elements("Item");
        //���������ӽڵ������Ԫ��
        int index = 0;
        foreach (XElement item in lst)
        {
            AssetBundleEntity entity = new AssetBundleEntity();
            entity.Key = "key" + ++index;//������ţ���ʽ:key���
            entity.Name = item.Attribute("Name").Value;//�����ϵ����
            entity.Tag = item.Attribute("Tag").Value;//������ǩ
            entity.IsFolder = item.Attribute("IsFolder").Value.Equals("True",System.StringComparison.CurrentCultureIgnoreCase);//������Ƿ����ļ���
            entity.IsFirstData = item.Attribute("IsFirstData").Value.Equals("True",System.StringComparison.CurrentCultureIgnoreCase);//������Ƿ��ǳ�ʼ����
            //�����д�����·����¼����
            IEnumerable<XElement> pathList = item.Elements("Path");
            foreach (XElement path in pathList)
            {
                entity.PathList.Add(path.Attribute("Value").Value);
            }
            m_List.Add(entity);
        }

        return m_List;
    }
}
