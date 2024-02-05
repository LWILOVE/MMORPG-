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
    /// NPC出生地
    /// </summary>
    public Vector3 NPCPosition { get; set; }//NPC出生地
    /// <summary>
    /// NPC的面向方向
    /// </summary>
    public float EulerAnglesY { get; set; }//NPC的面向
    /// <summary>
    /// NPC的开场白
    /// </summary>
    public string Prologue { get; set; }//NPC的开场白
}
