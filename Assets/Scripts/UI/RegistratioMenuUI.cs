using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RegistratioMenuUI : BasePanel
{
    [SerializeField] private InputField inputtext;
    private readonly string usernamePattern = @"^[A-Za-z0-9_]{3,8}$"; // 正则表达式约束输入

    private void Awake()
    {
        OpenPanel(UIConst.RegistrationMenu);

        inputtext = transform.GetChild(1).GetChild(0).GetComponent<InputField>();
    }

    private void OnDestroy()
    {
        ClosePanel();
    }

    /// <summary>
    /// 创建新存档
    /// </summary>
    public void StartNewUserData()
    {
        AudioSourceManager.Instance.PlaySound(GlobalAudioClips.ClickSound);

        DirectoryInfo directoryInfo = new DirectoryInfo(Application.persistentDataPath + "/users");
        FileInfo[] files = directoryInfo.GetFiles("*.json")
            .Where(file => file.Extension.ToLower() == ".json")
            .ToArray();

        string username = inputtext.text;
        if (ValidateUsername(username) && !File.Exists(Application.persistentDataPath + "/users/" + username + ".json") && files.Length < 2)
        {
            // 创建新的玩家数据，保存到本地并将其中数据交予GameManager后，进入游戏场景
            UserData userData = new UserData();
            userData.username = username;
            userData.health = 100;
            userData.coinCnt = 0;
            userData.sceneID = 1;
            userData.checkPointID = 1;
            userData.missionList = new List<Mission>();
            userData.slotDict = new Dictionary<int, string>();
            userData.itemsDict = new Dictionary<string, int>();

            #region 添加初始物品
            userData.slotDict.Add(0, ItemsConst.Arrow);
            userData.itemsDict.Add(ItemsConst.Arrow, 5);
            userData.slotDict.Add(1, ItemsConst.LifePotion);
            userData.itemsDict.Add(ItemsConst.LifePotion, 3);
            #endregion

            LocalConfig.SaveUserData(userData);
            GameManager.Instance.userData = userData;
            GameManager.Instance.slotDict = userData.slotDict;
            GameManager.Instance.itemsDict = userData.itemsDict;
            GameManager.Instance.missionList = userData.missionList;
            GameManager.Instance.username = userData.username;
            SceneLoadManager.Instance.LoadLevelByIndexWithSlider(1);
            ClosePanel();
        }
        else if (!ValidateUsername(username))
        { // 玩家名不合法
            transform.GetChild(6).gameObject.SetActive(true);
        }
        else if (File.Exists(Application.persistentDataPath + "/users/" + username + ".json"))
        { // 玩家名重复
            transform.GetChild(7).gameObject.SetActive(true);
        }
        else if (files.Length >= 2)
        { // 超出存档数
            transform.GetChild(8).gameObject.SetActive(true);
        }
    }

    public void CloseRegistrationMenu()
    {
        AudioSourceManager.Instance.PlaySound(GlobalAudioClips.ClickSound);
        Destroy(gameObject);
    }

    /// <summary>
    /// 检查玩家名
    /// </summary>
    /// <param name="username">玩家名</param>
    /// <returns></returns>
    public bool ValidateUsername(string username)
    {
        return Regex.IsMatch(username, usernamePattern);
    }
}
