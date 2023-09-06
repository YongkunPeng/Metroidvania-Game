using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionDelegate : MonoBehaviour
{
    public Mission mission;
    [SerializeField] private List<string> itemsRewardName = new List<string>(); // 奖励物品名
    [SerializeField] private List<int> itemsRewardCnt = new List<int>(); // 奖励数量

    private void Awake()
    { // 判断任务列表中有无该任务，防止切换场景后重复接取
        foreach (Mission mission in GameManager.Instance.missionList)
        {
            if (mission.missionName == this.mission.missionName)
            {
                this.mission = mission;
            }
        }
    }

    private void OnEnable()
    {
        for (int i = 0; i < itemsRewardName.Count; i++)
        { // 新建字典，并根据列表填充
            mission.itemsReward = new Dictionary<string, int>();
            mission.itemsReward.Add(itemsRewardName[i], itemsRewardCnt[i]);
        }
    }

    /// <summary>
    /// 委派任务
    /// </summary>
    public void Delegate()
    {
        if (mission.missionStatus == Mission.MissionStatus.Unaccepted)
        {
            GameManager.Instance.missionList.Add(mission);
            mission.UpdateMissionAccept();
        }
    }
}
