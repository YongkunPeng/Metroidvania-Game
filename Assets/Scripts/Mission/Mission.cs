using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Mission
{
    public enum MissionType
    { // 任务类型
        Collect, // 收集 
        KillEnemy, // 击杀
        KillMushroom, // 击杀蘑菇人
        KillGoblin, // 击杀哥布林
    }

    public enum MissionStatus
    { // 任务状态
        Unaccepted, // 未接受
        Accepted, // 接受
        Completed, // 完成
        Rewarded, // 已交付奖励
    }

    public string publisherName; // 发布者
    public string missionName; // 任务名
    public string missionDes; // 任务描述
    public int goalCnt; // 目标进度
    public int currCnt; // 当前进度
    public MissionType missionType; // 任务类型
    public MissionStatus missionStatus; // 任务状态
    public string collectItemName; // 收集物品名

    public Dictionary<string, int> itemsReward; // 物品奖励(物品，数量)
    public int goldReward; // 金币奖励

    // 击杀任务完成
    public void UpdateMissionData(int cnt)
    {
        currCnt += cnt;
        if (currCnt >= goalCnt)
        {
            TipsBoxManager.Instance.ShowTipsBox("任务：<color=red>" + missionName + "</color>已完成", 3f);
            missionStatus = MissionStatus.Completed;
        }
    }

    // 收集任务完成
    public void UpdateMissionComplete()
    {
        missionStatus = MissionStatus.Completed;
    }

    // 收集任务完成但未领奖，期间变为未到标
    public void UpdateMissionAccept()
    {
        missionStatus = MissionStatus.Accepted;
    }
}
