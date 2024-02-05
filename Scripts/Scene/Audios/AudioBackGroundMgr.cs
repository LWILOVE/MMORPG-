using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 背景音乐总管理器（支持音乐的淡入和淡出
/// </summary>
[RequireComponent(typeof(AudioSource))]//每个音效都需要音源，所以要携带这个
public class AudioBackGroundMgr : MonoBehaviour
{
    
    /// <summary>
    /// 正在播放音乐的音源
    /// </summary>
    private AudioSource m_AudioSource;
    
    /// <summary>
    /// 上一个播放的声音片段
    /// </summary>
    private AudioClip m_PrevAudioClip;
    /// <summary>
    /// 播放的背景音乐名
    /// </summary>
    private string m_AudioName;

    /// <summary>
    /// 最大音量
    /// </summary>
    private float m_MaxVolume = 0.2f;

    /// <summary>
    /// 单例
    /// </summary>
    public static AudioBackGroundMgr Instance;


    private void Awake()
    {
        Instance = this;
        //设置音源属性
        m_AudioSource = GetComponent<AudioSource>();
        m_AudioSource.volume = 0f;
        m_AudioSource.loop = true;
        //设定音源的类别：2D音乐：0，3D音乐：1：2D为全景音，3D为距离音
        m_AudioSource.spatialBlend = 0;
        DontDestroyOnLoad(gameObject);   
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    /// <summary>
    /// 播放背景音乐
    /// </summary>
    /// <param name="name">音乐名</param>
    public void Play(string name)
    {
        m_AudioName = name;
        StartCoroutine(DoPlay());
    }

    private IEnumerator DoPlay()
    {
        //淡出需要时间
        float fadeOut = 1;
        //淡入选哟的时间
        float fadeIn = 1;
        //延迟时间
        float delay = 0;
        AudioClip audioClip = null;
        ////加载音乐片段
        ///此处版本已迭代，TODO
        //audioClip = UnityEditor.AssetDatabase.LoadAssetAtPath<AudioClip>(string.Format("Assets/Download/Audio/BackGround/{0}.mp3",m_AudioName));
      
        //如果当前正在播放的就是这个音乐，那就不处理
        if (m_AudioSource.isPlaying && m_AudioSource.clip == audioClip)
        {
            yield return 0;
        }
        else
        {
            float time1 = Time.time;
            //播放音乐
            //1.检查是否有其他没放完的音乐
            if (m_PrevAudioClip != null)
            {
                //将上一个音乐淡出
                yield return StartCoroutine(StartFadeOut(fadeOut));
            }

            //设置延迟时间
            float time2 = Time.time - time1;
            if(delay > time2)
            {
                yield return new WaitForSeconds(delay-time2);
            }

            //播放音乐，并更新属性
            m_AudioSource.clip = audioClip;
            m_PrevAudioClip = audioClip;
            m_AudioSource.Play();

            //将声音进行淡入
            yield return StartCoroutine(StartFadeIn(fadeIn));
        }
    }
    /// <summary>
    /// 声音淡出协程  
    /// </summary>
    /// <param name="fadeOut"></param>
    /// <returns></returns>
    private IEnumerator StartFadeOut(float fadeOut)
    {
        float time = 0f;
        while (time <= fadeOut)
        {
            if (time != 0)
            {
                m_AudioSource.volume = Mathf.Lerp(m_MaxVolume,0f,time/fadeOut);
            }
            time += Time.deltaTime;
            yield return 1;
        }
        m_AudioSource.volume = 0;
    }

    /// <summary>
    /// 声音淡入协程
    /// </summary>
    /// <param name="fadeIn"></param>
    /// <returns></returns>
    private IEnumerator StartFadeIn(float fadeIn)
    {
        float time = 0f;
        while (time <= fadeIn)
        {
            if (time != 0)
            {
                m_AudioSource.volume = Mathf.Lerp( 0f, m_MaxVolume, time / fadeIn);
            }
            time += Time.deltaTime;
            yield return 1;
        }
        m_AudioSource.volume = m_MaxVolume;
    }
}
