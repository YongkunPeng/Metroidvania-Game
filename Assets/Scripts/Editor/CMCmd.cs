using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using My.SaveSystem;

public class CMCmd
{
    /// <summary>
    /// 保存玩家JSON数据
    /// </summary>
    [MenuItem("CMCmd/SaveUserData")]
    public static void SaveUserData()
    {
        // AdditionalJSONConvert.AddAllConvert(); // 添加自定义序列化，防止序列化Vector3和Vector2报错

        Dictionary<int, string> slotDic = new Dictionary<int, string> ()
        {
            {0, "箭矢"},
            {1, "蘑菇碎块"},
        };

        Dictionary<string, int> itemDic = new Dictionary<string, int>()
        {
            {"箭矢", 5},
            {"蘑菇碎块", 3},
        };

        UserData data = new UserData();
        data.slotDict = slotDic;
        data.itemsDict = itemDic;
        data.username = "SuperDDV";
        data.health = 100;
        data.coinCnt = 50;
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

        foreach (KeyValuePair<int, string> pair in data.slotDict)
        {
            Debug.Log("物品槽" + pair.Key + "含有物品：" + pair.Value + "，数量为：" + data.itemsDict[pair.Value]);
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
        Debug.Log("物品槽字典");
        foreach (KeyValuePair<int, string> slot in GameManager.Instance.slotDict)
        {
            Debug.Log(slot.Key + " - " + slot.Value);
        }
        Debug.Log("物品槽字典");
        foreach (KeyValuePair<string, int> item in GameManager.Instance.itemsDict)
        {
            Debug.Log(item.Key + " - " + item.Value);
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
