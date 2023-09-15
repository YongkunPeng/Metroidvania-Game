using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using Cinemachine;
using DG.Tweening;

public class BossManager : MonoBehaviour
{
    [SerializeField] private GameObject bossPre; // boss预制体
    [SerializeField] private GameObject boss; // 生成的boss
    private SamuraiFSMAI fsm;
    [SerializeField] private GameObject bossBody; // boss尸体
    [SerializeField] private Vector3 bossBornPositon; // boss出生位置
    [SerializeField] private GameObject airWall; // 墙体
    [SerializeField] private WallController wallController;
    [SerializeField] private GameObject playerLight; // 玩家灯光
    [SerializeField] private Light2D globalLight; // 全局灯光
    [SerializeField] private bool lightChange; // 协程是否已被启用过
    [SerializeField] private Color redColor; // 环境光

    [SerializeField] private CinemachineVirtualCamera virtualCamera1; // 只看向玩家
    [SerializeField] private CinemachineVirtualCamera virtualCamera2; // 看向玩家和BOSS
    [SerializeField] private CinemachineTargetGroup targetGroup;
    [SerializeField] private Transform playerCinemaTarget;

    private BoxCollider2D col;

    private void Awake()
    {
        col = GetComponent<BoxCollider2D>();
        bossBornPositon = new Vector3(24.6f, 0.04f, 0.0f);
        redColor = globalLight.color;
        wallController = airWall.GetComponent<WallController>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (!GameManager.Instance.userData.killBoss)
            { // 未击败boss，生成boss，打开墙体限制区域
                boss = GameObject.Instantiate(bossPre, bossBornPositon, Quaternion.identity);
                fsm =boss.GetComponent<SamuraiFSMAI>();
                airWall.SetActive(true);
                targetGroup.AddMember(boss.transform, 1, 0);
                targetGroup.AddMember(playerCinemaTarget, 2, 0);
                virtualCamera2.Priority = 10;
                virtualCamera1.Priority = 9;
                UIManager.Instance.OpenPanel(UIConst.BossInfo);
            }
            else
            { // 击败boss，生成尸体
                GameObject.Instantiate(bossBody, bossBornPositon, Quaternion.identity);
            }
            col.enabled = false;
        }
    }

    private void FixedUpdate()
    {
        if (boss == null || fsm.samuraiBlackboard.health <= 0.0f)
        { // 先前已击败boss或当前boss死亡，撤去墙体
            virtualCamera1.Priority = 10;
            virtualCamera2.Priority = 9;
            if (playerLight.activeInHierarchy && globalLight.intensity < 0.8f)
            {
                StartCoroutine(LightChangeDistinguish());
            }
        }

        if (!lightChange && boss != null)
        { // 存在boss，未调用过协程
            if (fsm.samuraiBlackboard.health <= fsm.samuraiBlackboard.maxHealth / 2)
            { // boss生命小于50%
                StartCoroutine(LightChangeDim());
            }
        }
    }

    // 光线变暗
    IEnumerator LightChangeDim()
    {
        lightChange = true;
        playerLight.SetActive(true);
        while (globalLight.intensity >= 0.6f || globalLight.color.g >= 190f / 255f || globalLight.color.b >= 190f / 255f)
        {
            globalLight.intensity = Mathf.Lerp(globalLight.intensity, 0.55f, 0.01f); // 修改亮度
            redColor.g = Mathf.Lerp(redColor.g, 180f / 255f, 0.01f);
            redColor.b = Mathf.Lerp(redColor.b, 180f / 255f, 0.01f);
            globalLight.color = redColor;
            yield return null;
        }
    }

    // 光线恢复
    IEnumerator LightChangeDistinguish()
    {
        playerLight.SetActive(false);
        while (globalLight.intensity < 0.8f || globalLight.color.g < 1 || globalLight.color.b < 1)
        {
            globalLight.intensity = Mathf.Lerp(globalLight.intensity, 0.85f, 0.01f); // 修改亮度
            redColor.g = Mathf.Lerp(redColor.g, 1, 0.01f);
            redColor.b = Mathf.Lerp(redColor.b, 1, 0.01f);
            globalLight.color = redColor;
            yield return null;
        }
    }
}
