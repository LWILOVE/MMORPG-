using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// ��ק��ɫ����ʱ�����ã�
/// </summary>
public class UISelectRoleDragView : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    //��ק����ʼλ��
    private Vector2 m_DragBeginPos;

    //������ק��λ��
    private Vector2 m_DragEndPos;

    /// <summary>
    /// ��קί�� <��ק����0��1��>
    /// </summary>
    public System.Action<int> OnSelectRoleDrag;
    /// <summary>
    /// ��ʼ��ק
    /// </summary>
    /// <param name="eventData"></param>
    /// <exception cref="System.NotImplementedException"></exception>
    public void OnBeginDrag(PointerEventData eventData)
    {
        m_DragBeginPos = eventData.position;
    }

    /// <summary>
    /// ��ק
    /// </summary>
    /// <param name="eventData"></param>
    /// <exception cref="System.NotImplementedException"></exception>
    public void OnDrag(PointerEventData eventData)
    {

    }

    /// <summary>
    /// ������ק
    /// </summary>
    /// <param name="eventData"></param>
    /// <exception cref="System.NotImplementedException"></exception>
    public void OnEndDrag(PointerEventData eventData)
    {
        m_DragEndPos = eventData.position;
        float x = m_DragBeginPos.x - m_DragEndPos.x;
        //20���ݴ�ֵ
        if (x > 20)
        {
            OnSelectRoleDrag(0);
        }
        else if (x < -20)
        {
            OnSelectRoleDrag(1);
        }
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnDestroy()
    {
        OnSelectRoleDrag = null;
    }
}
