using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RegistratioMenuUI : BasePanel
{
    [SerializeField] private InputField inputtext;
    private readonly string usernamePattern = @"^[A-Za-z0-9_]{3,8}$"; // ������ʽԼ������

    private void Awake()
    {
        OpenPanel(UIConst.RegistrationMenu);

        inputtext = transform.GetChild(1).GetChild(0).GetComponent<InputField>();
    }

    private void OnDestroy()
    {
        ClosePanel();
    }

    public void StartNewUserData()
    {
        string username = inputtext.text;
        if (ValidateUsername(username))
        {
            // �����µ�������ݣ����浽���ز����������ݽ���GameManager�󣬽�����Ϸ����
            UserData userData = new UserData();
            userData.username = username;
            userData.health = 100;
            userData.coinCnt = 0;
            userData.missionList = new List<Mission>();
            userData.slotDict = new Dictionary<int, string>();
            userData.itemsDict = new Dictionary<string, int>();
            userData.slotDict.Add(0, ItemsConst.Arrow);
            userData.itemsDict.Add(ItemsConst.Arrow, 5);
            LocalConfig.SaveUserData(userData);
            GameManager.Instance.userData = userData;
            GameManager.Instance.slotDict = userData.slotDict;
            GameManager.Instance.itemsDict = userData.itemsDict;
            GameManager.Instance.missionList = userData.missionList;
            GameManager.Instance.username = userData.username;
            SceneManager.LoadScene(1);
        }
        else
        {
            transform.GetChild(6).gameObject.SetActive(true);
        }
    }

    public void CloseRegistrationMenu()
    {
        Destroy(gameObject);
    }

    public bool ValidateUsername(string username)
    {
        return Regex.IsMatch(username, usernamePattern);
    }
}
