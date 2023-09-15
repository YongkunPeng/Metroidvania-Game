using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class StartSceneCanvasManager : MonoBehaviour
{
    private void Awake()
    {
        if (!File.Exists(Application.persistentDataPath + "/users"))
        { // �������򴴽����ļ�
            System.IO.Directory.CreateDirectory(Application.persistentDataPath + "/users");
        }
        UIManager.Instance.OpenPanel(UIConst.MainMenu);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
