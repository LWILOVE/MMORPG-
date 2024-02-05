using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class T4MGround : MonoBehaviour
{
    
    private void Start()
    {
        Renderer[] arr = GetComponentsInChildren<Renderer>(true);

        if (arr != null && arr.Length > 0)
        {
            for (int i = 0; i < arr.Length; i++)
            {
                //重新设定物体的Shader
                arr[i].material.shader = GlobalInit.Instance.T4MShaeder;
            }
        }
        Destroy(this);
    }
}
