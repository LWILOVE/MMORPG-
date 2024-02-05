using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

/// <summary>
/// ��Ч������ 
/// </summary>
public class AudioEffectMgr : MonoBehaviour
{
    public static AudioEffectMgr Instance = null;

    void Awake()
    {
        //Ψһʵ��
        Instance = this;
    }

    public static float Volume = 1f;//������ȡֵ��Χ��0-1

    //���������������б�����PoolManager��
    private List<AudioInfo> m_AudioList = new List<AudioInfo>();

    /// <summary>
    /// UI��Ч
    /// </summary>
    /// <param name="type"></param>
    public void PlayUIAudioEffect(UIAudioEffectType type)
    {
        Play("Download/Audio/UI/" + type.ToString() + ".mp3", Vector3.zero);
    }

    /// <summary>
    /// ��ָ����λ�ã�����ָ�����Ƶ�����
    /// </summary>
    /// <param name="AudioNameID"></param>
    /// <param name="pos"></param>
    public void Play(string audioPath, Vector3 pos, bool is3D = false)
    {
        //�˴��汾�ѵ���TODO
        //AudioClip audioClip = AssetDatabase.LoadAssetAtPath<AudioClip>(string.Format("Assets/{0}", audioPath)); 
        AudioClip audioClip = null;

        if (audioClip == null) return;

        AudioInfo info = FindSameAudio(audioClip.name);
        if (info != null)
        {
            //��������б��� ����ͬ������
            //������audioclip��ȷ���������µģ��ǳ���Ҫ��
            info.AudioName = audioClip.name;
            info.CurrAudioSource.clip = audioClip;

            //��ʼ����
            info.Play(pos, is3D);
        }
        else
        {
            //�Ƴ����ڵ�
            RemoveOverSound();

            //�½�һ��
            info = new AudioInfo(audioClip, gameObject);
            m_AudioList.Add(info);

            //��ʼ����
            info.Play(pos, is3D);
        }
    }

    /// <summary>
    /// ֹͣ��������
    /// </summary>
    public void StopAllAudio()
    {
        for (int i = m_AudioList.Count - 1; i >= 0; i--)
        {
            m_AudioList[i].Destroy();
        }

        m_AudioList.Clear();
    }

    /// <summary>
    /// ������ָ�����Ƶ��������Ƿ����б��д��ڣ����ҿ����ڲ���������
    /// ����
    ///    1������б�����ͬ���Ѿ�������ϵ���������ֱ�ӽ��䷵�ء�
    ///    2�����������������ͬ���ģ��������ڲ��ŵ�������������н���ʱ��������Ǹ�ֱ�ӷ��أ������ͻ�ѽ�����������Ǹ���ǰֹͣ����Ϊ�����������ˣ�
    ///       ֮�����ж���������Ϊ��������ʵ�У����ж�1���ῴ���е�١����̫�����̫�˷�cpu���ڴ�
    ///    3������������������������㣬��ֱ�ӷ���null
    /// </summary>
    /// <param name="audioName"></param>
    /// <returns></returns>
    private AudioInfo FindSameAudio(string audioName)
    {
        //----------------------
        //����б����Ѿ��������ˣ�����ֱ�ӷ���
        foreach (AudioInfo infoItem in m_AudioList)
        {
            if (Time.time > infoItem.PlayEndTime)
            {
                return infoItem;
            }
        }
        //����ִ�е������ʾ��û�в�����

        
        //----------------------
        //�ж��Ƿ����û�в�����ϵ�ͬ��������
        List<AudioInfo> infoArray = new List<AudioInfo>();
        foreach (AudioInfo infoItem in m_AudioList)
        {
            if (infoItem.AudioName == audioName)
            {
                infoArray.Add(infoItem);
            }
        }

        //���Ŀǰֻ��һ�����ڲ��ŵ�ͬ������������һ��Ҳû�У���ֱ�ӷ���
        if (infoArray.Count <= 1)
        {
            infoArray = null;
            return null;
        }
        //����ִ�е�����ͱ�ʾ�����Ѿ�����2��ͬ���ġ����ڲ��ŵ���������ѽ���ʱ��������Ǹ���Ϊ����ֵ����(�������Ǿ�ʵ����ֹͣ������Ǹ������������Ϊ������������)
        AudioInfo info = infoArray[0];
        for (int i = 1; i < infoArray.Count; i++)
        {
            if (info.PlayEndTime > infoArray[i].PlayEndTime)
            {
                info = infoArray[i];
            }
        }
        infoArray = null;
        return info;
    }

