using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 商品实体类---工具生成类，修改需谨慎
/// </summary>
public partial class ProductEntity : AbstractEntity
{

    
    /// <summary>
    /// 商品名称
    /// </summary>
    public string Name
    { get; set; }

    /// <summary>
    /// 商品价格
    /// </summary>
    public float Price
    { get; set; }

    /// <summary>
    /// 商品图片名
    /// </summary>
    public string PicName
    { get; set; }

    /// <summary>
    /// 商品描述
    /// </summary>
    public string Describe
    { get; set; }
}
