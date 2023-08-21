using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionDelegate : MonoBehaviour
{
    public Mission mission;

    /// <summary>
    /// 委派任务
    /// </summary>
    public void Delegate()
    {
        if (mission.missionStatus == Mission.MissionStatus.Unaccepted)
        {
            GameManager.Instance.missionList.Add(mission);
            mission.missionStatus = Mission.MissionStatus.Accepted;
        }
        else
        {
            Debug.Log("任务：" + mission.missionName + " 以被接受或已完成！");
        }
    }
}
