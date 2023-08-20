using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private bool isClose;
    [SerializeField] private GameObject keyboardInfo;
    private Color color;
    private AnimatorStateInfo info;
    private GameObject player;
    private string id;
    private float life;
    private int arrowCnt;
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
                player.GetComponent<PlayerControll>().GetData(ref id, ref life, ref coinCnt, ref arrowCnt);
                UserData data = new UserData(id, life, arrowCnt, coinCnt);
                LocalConfig.SaveUserData(data);
                Debug.Log("±£´æÍê³É£¡");
            }
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
            isClose = false;
        }
    }

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
