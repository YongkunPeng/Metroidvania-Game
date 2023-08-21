using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionDelegate : MonoBehaviour
{
    public Mission mission;

    /// <summary>
    /// ί������
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
            Debug.Log("����" + mission.missionName + " �Ա����ܻ�����ɣ�");
        }
    }
}
