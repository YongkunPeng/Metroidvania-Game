using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using Cinemachine;
using DG.Tweening;

public class BossManager : MonoBehaviour
{
    [SerializeField] private GameObject bossPre; // bossԤ����
    [SerializeField] private GameObject boss; // ���ɵ�boss
    private SamuraiFSMAI fsm;
    [SerializeField] private GameObject bossBody; // bossʬ��
    [SerializeField] private Vector3 bossBornPositon; // boss����λ��
    [SerializeField] private GameObject airWall; // ǽ��
    [SerializeField] private WallController wallController;
    [SerializeField] private GameObject playerLight; // ��ҵƹ�
    [SerializeField] private Light2D globalLight; // ȫ�ֵƹ�
    [SerializeField] private bool lightChange; // Э���Ƿ��ѱ����ù�
    [SerializeField] private Color redColor; // ������

    [SerializeField] private CinemachineVirtualCamera virtualCamera1; // ֻ�������
    [SerializeField] private CinemachineVirtualCamera virtualCamera2; // ������Һ�BOSS
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
            { // δ����boss������boss����ǽ����������
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
            { // ����boss������ʬ��
                GameObject.Instantiate(bossBody, bossBornPositon, Quaternion.identity);
            }
            col.enabled = false;
        }
    }

    private void FixedUpdate()
    {
        if (boss == null || fsm.samuraiBlackboard.health <= 0.0f)
        { // ��ǰ�ѻ���boss��ǰboss��������ȥǽ��
            virtualCamera1.Priority = 10;
            virtualCamera2.Priority = 9;
            if (playerLight.activeInHierarchy && globalLight.intensity < 0.8f)
            {
                StartCoroutine(LightChangeDistinguish());
            }
        }

        if (!lightChange && boss != null)
        { // ����boss��δ���ù�Э��
            if (fsm.samuraiBlackboard.health <= fsm.samuraiBlackboard.maxHealth / 2)
            { // boss����С��50%
                StartCoroutine(LightChangeDim());
            }
        }
    }

    // ���߱䰵
    IEnumerator LightChangeDim()
    {
        lightChange = true;
        playerLight.SetActive(true);
        while (globalLight.intensity >= 0.6f || globalLight.color.g >= 190f / 255f || globalLight.color.b >= 190f / 255f)
        {
            globalLight.intensity = Mathf.Lerp(globalLight.intensity, 0.55f, 0.01f); // �޸�����
            redColor.g = Mathf.Lerp(redColor.g, 180f / 255f, 0.01f);
            redColor.b = Mathf.Lerp(redColor.b, 180f / 255f, 0.01f);
            globalLight.color = redColor;
            yield return null;
        }
    }

    // ���߻ָ�
    IEnumerator LightChangeDistinguish()
    {
        playerLight.SetActive(false);
        while (globalLight.intensity < 0.8f || globalLight.color.g < 1 || globalLight.color.b < 1)
        {
            globalLight.intensity = Mathf.Lerp(globalLight.intensity, 0.85f, 0.01f); // �޸�����
            redColor.g = Mathf.Lerp(redColor.g, 1, 0.01f);
            redColor.b = Mathf.Lerp(redColor.b, 1, 0.01f);
            globalLight.color = redColor;
            yield return null;
        }
    }
}
