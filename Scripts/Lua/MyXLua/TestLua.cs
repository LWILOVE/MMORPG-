using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestLua : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Button btn = transform.GetComponent<UnityEngine.UI.Button>();
        btn.onClick.AddListener(null);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
