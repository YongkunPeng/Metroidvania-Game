using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossInfoUI : BasePanel
{
    [SerializeField] private float health; // …˙√¸÷µ
    [SerializeField] private float toughness; // »Õ–‘
    [SerializeField] private Slider healthBar;
    [SerializeField] private Slider toughnessBar;
    private SamuraiFSMAI boss;

    private void Awake()
    {
        OpenPanel(UIConst.BossInfo);
        boss = GameObject.FindObjectOfType<SamuraiFSMAI>();
    }

    private void Update()
    {
        health = Mathf.Clamp(boss.samuraiBlackboard.health, 0.0f, 300.0f); ;
        toughness = Mathf.Clamp(boss.samuraiBlackboard.toughness, 0.0f, 50.0f);

        healthBar.value = health / 300.0f;
        toughnessBar.value = toughness / 50.0f;

        if (health == 0.0f)
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        ClosePanel();
    }
}
