using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Exit : MonoBehaviour
{
    [SerializeField] private string enterSceneName; // 要进入传送点的名字
    [SerializeField] private string currSceneName; // 当前传送点的名字
    [SerializeField] private int enterSceneIndex; // 要进入场景的索引
    [SerializeField] private int currSceneIndex; //  当前场景的索引
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
