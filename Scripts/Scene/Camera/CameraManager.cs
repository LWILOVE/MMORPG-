using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [Header("�ɵ��ڲ���")]
    //��ת�ٶ�
    public float RotateSpeed = 15;

    [Header("�����ο�����")]
    //�����������
    public static CameraManager Instance;
    //����������ƶ�
    public Transform m_CameraUpAndDown;
    //�����ǰ���ƶ�
    public Transform m_CameraZoomContainer;
    //���������
    public Transform m_CameraContainer;
    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(this);
    }

    /// <summary>
    /// ��ʼ�����
    /// </summary>
    public void CameraInit()
    {
        m_CameraUpAndDown.transform.localEulerAngles = new Vector3(Mathf.Clamp(m_CameraUpAndDown.transform.localEulerAngles.x, 0f, 80f), 0, 0);
        m_CameraZoomContainer.localPosition = new Vector3(0, 0, Mathf.Clamp(m_CameraZoomContainer.localPosition.z, -8f, 4f));
    }
    /// <summary>
    /// �����(0)��(1)��ת��
    /// </summary>
    /// <param name="type">0��1��</param>
    public void SetCameraRotate(float type)
    {
        transform.Rotate(0, RotateSpeed * Time.deltaTime * (type > 0 ? 1 : -1), 0);
    }
    /// <summary>
    /// �����(0)��(1)��ת��
    /// </summary>
    /// <param name="type">0��1��</param>
    public void SetCameraUpAndDown(float type)
    {
        m_CameraUpAndDown.transform.Rotate(RotateSpeed * Time.deltaTime * (type > 0 ? 1 : -1), 0, 0);
        m_CameraUpAndDown.transform.localEulerAngles = new Vector3(Mathf.Clamp(m_CameraUpAndDown.transform.localEulerAngles.x, 0f, 80f), 0, 0);
    }
    /// <summary>
    /// �����(1)ǰ(0)��ת��
    /// </summary>
    /// <param name="type">1��0Զ</param>
    public void SetCameraZoom(float type)
    {
        m_CameraZoomContainer.Translate(Vector3.forward * Time.deltaTime * RotateSpeed * ((type > 0 ? 1 : -1)));
        m_CameraZoomContainer.localPosition = new Vector3(0, 0, m_CameraZoomContainer.localPosition.z);
    }
    /// <summary>
    /// ��ͷ��Զ��������
    /// </summary>
    /// <param name="pos">���ǵ�λ��</param>
    public void AutoLookAt(Vector3 pos)
    {
        m_CameraContainer.LookAt(pos);
    }
    //�����Ӿ�Ȧ
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 15f);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, 14f);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, 13f);
    }

    /// <summary>
    /// ����Ч��
    /// </summary>
    /// <param name="delay"></param>
    /// <param name="duration"></param>
    /// <param name="strength"></param>
    /// <param name="vibrato"></param>
    /// <returns></returns>
    public void CameraShake(float delay = 0f, float duration = 1f, float strength = 1f, int vibrato = 10)
    {
        StartCoroutine(DOCameraShake(delay, duration, strength, vibrato));
    }
    /// <summary>
    /// ����Ч��
    /// </summary>
    /// <param name="delay"></param>
    /// <param name="duration"></param>
    /// <param name="strength"></param>
    /// <param name="vibrato"></param>
    /// <returns></returns>
    private IEnumerator DOCameraShake(float delay = 0f, float duration = 1f, float strength = 1f, int vibrato = 10)
    {
        yield return new WaitForSeconds(delay);
        m_CameraContainer.DOShakePosition(1f, 1, 10);
    }
}
