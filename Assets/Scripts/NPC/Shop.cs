using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shop : MonoBehaviour
{
    [SerializeField] private GameObject keyboardInfo;
    [SerializeField] private bool isClose;

    private void Awake()
    {
        keyboardInfo = transform.GetChild(0).gameObject;
    }

    private void Update()
    {
        if (isClose && Input.GetKeyDown(KeyCode.E) && !GameManager.Instance.isPaused)
        { // ���̵����
            UIManager.Instance.OpenPanel(UIConst.Shop);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        isClose = true;
        keyboardInfo.SetActive(true);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        isClose = false;
        keyboardInfo.SetActive(false);
    }
}
