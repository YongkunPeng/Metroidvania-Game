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
        { // ���ݵ�ǰ��Ʒ�Ƿ���ÿ��ư�ť����
            if (!currItem.canUse)
            { // ��ǰ�����޷�ʹ��
                useBtn.GetComponent<Button>().interactable = false;
                useBtn.transform.GetChild(0).GetComponent<Text>().color = Color.gray;
            }
            else
            { // ��ǰ���߿���
                useBtn.GetComponent<Button>().interactable = true;
                useBtn.transform.GetChild(0).GetComponent<Text>().color = Color.white;
            }
        }
    }

    private void Update()
    {
        if (Input.GetMouseButton(0) && !isInsidePanel)
        { // ��ѡ�������������λ��
            HideOptionPanel();
        }
    }

    /// <summary>
    /// ʹ����Ʒ
    /// </summary>
    public void UseItem()
    {
        if (currItem.itemName == ItemsConst.LifePotion)
        { // ����ҩ���ظ�����
            GameManager.Instance.RemoveItem(currItem);
            GameObject.FindObjectOfType<PlayerControll>().HealLife(25f);
        }
        HideOptionPanel();
    }

    /// <summary>
    /// ������Ʒ
    /// </summary>
    public void DropItem()
    {
        GameManager.Instance.RemoveItem(currItem);
        HideOptionPanel();
    }

    /// <summary>
    /// ����
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
