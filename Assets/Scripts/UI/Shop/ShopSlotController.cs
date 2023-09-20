using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ShopSlotController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Items slotItem; // 槽物品
    [SerializeField] private Button buyBtn; // 购买按钮
    [SerializeField] private ItemTipUI itemTipUI; // 物品信息
    private Vector2 mousePos;

    private void Awake()
    {
        itemTipUI = transform.parent.parent.parent.GetChild(5).GetComponent<ItemTipUI>();
        buyBtn = GetComponent<Button>();
    }

    private void Start()
    {
        // 添加购买监听
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
    /// 购买当前物品
    /// </summary>
    public void BuyItem()
    {
        bool isBuy = GameObject.FindObjectOfType<PlayerControll>().ChangeCoinCnt(-slotItem.buyPrice);
        if (isBuy)
        {
            Debug.Log("买入：" + slotItem.buyPrice);
            TipsBoxManager.Instance.ShowTipsBox("买入物品：" + slotItem.itemName, 1f);
            AudioSourceManager.Instance.PlaySound(GlobalAudioClips.ShopSound);
            GameManager.Instance.AddItem(slotItem, 1);
            transform.parent.parent.parent.parent.parent.GetComponent<ShopMenu>().ChangeShouldUpdate();
        }
        else if (!isBuy)
        {
            TipsBoxManager.Instance.ShowTipsBox("金币不足", 1f);
        }
    }
}