    private void RemoveOverSound()
    {
        //����б��еĸ���������8�������Ȱ��Ѿ�������ϵģ� ȫ���Ƴ���
        if (m_AudioList.Count >= 8)
        {
            for (int i = m_AudioList.Count - 1; i >= 0; i--)
            {
                if (Time.time > m_AudioList[i].PlayEndTime)
                {
                    AudioInfo item = m_AudioList[i];
                    m_AudioList.Remove(item);
                    item.Destroy();//��Ҫ��
                }
            }
        }

        //����б��еĸ�����Ȼ����8�����������������Ƴ�����ֱ������8��
        while (m_AudioList.Count >= 8)
        {
            AudioInfo item = m_AudioList[0];
            foreach (AudioInfo info2 in m_AudioList)
            {
                if (item.PlayEndTime > info2.PlayEndTime)
                {
                    item = info2;
                }
            }
            m_AudioList.Remove(item);
            item.Destroy();//��Ҫ��
        }
    }
}

//����������(��Դ)
public class AudioInfo
{
    public AudioSource CurrAudioSource;//��Ƶ������
    public string AudioName;//��Ч�����ַ���
    public float PlayEndTime = 0;//���Ž���ʱ��
    public bool Is3D;

    /// <summary>
    /// ���캯��
    /// </summary>
    /// <param name="audioclip">��Ч</param>
    /// <param name="root">��Ӧ��GameObject�ĸ�����</param>
    public AudioInfo(AudioClip audioclip, GameObject root = null)
    {
        AudioName = audioclip.name;

        //--------------
        //�½�һ��gameObject
        GameObject obj = new GameObject("Audio_" + AudioName);
        if (root != null)
        {
            obj.transform.parent = root.transform;
        }

        //������Կ糡�����ͷţ���Ҫ������
        Object.DontDestroyOnLoad(obj);

        //������½���gameObject�Ϲ���һ��AudioClip���
        CurrAudioSource = obj.AddComponent<AudioSource>();
        CurrAudioSource.loop = false;//��ѭ��
        CurrAudioSource.volume = AudioEffectMgr.Volume;//������С(0-1)
        CurrAudioSource.rolloffMode = AudioRolloffMode.Linear;//������˥��ģʽ---����˥��
        CurrAudioSource.minDistance = 30;//������˥��ģʽ---��ʼ˥�����루��AudioSource��AudioListener֮��ľ��룬���������ֵʱ�ſ�ʼ����˥����
        CurrAudioSource.maxDistance = 200;//������˥��ģʽ---����˥�����루��AudioSource��AudioListener֮��ľ��룬С�������ֵʱ��������Ҳ��������
        CurrAudioSource.clip = audioclip;
        CurrAudioSource.panStereo = 1;
    }

    /// <summary>
    /// �ͷ���������ʱ����������ᱻ�ֹ�����
    /// </summary>
    public void Destroy()
    {
        //ֹͣ������Ч
        Stop();

        //�Ѷ�Ӧ��GameObjectҲ�ͷŵ�
        Object.Destroy(this.CurrAudioSource.gameObject);
    }

    /// <summary>
    /// ��ָ��������㴦����ʼ������Ч
    /// </summary>
    /// <param name="pos">3D�����ڵ�һ������㣬Ҫ��������㴦������������ע:u3d��3d��������Դ��λ���й�ϵ��ԽԶ������ԽС��</param>
    public void Play(Vector3 pos, bool is3D = false)
    {
        //��¼�²��Ž���ʱ��
        PlayEndTime = Time.time + this.CurrAudioSource.clip.length;

        //��gameobject�ƶ���Ҫ���λ��
        CurrAudioSource.gameObject.transform.position = pos;
        CurrAudioSource.Stop();

        //��ʼ����
        CurrAudioSource.spatialBlend = is3D ? 1 : 0;
        CurrAudioSource.Play();
    }

    /// <summary>
    /// ֹͣ����
    /// </summary>
    public void Stop()
    {
        CurrAudioSource.Stop();//ֹͣ��������
        PlayEndTime = 0f;//�Ѳ��Ž���ʱ������Ϊ0
    }
}