using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [Header("可调节部分")]
    //旋转速度
    public float RotateSpeed = 15;

    [Header("仅供参考部分")]
    //定义相机单例
    public static CameraManager Instance;
    //摄像机上下移动
    public Transform m_CameraUpAndDown;
    //摄像机前后移动
    public Transform m_CameraZoomContainer;
    //摄像机容器
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
    /// 初始化相机
    /// </summary>
    public void CameraInit()
    {
        m_CameraUpAndDown.transform.localEulerAngles = new Vector3(Mathf.Clamp(m_CameraUpAndDown.transform.localEulerAngles.x, 0f, 80f), 0, 0);
        m_CameraZoomContainer.localPosition = new Vector3(0, 0, Mathf.Clamp(m_CameraZoomContainer.localPosition.z, -8f, 4f));
    }
    /// <summary>
    /// 摄像机(0)左(1)右转动
    /// </summary>
    /// <param name="type">0左1右</param>
    public void SetCameraRotate(float type)
    {
        transform.Rotate(0, RotateSpeed * Time.deltaTime * (type > 0 ? 1 : -1), 0);
    }
    /// <summary>
    /// 摄像机(0)上(1)下转动
    /// </summary>
    /// <param name="type">0上1下</param>
    public void SetCameraUpAndDown(float type)
    {
        m_CameraUpAndDown.transform.Rotate(RotateSpeed * Time.deltaTime * (type > 0 ? 1 : -1), 0, 0);
        m_CameraUpAndDown.transform.localEulerAngles = new Vector3(Mathf.Clamp(m_CameraUpAndDown.transform.localEulerAngles.x, 0f, 80f), 0, 0);
    }
    /// <summary>
    /// 摄像机(1)前(0)后转动
    /// </summary>
    /// <param name="type">1近0远</param>
    public void SetCameraZoom(float type)
    {
        m_CameraZoomContainer.Translate(Vector3.forward * Time.deltaTime * RotateSpeed * ((type > 0 ? 1 : -1)));
        m_CameraZoomContainer.localPosition = new Vector3(0, 0, m_CameraZoomContainer.localPosition.z);
    }
    /// <summary>
    /// 镜头永远朝向主角
    /// </summary>
    /// <param name="pos">主角的位置</param>
    public void AutoLookAt(Vector3 pos)
    {
        m_CameraContainer.LookAt(pos);
    }
    //绘制视觉圈
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
    /// 震屏效果
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
    /// 震屏效果
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
