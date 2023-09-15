using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Exit : MonoBehaviour
{
    [SerializeField] private string enterSceneName; // Ҫ���봫�͵������
    [SerializeField] private string currSceneName; // ��ǰ���͵������
    [SerializeField] private int enterSceneIndex; // Ҫ���볡��������
    [SerializeField] private int currSceneIndex; //  ��ǰ����������
    [SerializeField] private GameObject player;
    [SerializeField] private Vector3 targetPos;

    private void Awake()
    {
        if (PlayerControll.startPoint == currSceneName)
        {
            player.transform.position = targetPos;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerControll.startPoint = enterSceneName;
            player.GetComponent<PlayerControll>().GetLifeCoinData(ref GameManager.Instance.userData.health, ref GameManager.Instance.userData.coinCnt);
            SceneLoadManager.Instance.LoadLevelByIndexWithSlider(enterSceneIndex);
        }
    }
}
