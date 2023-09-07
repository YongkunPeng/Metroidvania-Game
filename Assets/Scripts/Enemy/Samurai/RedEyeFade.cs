using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class RedEyeFade : MonoBehaviour
{
    [SerializeField] private SpriteRenderer sprite;
    [SerializeField] private Light2D redLight;

    private void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();
        redLight = GetComponent<Light2D>();
    }

    private void OnEnable()
    {
        sprite.color = Color.white;
        redLight.color = new Color(1, 0, 0, 1);
        StartCoroutine(FadeShadow());
    }

    IEnumerator FadeShadow()
    {
        while (sprite.color.a >= 0.1f)
        {
            sprite.color -= new Color(0, 0, 0, 0.01f);
            redLight.color -= new Color(0, 0, 0, 0.01f);
            yield return null;
        }
        ObjectPool.Instance.Push(gameObject);
    }
}
