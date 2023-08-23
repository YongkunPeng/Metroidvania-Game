using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SlotController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IDropHandler
{
    public int slotID;
    public Items slotItem;
    [SerializeField] private ItemTipUI itemTipUI;
    private Vector2 mousePos;

    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.V))
        //{
        //    if (slotItem == Resources.Load<Items>("ItemData/Item_Arrow"))
        //    {
        //        GameManager.Instance.RemoveItem(Resources.Load<Items>("ItemData/Item_Arrow"));
        //        itemTipUI.ShowItemTip();
        //        itemTipUI.UpdateItemTip(slotItem.itemName, slotItem.itemDes, slotItem.sellPrice);
        //    }
        //}
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
        int index = 0;
        foreach (KeyValuePair<Items, int> keyValuePair in GameManager.Instance.itemsDict)
        {
            if (slotID == index)
            {
                slotItem = keyValuePair.Key;
            }
            index++;
        }
        return slotItem;
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
        }
        else
        { // 当前槽不为空
            Transform temp = ItemDrag.startParent; // 记录该物体原来的槽
            item.transform.SetParent(temp); // 该槽的原物体置换到拖动物体原来的槽
            ItemDrag.itemBeginDragged.transform.SetParent(transform); // 被拖动的物体置换到新的槽
        }
    }
}
