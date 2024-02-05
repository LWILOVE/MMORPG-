using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// ��Ʒ���ݹ�����--���������࣬�޸������
/// </summary>
public partial class ProductDBModel : AbstractDBModel<ProductDBModel, ProductEntity>
{
    protected override string FileName { get { return "Product.data"; } }

    protected override ProductEntity MakeEntity(GameDataTableParser parse)
    {
        ProductEntity entity = new ProductEntity();

        entity.Id = parse.GetFieldValue("Id").ToInt();
        entity.Name = parse.GetFieldValue("Name");
        entity.Price = parse.GetFieldValue("Price").ToFloat();
        entity.PicName = parse.GetFieldValue("PicName");
        entity.Describe = parse.GetFieldValue("Describe");
        return entity;
    }
}
