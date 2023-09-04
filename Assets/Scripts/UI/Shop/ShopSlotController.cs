using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ShopSlotController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Items slotItem; // ����Ʒ
    [SerializeField] private Button buyBtn; // ����ť
    [SerializeField] private ItemTipUI itemTipUI; // ��Ʒ��Ϣ
    private Vector2 mousePos;

    private void Awake()
    {
        itemTipUI = transform.parent.parent.parent.GetChild(5).GetComponent<ItemTipUI>();
        buyBtn = GetComponent<Button>();
    }

    private void OnEnable()
    {
        // ��ӹ������
        buyBtn.onClick.AddListener(BuyItem);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        itemTipUI.ShowItemTip();
        itemTipUI.UpdateItemTip(slotItem.itemName, slotItem.itemDes, slotItem.buyPrice);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(GameObject.FindGameObjectWithTag("Shop").transform as RectTransform, Input.mousePosition, null, out mousePos);
        itemTipUI.SetTipPosition(mousePos);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        itemTipUI.HideItemTip();
    }

    /// <summary>
    /// ����ǰ��Ʒ
    /// </summary>
    public void BuyItem()
    {
        bool isBuy = GameObject.FindObjectOfType<PlayerControll>().ChangeCoinCnt(-slotItem.buyPrice);
        if (isBuy)
        {
            Debug.Log("���룺" + slotItem.buyPrice);
            AudioSourceManager.Instance.PlaySound(GlobalAudioClips.ShopSound);
            GameManager.Instance.AddItem(slotItem);
            transform.parent.parent.parent.parent.parent.GetComponent<ShopMenu>().ChangeShouldUpdate();
        }
    }
}
