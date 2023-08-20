using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SlotController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public int slotID;
    public Items slotItem;
    [SerializeField] private ItemTipUI itemTipUI;
    private Vector2 mousePos;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.V))
        {
            if (slotItem == Resources.Load<Items>("ItemData/Item_Arrow"))
            {
                GameManager.Instance.RemoveItem(Resources.Load<Items>("ItemData/Item_Arrow"));
                itemTipUI.ShowItemTip();
                itemTipUI.UpdateItemTip(slotItem.itemName, slotItem.itemDes, slotItem.sellPrice);
            }
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
        transform.GetChild(2).gameObject.SetActive(true); // 选中图片显示

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
        transform.GetChild(2).gameObject.SetActive(false); // 选中图片关闭

        itemTipUI.HideItemTip();
    }
}
