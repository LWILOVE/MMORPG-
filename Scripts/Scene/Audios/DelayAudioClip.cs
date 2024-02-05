using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DelayAudioClip 
{
    /// <summary>
    /// 要播放的音乐片段名
    /// </summary>
    public string AudioClipName;

    /// <summary>
    /// 音乐播放延迟时间
    /// </summary>
    public float DelayTime;
}
