using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// 商品数据管理类--工具生成类，修改需谨慎
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
