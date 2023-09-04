using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class OptionPanel : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private bool isInsidePanel;
    [SerializeField] private GameObject useBtn;
    [SerializeField] private GameObject dropBtn;
    public static Items currItem;

    private void Awake()
    {
        useBtn = transform.GetChild(0).gameObject;
        dropBtn = transform.GetChild(1).gameObject;
    }

    private void OnEnable()
    {
        if (currItem != null)
        { // 根据当前物品是否可用控制按钮交互
            if (!currItem.canUse)
            { // 当前道具无法使用
                useBtn.GetComponent<Button>().interactable = false;
                useBtn.transform.GetChild(0).GetComponent<Text>().color = Color.gray;
            }
            else
            { // 当前道具可用
                useBtn.GetComponent<Button>().interactable = true;
                useBtn.transform.GetChild(0).GetComponent<Text>().color = Color.white;
            }
        }
    }

    private void Update()
    {
        if (Input.GetMouseButton(0) && !isInsidePanel)
        { // 在选择界面外点击其它位置
            HideOptionPanel();
        }
    }

    /// <summary>
    /// 使用物品
    /// </summary>
    public void UseItem()
    {
        if (currItem.itemName == ItemsConst.LifePotion)
        { // 生命药剂回复生命
            GameManager.Instance.RemoveItem(currItem);
            GameObject.FindObjectOfType<PlayerControll>().HealLife(25f);
        }
        HideOptionPanel();
    }

    /// <summary>
    /// 丢弃物品
    /// </summary>
    public void DropItem()
    {
        GameManager.Instance.RemoveItem(currItem);
        HideOptionPanel();
    }

    /// <summary>
    /// 隐藏
    /// </summary>
    private void HideOptionPanel()
    {
        currItem = null;
        gameObject.SetActive(false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isInsidePanel = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isInsidePanel = false;
    }
}
