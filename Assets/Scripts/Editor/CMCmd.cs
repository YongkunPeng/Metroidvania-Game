using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class CMCmd
{
    /// <summary>
    /// 保存玩家JSON数据
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
        Debug.Log("保存完成!");
    }

    /// <summary>
    /// 加载玩家JSON数据
    /// </summary>
    [MenuItem("CMCmd/LoadUserData")]
    public static void LoadUserData()
    {
        string name = "SuperDDV";
        UserData data = LocalConfig.LoadUserData(name);

        Debug.Log("玩家名：" + data.username);
        Debug.Log("生命值：" + data.health);
        Debug.Log("金币数：" + data.coinCnt);

        foreach (KeyValuePair<Items, int> item in data.items)
        {
            Debug.Log("物品名：" + item.Key.itemName);
            Debug.Log("物品描述：" + item.Key.itemDes);
            Debug.Log("物品数量:" + item.Value);
        }

        Debug.Log("加载完成!");
    }

    /// <summary>
    /// 场景所存在UI字典调试
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
    /// 背包字典调试
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
    /// 任务列表调试
    /// </summary>
    [MenuItem("CMCmd/CheckMissionList")]
    public static void CheckMissionList()
    {
        foreach(Mission mission in GameManager.Instance.missionList)
        {
            Debug.Log("任务名：" + mission.missionName);
            Debug.Log("任务状态：" + mission.missionStatus);
        }
    }

    [MenuItem("CMCmd/CompleteMission")]
    /// <summary>
    /// 完成任务
    /// </summary>
    public static void CompleteMission()
    {
        foreach (Mission mission in GameManager.Instance.missionList)
        {
            if (mission.missionName == "进入森林深处的资格")
            {
                mission.missionStatus = Mission.MissionStatus.Completed;
            }
        }
    }
}
