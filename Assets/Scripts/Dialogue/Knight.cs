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
            // 使NPC对话时始终面朝玩家
            if (transform.position.x - player.position.x > 0)
            {
                transform.localScale = new Vector3(-1, 1, 1);
                keyboardInfo.transform.localScale = new Vector3(-1, 1, 1); // 防止提示跟随父物体翻转
            }
            else
            {
                transform.localScale = Vector3.one;
                keyboardInfo.transform.localScale = Vector3.one; // 防止提示跟随父物体翻转
            }

            // 激活失活相应物体
            keyboardInfo.SetActive(false);
            dialog.SetActive(true);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !dialog.activeInHierarchy)
        { // 如果玩家接近且未交互
            player = collision.transform;
            isClose = true;
            keyboardInfo.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        { // 玩家远离
            isClose = false;
            keyboardInfo.SetActive(false);
            dialog.SetActive(false);
        }
    }
}
