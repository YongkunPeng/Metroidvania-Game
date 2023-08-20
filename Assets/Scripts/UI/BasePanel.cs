using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasePanel : MonoBehaviour
{
    [SerializeField] protected bool isRemove = false; // �����Ƿ񱻹ر�
    [SerializeField] new protected string name; // ʹ��new�������أ����غ���һ��Ϊpublic new�����ر���һ��Ϊnew public������Ч����ͬ

    public virtual void OpenPanel(string name) // ��ָ��UI
    {
        this.name = name;
        gameObject.SetActive(true);
    }

    public virtual void ClosePanel() // �رյ�ǰUI
    {
        isRemove = true;
        gameObject.SetActive(false);
        Destroy(gameObject);

        // �ӻ������Ƴ����棬��ʾδ��
        if (UIManager.Instance.panelDict.ContainsKey(name))
        {
            UIManager.Instance.panelDict.Remove(name);
        }
    }
}
