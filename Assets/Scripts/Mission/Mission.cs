using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Mission
{
    public enum MissionType
    { // 任务类型
        Collect, // 收集 
        Kill, // 击杀
    }

    public enum MissionStatus
    { // 任务状态
        Unaccepted, // 未接受
        Accepted, // 接受
        Completed, // 完成
        Rewarded, // 已交付奖励
    }

    public string publisherName;
    public string missionName;
    public string missionDes;
    public MissionType missionType;
    public MissionStatus missionStatus;

    public Dictionary<Items, int> itemsReward; // 物品奖励(物品，数量)
    public int goldReward; // 金币奖励
}
