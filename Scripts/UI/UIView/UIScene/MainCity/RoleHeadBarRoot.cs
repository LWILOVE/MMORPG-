using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ��ɫ���ָ����ɫ����   
/// </summary>
public class RoleHeadBarRoot : MonoBehaviour
{
    public static RoleHeadBarRoot Instance;
    private void Awake()
    {
        Instance = this;
    }
}
