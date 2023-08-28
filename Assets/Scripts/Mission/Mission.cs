using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Mission
{
    public enum MissionType
    { // ��������
        Collect, // �ռ� 
        KillEnemy, // ��ɱ
        KillMushroom, // ��ɱĢ����
        KillGoblin, // ��ɱ�粼��
    }

    public enum MissionStatus
    { // ����״̬
        Unaccepted, // δ����
        Accepted, // ����
        Completed, // ���
        Rewarded, // �ѽ�������
    }

    public string publisherName; // ������
    public string missionName; // ������
    public string missionDes; // ��������
    public int goalCnt; // Ŀ�����
    public int currCnt; // ��ǰ����
    public MissionType missionType; // ��������
    public MissionStatus missionStatus; // ����״̬
    public string collectItemName; // �ռ���Ʒ��

    public Dictionary<string, int> itemsReward; // ��Ʒ����(��Ʒ������)
    public int goldReward; // ��ҽ���

    // ��ɱ�������
    public void UpdateMissionData(int cnt)
    {
        currCnt += cnt;
        if (currCnt >= goalCnt)
        {
            missionStatus = MissionStatus.Completed;
        }
    }

    // �ռ��������
    public void UpdateMissionComplete()
    {
        missionStatus = MissionStatus.Completed;
    }

    // �ռ�������ɵ�δ�콱���ڼ��Ϊδ����
    public void UpdateMissionAccept()
    {
        missionStatus = MissionStatus.Accepted;
    }
}
