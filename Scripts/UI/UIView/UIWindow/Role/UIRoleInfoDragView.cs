using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIRoleInfoDragView : UISubViewBase, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    /// <summary>
    /// 要旋转的目标 
    /// </summary>
    [SerializeField]
    private Transform m_Target;

    /// <summary>
    /// 拖拽的起始位置
    /// </summary>
    private Vector2 m_DragBeginPos;

    /// <summary>
    /// 结束拖拽的位置
    /// </summary>
    private Vector2 m_DragEndPos;

    /// <summary>
    /// 旋转速度
    /// </summary>
    private float m_Speed = 300;

    /// <summary>
    /// 开始拖拽
    /// </summary>
    /// <param name="eventData"></param>
    /// <exception cref="System.NotImplementedException"></exception>
    public void OnBeginDrag(PointerEventData eventData)
    {
        m_DragBeginPos = eventData.position;
    }

    /// <summary>
    /// 拖拽角色旋转
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
    /// 结束拖拽
    /// </summary>
    /// <param name="eventData"></param>
    /// <exception cref="System.NotImplementedException"></exception>
    public void OnEndDrag(PointerEventData eventData)
    {


    }
}
