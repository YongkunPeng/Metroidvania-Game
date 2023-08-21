using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasManager : MonoBehaviour
{
    private void Awake()
    {
        UIManager.Instance.OpenPanel(UIConst.PlayerInfo);
    }

    void Update()
    {
        OpenBag();
    }

    private void OpenBag()
    {
        if (Input.GetKeyDown(KeyCode.I) && !UIManager.Instance.panelDict.ContainsKey(UIConst.PlayerBag) && !UIManager.Instance.panelDict.ContainsKey(UIConst.PlayerMission))
        { // 按下I且缓存中没有背包界面和任务界面
            UIManager.Instance.OpenPanel(UIConst.PlayerBag);
        }
        if (Input.GetKeyDown(KeyCode.M) && !UIManager.Instance.panelDict.ContainsKey(UIConst.PlayerBag) && !UIManager.Instance.panelDict.ContainsKey(UIConst.PlayerMission))
        { // 按下M且缓存中没有背包界面和任务界面
            UIManager.Instance.OpenPanel(UIConst.PlayerMission);
        }
    }
}
