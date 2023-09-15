using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RestartMenuUI : BasePanel
{
    private void Awake()
    {
        OpenPanel(UIConst.RestartMenu);
    }

    private void Update()
    {
        if (gameObject.activeInHierarchy && Time.timeScale != 0)
        {
            GameManager.Instance.isPaused = true;
            Time.timeScale = 0;
        }
    }

    /// <summary>
    /// 加载到主菜单
    /// </summary>
    public void LoadMainMenu()
    {
        SceneLoadManager.Instance.LoadLevelByIndexWithSlider(0);
    }

    /// <summary>
    /// 重新开始游戏，返回最近存档点
    /// </summary>
    public void RestartGame()
    {
        AudioSourceManager.Instance.PlayBGM(GlobalAudioClips.BGM1);
        GameManager.Instance.userData = LocalConfig.LoadUserData(GameManager.Instance.username);
        GameManager.Instance.shouldTransmit = true; // 死亡返回记录点
        GameManager.Instance.InitUserData(GameManager.Instance.userData, true);
    }

    private void OnDestroy()
    {
        ClosePanel();
        GameManager.Instance.isPaused = false;
        Time.timeScale = 1;
    }
}
