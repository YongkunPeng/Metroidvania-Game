using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SlotController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IDropHandler
{
    public int slotID;
    public Items slotItem;
    [SerializeField] private ItemTipUI itemTipUI;
    [SerializeField] private GameObject optionPanel;
    private Vector2 mousePos;

    private void Update()
    {
        GetItem(); // 持续获取当前槽内物品信息
    }

    public GameObject item
    {
        get
        {
            if (transform.childCount > 1)
            {
                return transform.GetChild(1).gameObject;
            }
            return null;
        }
    }

    /// <summary>
    /// 根据该背包格子，查询字典中该背包格子的物品
    /// </summary>
    /// <returns>该背包格子所含物品的Items</returns>
    private Items GetItem()
    {
        if (GameManager.Instance.slotDict.ContainsKey(slotID))
        {
            slotItem = GameManager.Instance.resourceDict[GameManager.Instance.slotDict[slotID]];
            return slotItem;
        }
        slotItem = null;
        return null;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        transform.GetChild(0).gameObject.SetActive(true); // 选中图片显示

        if (GetItem() != null)
        { // 该背包格内存在物品
            itemTipUI.ShowItemTip();
            itemTipUI.UpdateItemTip(slotItem.itemName, slotItem.itemDes, slotItem.sellPrice);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(GameObject.FindGameObjectWithTag("Bag").transform as RectTransform, Input.mousePosition, null, out mousePos);
            itemTipUI.SetTipPosition(mousePos);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        transform.GetChild(0).gameObject.SetActive(false); // 选中图片关闭

        itemTipUI.HideItemTip();
    }

    public void OnDrop(PointerEventData eventData)
    { // 此处的transform为松开键鼠时的所处的游戏对象
        if (!item)
        { // 当前槽为空
            ItemDrag.itemBeginDragged.transform.SetParent(transform);
            if (!GameManager.Instance.slotDict.ContainsKey(slotID))
            {
                GameManager.Instance.slotDict.Add(slotID, GameManager.Instance.slotDict[ItemDrag.startID]);
                GameManager.Instance.slotDict.Remove(ItemDrag.startID);
            }
        }
        else
        { // 当前槽不为空
            Transform temp = ItemDrag.startParent; // 记录该物体原来的槽
            item.transform.SetParent(temp); // 该槽的原物体置换到拖动物体原来的槽
            ItemDrag.itemBeginDragged.transform.SetParent(transform); // 被拖动的物体置换到新的槽

            // 字典值交换
            string tempStr = GameManager.Instance.slotDict[ItemDrag.startID];
            GameManager.Instance.slotDict[ItemDrag.startID] = GameManager.Instance.slotDict[slotID];
            GameManager.Instance.slotDict[slotID] = tempStr;
        }
    }

    public void ShowOptionPanel()
    {
        if (slotItem != null)
        {
            OptionPanel.currItem = slotItem;
            Vector2 mouse;
            optionPanel.SetActive(true);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(GameObject.FindGameObjectWithTag("Bag").transform as RectTransform, Input.mousePosition, null, out mouse);
            optionPanel.transform.localPosition = mouse;
        }
    }
}
