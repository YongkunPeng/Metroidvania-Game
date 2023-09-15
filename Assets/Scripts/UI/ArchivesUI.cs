using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ArchivesUI : BasePanel
{
    [SerializeField] private GameObject userData1;
    [SerializeField] private GameObject userData2;
    [SerializeField] private string selectedData;
    private FileInfo[] files;

    private void Awake()
    {
        OpenPanel(UIConst.Archives);

        userData1 = transform.GetChild(0).GetChild(0).GetChild(0).gameObject;
        userData2 = transform.GetChild(0).GetChild(0).GetChild(1).gameObject;

        UpdateArchivesUI();
    }

    private void OnDestroy()
    {
        ClosePanel();
    }

    /// <summary>
    /// ¸üÐÂ´æµµ½çÃæÐÅÏ¢
    /// </summary>
    private void UpdateArchivesUI()
    {
        DirectoryInfo directoryInfo = new DirectoryInfo(Application.persistentDataPath + "/users");
        files = directoryInfo.GetFiles("*.json")
            .Where(file => file.Extension.ToLower() == ".json")
            .ToArray();

        if (files.Length == 1)
        {
            UserData data1 = LocalConfig.LoadUserData(files[0].Name.Replace(".json", ""));

            userData1.transform.GetChild(0).gameObject.SetActive(true);
            userData1.transform.GetChild(1).gameObject.SetActive(false);
            userData1.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = data1.username;
            userData1.transform.GetChild(0).GetChild(1).GetComponent<Text>().text = data1.coinCnt.ToString();
            userData1.GetComponent<Button>().interactable = true;

            userData2.transform.GetChild(0).gameObject.SetActive(false);
            userData2.transform.GetChild(1).gameObject.SetActive(true);
            userData2.GetComponent<Button>().interactable = false;
        }
        else if (files.Length >= 2)
        {
            UserData data1 = LocalConfig.LoadUserData(files[0].Name.Replace(".json", ""));
            UserData data2 = LocalConfig.LoadUserData(files[1].Name.Replace(".json", ""));

            userData1.transform.GetChild(0).gameObject.SetActive(true);
            userData1.transform.GetChild(1).gameObject.SetActive(false);
            userData1.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = data1.username;
            userData1.transform.GetChild(0).GetChild(1).GetComponent<Text>().text = data1.coinCnt.ToString();
            userData1.GetComponent<Button>().interactable = true;

            userData2.transform.GetChild(0).gameObject.SetActive(true);
            userData2.transform.GetChild(1).gameObject.SetActive(false);
            userData2.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = data2.username;
            userData2.transform.GetChild(0).GetChild(1).GetComponent<Text>().text = data2.coinCnt.ToString();
            userData2.GetComponent<Button>().interactable = true;
        }
        else
        {
            userData1.transform.GetChild(0).gameObject.SetActive(false);
            userData1.transform.GetChild(1).gameObject.SetActive(true);
            userData1.GetComponent<Button>().interactable = false;

            userData2.transform.GetChild(0).gameObject.SetActive(false);
            userData2.transform.GetChild(1).gameObject.SetActive(true);
            userData2.GetComponent<Button>().interactable = false;
        }
    }

    /// <summary>
    /// ¹Ø±Õ´æµµ½çÃæ
    /// </summary>
    public void CloseArchivesUI()
    {
        GameObject.FindGameObjectWithTag("MainMenu").GetComponent<MainMenuUI>().ContinueGameBtnControll();
        Destroy(gameObject);
    }

    /// <summary>
    /// ¼ÓÔØ´æµµ1
    /// </summary>
    public void LoadUserData1()
    {
        GameManager.Instance.shouldTransmit = true;
        selectedData = userData1.transform.GetChild(0).GetChild(0).GetComponent<Text>().text;
        UserData data = LocalConfig.LoadUserData(selectedData);
        GameManager.Instance.InitUserData(data, true);
    }

    /// <summary>
    /// ¼ÓÔØ´æµµ2
    /// </summary>
    public void LoadUserData2()
    {
        GameManager.Instance.shouldTransmit = true;
        selectedData = userData2.transform.GetChild(0).GetChild(0).GetComponent<Text>().text;
        UserData data = LocalConfig.LoadUserData(selectedData);
        GameManager.Instance.InitUserData(data, true);
    }

    /// <summary>
    /// É¾³ý´æµµ1
    /// </summary>
    public void DeleteUserData1()
    {
        TipsBoxManager.Instance.ShowTipsBox("ÒÑÉ¾³ý´æµµ£º<color=red>" + files[0].Name.Replace(".json", "") + "</color>", 2f);
        files[0].Delete();
        UpdateArchivesUI();
    }

    /// <summary>
    /// É¾³ý´æµµ2
    /// </summary>
    public void DeleteUserData2()
    {
        TipsBoxManager.Instance.ShowTipsBox("ÒÑÉ¾³ý´æµµ£º<color=red>" + files[1].Name.Replace(".json", "") + "</color>", 2f);
        files[1].Delete();
        UpdateArchivesUI();
    }
}
