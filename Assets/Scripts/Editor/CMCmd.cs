using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class CMCmd
{
    /// <summary>
    /// �������JSON����
    /// </summary>
    [MenuItem("CMCmd/SaveUserData")]
    public static void SaveUserData()
    {
        Dictionary<Items, int> items = new Dictionary<Items, int>()
        {
            {Resources.Load<Items>("ItemData/Item_Arrow"), 5},
            {Resources.Load<Items>("ItemData/Item_Mushroom_Chunks"), 3}
        };
        UserData data = new UserData();
        data.username = "SuperDDV";
        data.health = 100f;
        data.arrowCnt = 5;
        data.coinCnt = 300;
        data.items = items;
        LocalConfig.SaveUserData(data);
        Debug.Log("�������!");
    }

    /// <summary>
    /// �������JSON����
    /// </summary>
    [MenuItem("CMCmd/LoadUserData")]
    public static void LoadUserData()
    {
        string name = "SuperDDV";
        UserData data = LocalConfig.LoadUserData(name);

        Debug.Log("�������" + data.username);
        Debug.Log("����ֵ��" + data.health);
        Debug.Log("�������" + data.coinCnt);

        foreach (KeyValuePair<Items, int> item in data.items)
        {
            Debug.Log("��Ʒ����" + item.Key.itemName);
            Debug.Log("��Ʒ������" + item.Key.itemDes);
            Debug.Log("��Ʒ����:" + item.Value);
        }

        Debug.Log("�������!");
    }

    /// <summary>
    /// ����������UI�ֵ����
    /// </summary>
    [MenuItem("CMCmd/CheckUIPanelDic")]
    public static void CheckUIPanelDic()
    {
        foreach (KeyValuePair<string, BasePanel> dic in UIManager.Instance.panelDict)
        {
            Debug.Log(dic.Key + " - " + dic.Value);
        }
    }

    /// <summary>
    /// �����ֵ����
    /// </summary>
    [MenuItem("CMCmd/CheckBagDic")]
    public static void CheckBagDic()
    {
        foreach (KeyValuePair<Items, int> dic in GameManager.Instance.itemsDict)
        {
            Debug.Log(dic.Key.itemName + " - " + dic.Value);
        }
    }

    /// <summary>
    /// �����б����
    /// </summary>
    [MenuItem("CMCmd/CheckMissionList")]
    public static void CheckMissionList()
    {
        foreach(Mission mission in GameManager.Instance.missionList)
        {
            Debug.Log("��������" + mission.missionName);
            Debug.Log("����״̬��" + mission.missionStatus);
        }
    }

    [MenuItem("CMCmd/CompleteMission")]
    /// <summary>
    /// �������
    /// </summary>
    public static void CompleteMission()
    {
        foreach (Mission mission in GameManager.Instance.missionList)
        {
            if (mission.missionName == "����ɭ������ʸ�")
            {
                mission.missionStatus = Mission.MissionStatus.Completed;
            }
        }
    }
}
