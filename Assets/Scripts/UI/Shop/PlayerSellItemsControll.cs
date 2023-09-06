using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerSellItemsControll : MonoBehaviour
{
    [SerializeField] private Image itemImg; // ��ƷͼƬ
    [SerializeField] private Text itemName; // ��Ʒ��
    [SerializeField] private Text itemCnt; // ��Ʒ��
    [SerializeField] private Text itemSell; // ����
    [SerializeField] private Items item; //��Ʒ
    [SerializeField] private Button sellBtn; // ������ť

    private void Awake()
    {
        itemImg = transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Image>();
        itemName = transform.GetChild(1).GetComponent<Text>();
        itemCnt = transform.GetChild(2).GetComponent<Text>();
        itemSell = transform.GetChild(3).GetComponent<Text>();
        sellBtn = transform.GetChild(4).GetComponent<Button>();
    }

    private void OnEnable()
    {
        sellBtn.onClick.AddListener(SellItem);
    }

    /// <summary>
    /// ���¸ÿ�������Ʒ�����Ϣ
    /// </summary>
    /// <param name="itemName">��Ʒ��</param>
    /// <param name="itemCnt">��Ʒ����</param>
    public void UpdateItemData(string itemName, int itemCnt)
    {
        this.item = GameManager.Instance.resourceDict[itemName];
        this.itemImg.sprite = item.itemSprite;
        this.itemName.text = itemName;
        this.itemCnt.text = itemCnt.ToString();
        this.itemSell.text = "�ۼۣ�" + item.sellPrice;
    }

    /// <summary>
    /// ��������
    /// </summary>
    private void SellItem()
    {
        bool isSell = GameObject.FindObjectOfType<PlayerControll>().ChangeCoinCnt(this.item.sellPrice);
        if (isSell)
        {
            Debug.Log("������" + this.item.sellPrice);
            TipsBoxManager.Instance.ShowTipsBox("������Ʒ��" + this.item.itemName, 1f);
            AudioSourceManager.Instance.PlaySound(GlobalAudioClips.ShopSound);
            GameManager.Instance.RemoveItem(this.item);
            transform.parent.parent.parent.parent.GetComponent<ShopMenu>().ChangeShouldUpdate();
        }
    }
}
