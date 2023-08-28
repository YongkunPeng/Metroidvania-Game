using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionDelegate : MonoBehaviour
{
    public Mission mission;

    private void Awake()
    { // �ж������б������޸����񣬷�ֹ�л��������ظ���ȡ
        foreach (Mission mission in GameManager.Instance.missionList)
        {
            if (mission.missionName == this.mission.missionName)
            {
                this.mission = mission;
            }
        }
    }

    /// <summary>
    /// ί������
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
