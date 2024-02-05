using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIRoleInfoDragView : UISubViewBase, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    /// <summary>
    /// Ҫ��ת��Ŀ�� 
    /// </summary>
    [SerializeField]
    private Transform m_Target;

    /// <summary>
    /// ��ק����ʼλ��
    /// </summary>
    private Vector2 m_DragBeginPos;

    /// <summary>
    /// ������ק��λ��
    /// </summary>
    private Vector2 m_DragEndPos;

    /// <summary>
    /// ��ת�ٶ�
    /// </summary>
    private float m_Speed = 300;

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
    /// ��ק��ɫ��ת
    /// </summary>
    /// <param name="eventData"></param>
    /// <exception cref="System.NotImplementedException"></exception>
    public void OnDrag(PointerEventData eventData)
    {
        m_DragEndPos = eventData.position;
        float x = m_DragBeginPos.x - m_DragEndPos.x;
        m_Target.Rotate(0,Time.deltaTime*m_Speed*(x>0?1:-1),0);
        m_DragBeginPos = m_DragEndPos;
    }

    /// <summary>
    /// ������ק
    /// </summary>
    /// <param name="eventData"></param>
    /// <exception cref="System.NotImplementedException"></exception>
    public void OnEndDrag(PointerEventData eventData)
    {


    }
}
