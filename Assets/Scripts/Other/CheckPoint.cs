using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private bool isClose;
    [SerializeField] private GameObject keyboardInfo;
    [SerializeField] private int sceneID; // 场景ID
    [SerializeField] private int checkPointID; // 记录点ID
    private Color color;
    private AnimatorStateInfo info;
    private GameObject player;
    private float life;
    private int coinCnt;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        keyboardInfo = transform.GetChild(0).gameObject;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && isClose)
        {
            animator.SetBool("isEffect", true);
            IEnumerator ie = SwitchAnimation();
            Coroutine cor = StartCoroutine(ie);
            if (player != null)
            {
                AudioSourceManager.Instance.PlaySound(GlobalAudioClips.CheckPoint);
                player.GetComponent<PlayerControll>().HealLife(100f);
                player.GetComponent<PlayerControll>().GetLifeCoinData(ref life, ref coinCnt);
                GameManager.Instance.SaveUserData(life, coinCnt, sceneID, checkPointID);
                TipsBoxManager.Instance.ShowTipsBox("存档成功", 2f);
            }
        }
    }

    // 获取记录点ID
    public int CheckPointID
    {
        get
        {
            return checkPointID;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            keyboardInfo.SetActive(true);
            isClose = true;
            player = collision.gameObject;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            keyboardInfo.SetActive(false);
            player = null;
            isClose = false;
        }
    }

    /// <summary>
    /// 控制存档动画
    /// </summary>
    /// <returns></returns>
    IEnumerator SwitchAnimation()
    {
        yield return new WaitForEndOfFrame();
        while (true)
        {
            info = animator.GetCurrentAnimatorStateInfo(0);
            if (info.normalizedTime >= 0.95f)
            {
                animator.SetBool("isEffect", false);
                yield break;
            }
            yield return null;
        }
    }
}
