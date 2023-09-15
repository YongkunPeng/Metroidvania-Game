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
    /// ���ص����˵�
    /// </summary>
    public void LoadMainMenu()
    {
        SceneLoadManager.Instance.LoadLevelByIndexWithSlider(0);
    }

    /// <summary>
    /// ���¿�ʼ��Ϸ����������浵��
    /// </summary>
    public void RestartGame()
    {
        AudioSourceManager.Instance.PlayBGM(GlobalAudioClips.BGM1);
        GameManager.Instance.userData = LocalConfig.LoadUserData(GameManager.Instance.username);
        GameManager.Instance.shouldTransmit = true; // �������ؼ�¼��
        GameManager.Instance.InitUserData(GameManager.Instance.userData, true);
    }

    private void OnDestroy()
    {
        ClosePanel();
        GameManager.Instance.isPaused = false;
        Time.timeScale = 1;
    }
}
