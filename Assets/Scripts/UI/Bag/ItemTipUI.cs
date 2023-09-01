using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemTipUI : MonoBehaviour
{
    [SerializeField] private Text nameText;
    [SerializeField] private Text descriptionText;
    [SerializeField] private Text soldPriceText;

    private void Awake()
    {
        nameText = transform.GetChild(0).GetComponent<Text>();
        descriptionText = transform.GetChild(1).GetComponent<Text>();
        soldPriceText = transform.GetChild(2).GetComponent<Text>();
    }

    /// <summary>
    /// ��ʾ��Ʒ��ϸ��ϢUI
    /// </summary>
    public void ShowItemTip()
    {
        gameObject.SetActive(true);
    }

    /// <summary>
    /// ������Ʒ��ϸ��ϢUI��ͬʱ�����Ϣ
    /// </summary>
    public void HideItemTip()
    {
        gameObject.SetActive(false);
        if (nameText != null || descriptionText != null || soldPriceText != null)
        {
            nameText.text = null;
            descriptionText.text = null;
            soldPriceText.text = null;
        }
    }

    /// <summary>
    /// ������Ʒ��ϸ��ϢUI�е�����
    /// </summary>
    /// <param name="name">��Ʒ��</param>
    /// <param name="desc">��Ʒ����</param>
    /// <param name="coin">������̵������۸�</param>
    public void UpdateItemTip(string name, string desc, int coin)
    {
        nameText.text = name;
        descriptionText.text = desc;
        soldPriceText.text = coin.ToString();
    }

    /// <summary>
    /// �޸���Ʒ��ϸ��ϢUI��λ�õ���괦
    /// </summary>
    /// <param name="pos">��ʾ��λ��</param>
    public void SetTipPosition(Vector2 pos)
    {
        transform.localPosition = pos;
    }
}
