using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// 拖拽角色（暂时不想用）
/// </summary>
public class UISelectRoleDragView : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    //拖拽的起始位置
    private Vector2 m_DragBeginPos;

    //结束拖拽的位置
    private Vector2 m_DragEndPos;

    /// <summary>
    /// 拖拽委托 <拖拽方向0左1右>
    /// </summary>
    public System.Action<int> OnSelectRoleDrag;
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
    /// 拖拽
    /// </summary>
    /// <param name="eventData"></param>
    /// <exception cref="System.NotImplementedException"></exception>
    public void OnDrag(PointerEventData eventData)
    {

    }

    /// <summary>
    /// 结束拖拽
    /// </summary>
    /// <param name="eventData"></param>
    /// <exception cref="System.NotImplementedException"></exception>
    public void OnEndDrag(PointerEventData eventData)
    {
        m_DragEndPos = eventData.position;
        float x = m_DragBeginPos.x - m_DragEndPos.x;
        //20：容错值
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
