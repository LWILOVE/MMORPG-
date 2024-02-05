using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCWorldMapData
{
    /// <summary>
    /// ID
    /// </summary>
    public int NPCId;//ID
    /// <summary>
    /// NPC������
    /// </summary>
    public Vector3 NPCPosition { get; set; }//NPC������
    /// <summary>
    /// NPC��������
    /// </summary>
    public float EulerAnglesY { get; set; }//NPC������
    /// <summary>
    /// NPC�Ŀ�����
    /// </summary>
    public string Prologue { get; set; }//NPC�Ŀ�����
}
