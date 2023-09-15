using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class WallController : MonoBehaviour
{
    private SamuraiFSMAI fsm;
    private bool isDead = true;

    private void Awake()
    {
        fsm = GameObject.FindObjectOfType<SamuraiFSMAI>();
    }

    private void OnEnable()
    {
        AudioSourceManager.Instance.PlayBGM(GlobalAudioClips.BossFight);
        transform.DOMoveY(2.0f, 1f).From();
    }

    private void FixedUpdate()
    {
        if (fsm.samuraiBlackboard.health <= 0.0f)
        {
            if (isDead)
            {
                isDead = false;
                AudioSourceManager.Instance.PlayBGM(GlobalAudioClips.BGM1);
                transform.DOMoveY(3.0f, 1f);
            }
        }
    }
}
