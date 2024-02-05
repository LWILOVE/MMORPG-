using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Text的数字文字 自动动画显示
/// </summary>
public class AutoNumberAnimation : MonoBehaviour
{
    private Queue<int> m_Queue = new Queue<int>();
    private Text m_text;
    private List<int> m_list = new List<int>();
    private bool m_IsBusy = false;

    private void Start()
    {
        
    }

    public void DoNumber(int number)
    {
        m_Queue.Enqueue(number);
        Debug.Log("加入队列==" + number);
        CheckQueue();
    }

    private void CheckQueue()
    {
        //检查队列
        if (m_Queue.Count >= 1 && !m_IsBusy)
        {
            DoAnim();
        }
    }

    private void DoAnim()
    {
        if (m_Queue.Count >= 1)
        {
            m_IsBusy = true;
            m_list.Clear();

            //从队列中取出一个
            int toValue = m_Queue.Dequeue();
            if (m_text == null)
            {
                m_text = GetComponent<Text>();
            }
            int currValue = 0;
            int.TryParse(m_text.text,out currValue);
            //计算差额 算出递增或者递减值
            int value = toValue - currValue;
            int step = (int)(value/20f);
            if (value > 0)
            {
                step = Mathf.Clamp(step, 1, step);
            }
            else
            {
                step = Mathf.Clamp(step,step,-1);
            }
            int animValue = currValue;
            if (value > 0)
            {
                while (animValue > toValue)
                {
                    m_list.Add(animValue);
                    animValue += step;
                }
            }
            m_list.Add(toValue);
            StartCoroutine(DoText());
        }
    }

    private IEnumerator DoText()
    {
        if (m_list.Count > 0)
        {
            for (int i = 0; i < m_list.Count; i++)
            {
                m_text.text = m_list[i].ToString();
                yield return new WaitForSeconds(0.02f);
            }
            m_list.Clear();
            m_IsBusy = false;
            Debug.Log("执行动画完毕");
            CheckQueue();
        }
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
        m_Queue.Clear();
        m_Queue = null;
        m_text = null;
        m_list.Clear();
        m_list = null;
    }
}
