using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class GameLevelGradeEntity
{
    /// <summary>
    /// 当前难度等级
    /// </summary>
    public GameLevelGrade CurrentGrade
    {
        get { return (GameLevelGrade)Grade; }
    }

    #region 奖励列表
    /// <summary>
    /// 关卡奖励的装备列表
    /// </summary>
    private List<GoodsEntity> m_EquipList;
    public List<GoodsEntity> EquipList
    {
        get 
        {
            if (m_EquipList == null)
            {
                m_EquipList = new List<GoodsEntity>();
                string[] arr = Equip.Split("|");
                if (arr.Length > 0)
                {
                    for (int i = 0; i < arr.Length; i++)
                    {
                        string[] arr2 = arr[i].Split('_');
                        if (arr2.Length >= 3)
                        {
                            GoodsEntity entity = new GoodsEntity();
                            int goodsId = 0;
                            int.TryParse(arr2[0],out goodsId);

                            int probability = 0;
                            int.TryParse(arr2[1], out probability);

                            int count = 0;
                            int.TryParse(arr2[2], out count);

                            string name = string.Empty;
                            EquipEntity equipEntity = EquipDBModel.Instance.Get(goodsId);

                            if (equipEntity != null)
                            {
                                name = equipEntity.Name;
                            }
                            entity.Id = goodsId;
                            entity.Name = name;
                            entity.Probability = probability;
                            entity.count = count;

                            m_EquipList.Add(entity);
                        }
                    }
                }
            }
            return m_EquipList;
        }
    }

    /// <summary>
    /// 关卡奖励的道具列表
    /// </summary>
    private List<GoodsEntity> m_ItemList;
    public List<GoodsEntity> ItemList
    {
        get
        {
            if (m_ItemList == null)
            {
                m_ItemList = new List<GoodsEntity>();
                string[] arr = Equip.Split("|");
                if (arr.Length > 0)
                {
                    for (int i = 0; i < arr.Length; i++)
                    {
                        string[] arr2 = arr[i].Split('_');
                        if (arr2.Length >= 3)
                        {
                            GoodsEntity entity = new GoodsEntity();
                            int goodsId = 0;
                            int.TryParse(arr2[0], out goodsId);

                            int probability = 0;
                            int.TryParse(arr2[1], out probability);

                            int count = 0;
                            int.TryParse(arr2[2], out count);

                            if (goodsId == 120225)
                            { goodsId = 110336; }
                            if (goodsId == 120214)
                            { goodsId = 110341; }
                            if (goodsId == 120211)
                            { goodsId = 110321; }

                            string name = string.Empty;
                            ItemEntity itemEntity = ItemDBModel.Instance.Get(goodsId);

                            if (itemEntity != null)
                            {
                                name = itemEntity.Name;
                            }
                            entity.Id = goodsId;
                            entity.Name = name;
                            entity.Probability = probability;
                            entity.count = count;

                            m_ItemList.Add(entity);
                        }
                    }
                }
            }
            return m_ItemList;
        }
    }

    /// <summary>
    /// 关卡奖励的材料列表
    /// </summary>
    private List<GoodsEntity> m_MaterialList;
    public List<GoodsEntity> MaterialList
    {
        get
        {
            if (m_MaterialList == null)
            {
                m_MaterialList = new List<GoodsEntity>();
                string[] arr = Equip.Split("|");
                if (arr.Length > 0)
                {
                    for (int i = 0; i < arr.Length; i++)
                    {
                        string[] arr2 = Material.Split('_');
                        if (arr2.Length >= 3)
                        {
                            GoodsEntity entity = new GoodsEntity();
                            int goodsId = 0;
                            int.TryParse(arr2[0], out goodsId);

                            int probability = 0;
                            int.TryParse(arr2[1], out probability);

                            int count = 0;
                            int.TryParse(arr2[2], out count);

                            string name = string.Empty;
                            MaterialEntity itemEntity = MaterialDBModel.Instance.Get(goodsId);

                            if (itemEntity != null)
                            {
                                name = itemEntity.Name;
                            }
                            entity.Id = goodsId;
                            entity.Name = name;
                            entity.Probability = probability;
                            entity.count = count;

                            m_MaterialList.Add(entity);
                        }
                    }
                }
            }
            return m_MaterialList;
        }
    }
    #endregion

}
