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
        if (Input.GetKeyDown(KeyCode.I) 
            && !UIManager.Instance.panelDict.ContainsKey(UIConst.PlayerBag) 
            && !UIManager.Instance.panelDict.ContainsKey(UIConst.PlayerMission)
            && !UIManager.Instance.panelDict.ContainsKey(UIConst.Settings))
        { // ����I�һ�����û�б���������������
            UIManager.Instance.OpenPanel(UIConst.PlayerBag);
        }
        if (Input.GetKeyDown(KeyCode.M) 
            && !UIManager.Instance.panelDict.ContainsKey(UIConst.PlayerBag) 
            && !UIManager.Instance.panelDict.ContainsKey(UIConst.PlayerMission)
            && !UIManager.Instance.panelDict.ContainsKey(UIConst.Settings))
        { // ����M�һ�����û�б���������������
            UIManager.Instance.OpenPanel(UIConst.PlayerMission);
        }
        if (Input.GetKeyDown(KeyCode.Escape)
            && !UIManager.Instance.panelDict.ContainsKey(UIConst.PlayerBag) 
            && !UIManager.Instance.panelDict.ContainsKey(UIConst.PlayerMission)
            && !UIManager.Instance.panelDict.ContainsKey(UIConst.Settings))
        {
            UIManager.Instance.OpenPanel(UIConst.Settings);
        }
    }
}
