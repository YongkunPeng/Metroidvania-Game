using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoadManager : MonoBehaviour
{
    private static SceneLoadManager _Instance;

    public static SceneLoadManager Instance
    {
        get
        {
            if (_Instance == null)
            {
                _Instance = GameObject.FindObjectOfType<SceneLoadManager>();
                if(_Instance == null)
                {
                    GameObject gameObject = new GameObject("SceneLoadManager");
                    _Instance = gameObject.AddComponent<SceneLoadManager>();
                }
            }
            return _Instance;
        }
    }

    private void Awake()
    {
        if (_Instance != null && _Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
    }

    public void LoadLevelByIndex(int index)
    {
        UIManager.Instance.OpenPanel(UIConst.LoadScene);
        GameObject.FindObjectOfType<LoadSceneUI>().LoadLevelByIndex(index);
    }
}
