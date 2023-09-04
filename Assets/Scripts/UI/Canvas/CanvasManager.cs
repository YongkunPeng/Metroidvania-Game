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

    /// <summary>
    /// 打开各个界面(该界面未打开，且不存在其它界面时)
    /// </summary>
    private void OpenBag()
    {
        if (   !UIManager.Instance.panelDict.ContainsKey(UIConst.PlayerBag)
            && !UIManager.Instance.panelDict.ContainsKey(UIConst.PlayerMission)
            && !UIManager.Instance.panelDict.ContainsKey(UIConst.Settings)
            && !UIManager.Instance.panelDict.ContainsKey(UIConst.Shop))
        {
            if (Input.GetKeyDown(KeyCode.I))
            {
                UIManager.Instance.OpenPanel(UIConst.PlayerBag);
            }
            else if (Input.GetKeyDown(KeyCode.M))
            {
                UIManager.Instance.OpenPanel(UIConst.PlayerMission);
            }
            else if (Input.GetKeyDown(KeyCode.Escape))
            {
                UIManager.Instance.OpenPanel(UIConst.Settings);
            }
        }
    }
}
