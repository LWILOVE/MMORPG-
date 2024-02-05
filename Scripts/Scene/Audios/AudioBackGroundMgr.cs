using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ���������ܹ�������֧�����ֵĵ���͵���
/// </summary>
[RequireComponent(typeof(AudioSource))]//ÿ����Ч����Ҫ��Դ������ҪЯ�����
public class AudioBackGroundMgr : MonoBehaviour
{
    
    /// <summary>
    /// ���ڲ������ֵ���Դ
    /// </summary>
    private AudioSource m_AudioSource;
    
    /// <summary>
    /// ��һ�����ŵ�����Ƭ��
    /// </summary>
    private AudioClip m_PrevAudioClip;
    /// <summary>
    /// ���ŵı���������
    /// </summary>
    private string m_AudioName;

    /// <summary>
    /// �������
    /// </summary>
    private float m_MaxVolume = 0.2f;

    /// <summary>
    /// ����
    /// </summary>
    public static AudioBackGroundMgr Instance;


    private void Awake()
    {
        Instance = this;
        //������Դ����
        m_AudioSource = GetComponent<AudioSource>();
        m_AudioSource.volume = 0f;
        m_AudioSource.loop = true;
        //�趨��Դ�����2D���֣�0��3D���֣�1��2DΪȫ������3DΪ������
        m_AudioSource.spatialBlend = 0;
        DontDestroyOnLoad(gameObject);   
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    /// <summary>
    /// ���ű�������
    /// </summary>
    /// <param name="name">������</param>
    public void Play(string name)
    {
        m_AudioName = name;
        StartCoroutine(DoPlay());
    }

    private IEnumerator DoPlay()
    {
        //������Ҫʱ��
        float fadeOut = 1;
        //����ѡӴ��ʱ��
        float fadeIn = 1;
        //�ӳ�ʱ��
        float delay = 0;
        AudioClip audioClip = null;
        ////��������Ƭ��
        ///�˴��汾�ѵ�����TODO
        //audioClip = UnityEditor.AssetDatabase.LoadAssetAtPath<AudioClip>(string.Format("Assets/Download/Audio/BackGround/{0}.mp3",m_AudioName));
      
        //�����ǰ���ڲ��ŵľ���������֣��ǾͲ�����
        if (m_AudioSource.isPlaying && m_AudioSource.clip == audioClip)
        {
            yield return 0;
        }
        else
        {
            float time1 = Time.time;
            //��������
            //1.����Ƿ�������û���������
            if (m_PrevAudioClip != null)
            {
                //����һ�����ֵ���
                yield return StartCoroutine(StartFadeOut(fadeOut));
            }

            //�����ӳ�ʱ��
            float time2 = Time.time - time1;
            if(delay > time2)
            {
                yield return new WaitForSeconds(delay-time2);
            }

            //�������֣�����������
            m_AudioSource.clip = audioClip;
            m_PrevAudioClip = audioClip;
            m_AudioSource.Play();

            //���������е���
            yield return StartCoroutine(StartFadeIn(fadeIn));
        }
    }
    /// <summary>
    /// ��������Э��  
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
    /// ��������Э��
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
