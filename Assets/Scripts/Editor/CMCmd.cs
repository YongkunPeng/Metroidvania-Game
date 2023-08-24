using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using My.SaveSystem;

public class CMCmd
{
    /// <summary>
    /// �������JSON����
    /// </summary>
    [MenuItem("CMCmd/SaveUserData")]
    public static void SaveUserData()
    {
        // AdditionalJSONConvert.AddAllConvert(); // ����Զ������л�����ֹ���л�Vector3��Vector2����

        Dictionary<int, string> slotDic = new Dictionary<int, string> ()
        {
            {0, "��ʸ"},
            {1, "Ģ�����"},
        };

        Dictionary<string, int> itemDic = new Dictionary<string, int>()
        {
            {"��ʸ", 5},
            {"Ģ�����", 3},
        };

        UserData data = new UserData();
        data.slotDict = slotDic;
        data.itemsDict = itemDic;
        data.username = "SuperDDV";
        data.health = 100;
        data.coinCnt = 50;
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

        foreach (KeyValuePair<int, string> pair in data.slotDict)
        {
            Debug.Log("��Ʒ��" + pair.Key + "������Ʒ��" + pair.Value + "������Ϊ��" + data.itemsDict[pair.Value]);
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
        Debug.Log("��Ʒ���ֵ�");
        foreach (KeyValuePair<int, string> slot in GameManager.Instance.slotDict)
        {
            Debug.Log(slot.Key + " - " + slot.Value);
        }
        Debug.Log("��Ʒ���ֵ�");
        foreach (KeyValuePair<string, int> item in GameManager.Instance.itemsDict)
        {
            Debug.Log(item.Key + " - " + item.Value);
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
