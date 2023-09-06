using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionDelegate : MonoBehaviour
{
    public Mission mission;
    [SerializeField] private List<string> itemsRewardName = new List<string>(); // ������Ʒ��
    [SerializeField] private List<int> itemsRewardCnt = new List<int>(); // ��������

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

    private void OnEnable()
    {
        for (int i = 0; i < itemsRewardName.Count; i++)
        { // �½��ֵ䣬�������б����
            mission.itemsReward = new Dictionary<string, int>();
            mission.itemsReward.Add(itemsRewardName[i], itemsRewardCnt[i]);
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
