using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TipsBoxManager : MonoBehaviour
{
    private static TipsBoxManager _Instance;

    public static TipsBoxManager Instance
    {
        get
        {
            if (_Instance == null)
            {
                _Instance = GameObject.FindObjectOfType<TipsBoxManager>();
                if( _Instance == null)
                {
                    GameObject gameObject = new GameObject("TipsBoxManager");
                    _Instance = gameObject.AddComponent<TipsBoxManager>();
                }
            }
            return _Instance;
        }
    }

    private void Awake()
    {
        if (_Instance != null && _Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// ��ʾ����ʾ
    /// </summary>
    /// <param name="text">�ı�����</param>
    /// <param name="existTime">����ʱ��</param>
    public void ShowTipsBox(string text, float existTime)
    {
        if (UIManager.Instance.panelDict.ContainsKey(UIConst.TipsBox))
        {
            UIManager.Instance.panelDict.Remove(UIConst.TipsBox);
        }
        TipsBoxUI tipsBox = UIManager.Instance.OpenPanel(UIConst.TipsBox) as TipsBoxUI;
        tipsBox.SetTipText(text, existTime);
    }
}