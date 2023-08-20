using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasePanel : MonoBehaviour
{
    [SerializeField] protected bool isRemove = false; // 界面是否被关闭
    [SerializeField] new protected string name; // 使用new用于隐藏，隐藏函数一般为public new，隐藏变量一般为new public，二者效果相同

    public virtual void OpenPanel(string name) // 打开指定UI
    {
        this.name = name;
        gameObject.SetActive(true);
    }

    public virtual void ClosePanel() // 关闭当前UI
    {
        isRemove = true;
        gameObject.SetActive(false);
        Destroy(gameObject);

        // 从缓存中移除界面，表示未打开
        if (UIManager.Instance.panelDict.ContainsKey(name))
        {
            UIManager.Instance.panelDict.Remove(name);
        }
    }
}
