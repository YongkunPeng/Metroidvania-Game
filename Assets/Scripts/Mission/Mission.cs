using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Mission
{
    public enum MissionType
    { // ��������
        Collect, // �ռ� 
        Kill, // ��ɱ
    }

    public enum MissionStatus
    { // ����״̬
        Unaccepted, // δ����
        Accepted, // ����
        Completed, // ���
        Rewarded, // �ѽ�������
    }

    public string publisherName;
    public string missionName;
    public string missionDes;
    public MissionType missionType;
    public MissionStatus missionStatus;

    public Dictionary<Items, int> itemsReward; // ��Ʒ����(��Ʒ������)
    public int goldReward; // ��ҽ���
}
