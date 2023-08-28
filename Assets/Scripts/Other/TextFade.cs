using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextFade : MonoBehaviour
{
    [SerializeField] private float waitTime;
    [SerializeField] private Text text;
    private Color color;

    private void OnEnable()
    {
        text = GetComponent<Text>();
        color = new Color(1, 0, 0, 1);
        text.color = color;
        StartCoroutine(FadeText());
    }

    IEnumerator FadeText()
    {
        yield return new WaitForSeconds(waitTime);
        while (text.color.a >= 0.05f)
        {
            color.a = Mathf.Lerp(color.a, 0, 0.005f);
            text.color = color;
            yield return null;
        }
        gameObject.SetActive(false);
    }
}
