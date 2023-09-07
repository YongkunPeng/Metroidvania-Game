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
        // 暂停游戏
        OpenPanel(UIConst.PlayerMission);
        GameManager.Instance.isPaused = true;
        Time.timeScale = 0f;

        grid = transform.GetChild(0).GetChild(0).GetChild(0);
        rect = grid.GetComponent<RectTransform>(); // 获取网格RectTransform
        rect.sizeDelta = new Vector2(rect.sizeDelta.x, 330 * GameManager.Instance.missionList.Count); // 根据任务数量调整网格大小

        // 生成任务预制体并填充对应信息
        missionSerie = Resources.Load<GameObject>("Prefabs/other/mission");
        foreach(Mission mission in GameManager.Instance.missionList)
        {
            GameObject missionChild = GameObject.Instantiate(missionSerie, grid);
            missionChild.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = mission.missionName; // 显示任务名
            missionChild.transform.GetChild(0).GetChild(1).GetComponent<Text>().text = mission.publisherName; // 显示发布者名
            missionChild.transform.GetChild(0).GetChild(2).GetComponent<Text>().text = mission.missionDes; // 显示任务描述
            missionChild.transform.GetChild(0).GetChild(4).GetComponent<Text>().text = mission.goldReward.ToString(); // 显示金币奖励数
            if (mission.missionStatus == Mission.MissionStatus.Completed)
            { // 若完成任务，显示对应图像
                missionChild.transform.GetChild(0).GetChild(5).GetChild(0).gameObject.SetActive(true);
            }
            if (mission.missionStatus == Mission.MissionStatus.Rewarded)
            { // 若已领取奖励，显示对应图标
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

    private void OnDestroy()
    {
        ClosePanel();
    }
}
