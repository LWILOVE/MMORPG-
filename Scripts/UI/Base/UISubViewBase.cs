using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// UI����������д��
/// </summary>
public class UISubViewBase : MonoBehaviour
{
    private void Awake()
    {
        OnAwake();
    }

    private void Start()
    {
        OnStart();
    }

    private void Destroy()
    {
        BeforeDestroy();
    }

    protected virtual void OnAwake() { }

    protected virtual void OnStart() { }

    protected virtual void BeforeDestroy() { }
    


}
