using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knight : MonoBehaviour
{
    [SerializeField] private GameObject keyboardInfo;
    [SerializeField] private bool isClose;
    [SerializeField] private Transform player;
    [SerializeField] private GameObject dialog;

    private void Awake()
    {
        keyboardInfo = transform.GetChild(0).gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if (isClose && Input.GetKeyDown(KeyCode.E) && !GameManager.Instance.isPaused)
        {
            // ʹNPC�Ի�ʱʼ���泯���
            if (transform.position.x - player.position.x > 0)
            {
                transform.localScale = new Vector3(-1, 1, 1);
                keyboardInfo.transform.localScale = new Vector3(-1, 1, 1); // ��ֹ��ʾ���游���巭ת
            }
            else
            {
                transform.localScale = Vector3.one;
                keyboardInfo.transform.localScale = Vector3.one; // ��ֹ��ʾ���游���巭ת
            }

            // ����ʧ����Ӧ����
            keyboardInfo.SetActive(false);
            dialog.SetActive(true);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !dialog.activeInHierarchy)
        { // �����ҽӽ���δ����
            player = collision.transform;
            isClose = true;
            keyboardInfo.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        { // ���Զ��
            isClose = false;
            keyboardInfo.SetActive(false);
            dialog.SetActive(false);
        }
    }
}
