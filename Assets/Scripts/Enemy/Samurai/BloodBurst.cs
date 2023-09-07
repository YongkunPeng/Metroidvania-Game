using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloodBurst : MonoBehaviour
{
    [SerializeField] private Animator animator;
    private AnimatorStateInfo info;

    private void Awake()
    {
        animator.GetComponent<Animator>();
    }

    private void OnEnable()
    {
        animator.Play("Burst");
    }

    private void Update()
    {
        info = animator.GetCurrentAnimatorStateInfo(0);
        if (info.normalizedTime >= 0.95f)
        {
            gameObject.SetActive(false);
        }
    }
}
