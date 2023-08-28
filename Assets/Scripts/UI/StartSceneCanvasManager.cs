using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartSceneCanvasManager : MonoBehaviour
{
    private void Awake()
    {
        UIManager.Instance.OpenPanel(UIConst.MainMenu);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
