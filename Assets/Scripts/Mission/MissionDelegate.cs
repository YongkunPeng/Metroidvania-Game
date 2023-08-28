using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionDelegate : MonoBehaviour
{
    public Mission mission;

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
