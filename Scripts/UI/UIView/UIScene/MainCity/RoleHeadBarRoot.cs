using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 角色名字跟随角色基类   
/// </summary>
public class RoleHeadBarRoot : MonoBehaviour
{
    public static RoleHeadBarRoot Instance;
    private void Awake()
    {
        Instance = this;
    }
}
