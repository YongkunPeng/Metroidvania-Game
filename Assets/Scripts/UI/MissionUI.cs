using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MissionUI : BasePanel
{
    [SerializeField] private Transform grid;
    [SerializeField] private RectTransform rect;
    [SerializeField] private GameObject missionSerie;

    private void Awake()
    {
        // ��ͣ��Ϸ
        OpenPanel(UIConst.PlayerMission);
        GameManager.Instance.isPaused = true;
        Time.timeScale = 0f;

        grid = transform.GetChild(0).GetChild(0).GetChild(0);
        rect = grid.GetComponent<RectTransform>(); // ��ȡ����RectTransform
        rect.sizeDelta = new Vector2(rect.sizeDelta.x, 330 * GameManager.Instance.missionList.Count); // ���������������������С

        // ��������Ԥ���岢����Ӧ��Ϣ
        missionSerie = Resources.Load<GameObject>("Prefabs/other/mission");
        foreach(Mission mission in GameManager.Instance.missionList)
        {
            GameObject missionChild = GameObject.Instantiate(missionSerie, grid);
            missionChild.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = mission.missionName; // ��ʾ������
            missionChild.transform.GetChild(0).GetChild(1).GetComponent<Text>().text = mission.publisherName; // ��ʾ��������
            missionChild.transform.GetChild(0).GetChild(2).GetComponent<Text>().text = mission.missionDes; // ��ʾ��������
            missionChild.transform.GetChild(0).GetChild(4).GetComponent<Text>().text = mission.goldReward.ToString(); // ��ʾ��ҽ�����
            if (mission.missionStatus == Mission.MissionStatus.Completed)
            { // �����������ʾ��Ӧͼ��
                missionChild.transform.GetChild(0).GetChild(5).GetChild(0).gameObject.SetActive(true);
            }
            if (mission.missionStatus == Mission.MissionStatus.Rewarded)
            { // ������ȡ��������ʾ��Ӧͼ��
                missionChild.transform.GetChild(0).GetChild(5).GetChild(1).gameObject.SetActive(true);
            }
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            CloseMissionList();
        }
    }

    private void CloseMissionList()
    {
        if (!isRemove)
        {
            ClosePanel();
            GameManager.Instance.isPaused = false;
            Time.timeScale = 1f;
        }
    }
}
