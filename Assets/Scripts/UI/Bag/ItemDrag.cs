using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemDrag : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    // ��¼�ϵõ�����
    public static GameObject itemBeginDragged;
    public static Transform startParent;
    public static int startID;


    [Header("ԭ��Ʒλ�ú�ԭ��Ʒ��ID")]
    [SerializeField] private Vector3 startPosition;

    [Header("��������в۵ĸ�����")]
    [SerializeField] private LayoutElement element;
    [SerializeField] private Transform outSlot; // ��¼���в۵ĸ�����

    private void Awake()
    {
        element = GetComponent<LayoutElement>();
        outSlot = transform.parent.parent;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        startParent = transform.parent;
        startPosition = transform.position;
        startID = startParent.GetComponent<SlotController>().slotID;

        itemBeginDragged = gameObject;
        GetComponent<CanvasGroup>().blocksRaycasts = false;
        startParent.GetComponent<GridLayoutGroup>().enabled = false; // ȡ�����񲼾ַ�ֹ�϶�ʱ��˸
        element.ignoreLayout = true;
        transform.SetParent(outSlot);
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = Input.mousePosition + new Vector3(0, 0, 1);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        element.ignoreLayout = false;
        startParent.GetComponent<GridLayoutGroup>().enabled = true; // �ָ����񲼾�����Ʒ����������
        itemBeginDragged = null;
        if (transform.parent == startParent || transform.parent == outSlot)
        {
            transform.SetParent(startParent);
            transform.position = startPosition;
        }
        GetComponent<CanvasGroup>().blocksRaycasts = true;
    }
}
