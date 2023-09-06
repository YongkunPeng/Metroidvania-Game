using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShinningEffect : MonoBehaviour
{
    [SerializeField] private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        animator.Play("Shinning");
    }

    // вўВиЬиаЇ
    public void HideEffect()
    {
        gameObject.SetActive(false);
    }
}
