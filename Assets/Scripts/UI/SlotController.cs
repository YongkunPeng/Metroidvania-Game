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
    /// ���ݸñ������ӣ���ѯ�ֵ��иñ������ӵ���Ʒ
    /// </summary>
    /// <returns>�ñ�������������Ʒ��Items</returns>
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
        transform.GetChild(2).gameObject.SetActive(true); // ѡ��ͼƬ��ʾ

        if (GetItem() != null)
        { // �ñ������ڴ�����Ʒ
            itemTipUI.ShowItemTip();
            itemTipUI.UpdateItemTip(slotItem.itemName, slotItem.itemDes, slotItem.sellPrice);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(GameObject.FindGameObjectWithTag("Bag").transform as RectTransform, Input.mousePosition, null, out mousePos);
            itemTipUI.SetTipPosition(mousePos);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        transform.GetChild(2).gameObject.SetActive(false); // ѡ��ͼƬ�ر�

        itemTipUI.HideItemTip();
    }
}
