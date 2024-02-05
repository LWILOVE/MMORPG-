using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UITextForHUD : MonoBehaviour
{
    [HideInInspector]
    public float duration;
    public AnimationCurve scaleCurve;
    public AnimationCurve velocityCurve;
    public AnimationCurve offsetCurve;
    [HideInInspector]
    public float t_time = 0;
    [HideInInspector]
    public RectTransform rectTransform;
    //初始位置
    [HideInInspector]
    public Vector2 recPos;
    //初始规模
    [HideInInspector]
    public Vector3 recScale;

    float move_x;
    float move_y;
    // Start is called before the first frame update

    private void Awake()
    {
        t_time = 0;
        rectTransform = gameObject.GetComponent<RectTransform>();
        move_x = Screen.width;
        move_y = Screen.height;
        if (rectTransform == null)
        {
            recScale = gameObject.transform.localScale;
            gameObject.transform.position = Vector3.zero;
        }
        else
        {
            rectTransform.anchoredPosition3D = Vector3.zero;
            recScale = rectTransform.localScale;
        }
    }
    // Update is called once per frame
    void Update()
    {
        t_time += Time.deltaTime;
        if (t_time > duration)
        {
            Destroy(gameObject);
        }
        if (rectTransform == null)
        {
            gameObject.transform.localScale = recScale + Vector3.one*scaleCurve.Evaluate(t_time);
            gameObject.transform.position = new Vector2(offsetCurve.Evaluate(t_time)*10,velocityCurve.Evaluate(t_time)*10);
        }
        else
        {
            rectTransform.anchoredPosition = new Vector2(offsetCurve.Evaluate(t_time)*move_x, velocityCurve.Evaluate(t_time)*move_y); 
            rectTransform.localScale = recScale + Vector3.one * scaleCurve.Evaluate(t_time); 
        }
    }
}
