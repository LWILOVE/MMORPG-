using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;

/// <summary>
/// 打包路径相关类
/// </summary>
public class AssetBundleDAL
{
    /// <summary>
    /// xml路径
    /// </summary>
    private string m_Path;
    /// <summary>
    /// 打包数据实体集合
    /// 使用:m_List.PathList[i]为该实体的第i个物体的路径
    /// </summary>
    private List<AssetBundleEntity> m_List = null;
    /// <summary>
    /// 路径传输构造函数
    /// 功能：
    /// 路径初始化+创造空实体
    /// </summary>
    /// <param name="path">目标XML文件所在路径</param>
    public AssetBundleDAL(string path)
    {
        m_Path = path;
        m_List = new List<AssetBundleEntity>();
    }
    /// <summary>
    /// 返回XML数据
    /// </summary>
    /// <returns>XML中记录的各种元素链表</returns>
    public List<AssetBundleEntity> GetList()
    {
        m_List.Clear();

        //xml读取 :将加载的数据添加到加载列表(m_List)中
        //XDocument类：隶属于System.Xml.Linq，待学习：详见C#知识
        //xml文件数据读取
        XDocument xDoc = XDocument.Load(m_Path);
        //读取xml的根节点
        XElement root = xDoc.Root;
        //读取根节点下的子结点
        XElement assetBundleNode = root.Element("AssetBundle");
        //获取指定结点的所有子节点
        IEnumerable<XElement> lst = assetBundleNode.Elements("Item");
        //遍历所得子节点的所有元素
        int index = 0;
        foreach (XElement item in lst)
        {
            AssetBundleEntity entity = new AssetBundleEntity();
            entity.Key = "key" + ++index;//打包物编号，格式:key编号
            entity.Name = item.Attribute("Name").Value;//打包物系列名
            entity.Tag = item.Attribute("Tag").Value;//打包物标签
            entity.IsFolder = item.Attribute("IsFolder").Value.Equals("True",System.StringComparison.CurrentCultureIgnoreCase);//打包物是否是文件夹
            entity.IsFirstData = item.Attribute("IsFirstData").Value.Equals("True",System.StringComparison.CurrentCultureIgnoreCase);//打包物是否是初始数据
            //将所有打包物的路径记录下来
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
