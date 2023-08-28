using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuUI : BasePanel
{
    [SerializeField] private Button newGameBtn;
    [SerializeField] private Button continueGameBtn;
    [SerializeField] private Button settingsBtn;
    [SerializeField] private Button quitBtn;
    [SerializeField] private GraphicRaycaster ray;
    private FileInfo[] files;

    private void Awake()
    {
        OpenPanel(UIConst.MainMenu);

        newGameBtn = transform.GetChild(1).GetChild(1).GetComponent<Button>();
        continueGameBtn = transform.GetChild(1).GetChild(2).GetComponent<Button>();
        settingsBtn = transform.GetChild(1).GetChild(3).GetComponent<Button>();
        quitBtn = transform.GetChild(1).GetChild(4).GetComponent<Button>();
        ray = GetComponent<GraphicRaycaster>();

        DirectoryInfo directoryInfo = new DirectoryInfo(Application.persistentDataPath + "/users");
        files = directoryInfo.GetFiles("*.json")
            .Where(file => file.Extension.ToLower() == ".json")
            .ToArray();

        ContinueGameBtnControll();
    }

    private void FixedUpdate()
    {
        CheckCanRaycast();
    }

    /// <summary>
    /// 检查是否可交互
    /// </summary>
    private void CheckCanRaycast()
    {
        if (ray.IsActive())
        {
            if (UIManager.Instance.panelDict.ContainsKey(UIConst.RegistrationMenu))
            {
                ray.enabled = false;
            }
        }
        else
        {
            if (!UIManager.Instance.panelDict.ContainsKey(UIConst.RegistrationMenu))
            {
                ray.enabled = true;
            }
        }
    }

    private void OnDestroy()
    {
        ClosePanel();
    }

    /// <summary>
    /// 管理继续游戏按钮是否可用
    /// </summary>
    private void ContinueGameBtnControll()
    {
        if (files.Length == 0)
        {
            continueGameBtn.interactable = false;
            continueGameBtn.transform.GetChild(0).GetComponent<Text>().color = new Color(0, 0, 0, 50f / 255f);
        }
        else
        {
            continueGameBtn.interactable = true;
            continueGameBtn.transform.GetChild(0).GetComponent<Text>().color = Color.black;
        }
    }

    /// <summary>
    /// 新游戏
    /// </summary>
    public void CreateNewGame()
    {
        // 打开建档界面
        UIManager.Instance.OpenPanel(UIConst.RegistrationMenu);
    }

    /// <summary>
    /// 继续游戏
    /// </summary>
    public void ContinueExistGame()
    {
        // 显示存档列表
        UserData userData = LocalConfig.LoadUserData("DDV");
        GameManager.Instance.InitUserData(userData);
    }

    /// <summary>
    /// 设置
    /// </summary>
    public void SettingsOpen()
    {
        // 打开设置界面
    }

    /// <summary>
    /// 退出游戏
    /// </summary>
    public void QuitGame()
    {
        Application.Quit();
    }
}
