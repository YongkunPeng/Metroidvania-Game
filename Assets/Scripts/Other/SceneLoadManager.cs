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

    public void LoadLevelByIndexWithSlider(int index)
    {
        LoadSceneUI loadUI = UIManager.Instance.OpenPanel(UIConst.LoadScene) as LoadSceneUI;
        loadUI.LoadLevelByIndex(index);
    }

    public void LoadLevelByIndexWithDark(int index)
    {
        LoadSceneWithDarkUI loadUI = UIManager.Instance.OpenPanel(UIConst.LoadSceneWithDark) as LoadSceneWithDarkUI;
        loadUI.LoadLevelByIndex(index);
    }
}
