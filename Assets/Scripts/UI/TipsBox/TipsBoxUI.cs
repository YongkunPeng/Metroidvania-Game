using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class TipsBoxUI : BasePanel
{
    [SerializeField] private Text tipText;
    private Tweener tipTween;

    private void Awake()
    {
        OpenPanel(UIConst.TipsBox);
        tipText = transform.GetChild(0).GetComponent<Text>();
    }

    private void OnDestroy()
    {
        ClosePanel();
    }

    private void OnEnable()
    {

    }

    /// <summary>
    /// /设置文本内容
    /// </summary>
    /// <param name="text">文本内容</param>
    public void SetTipText(string text, float existTime)
    {
        tipText.text = text;
        tipTween = tipText.rectTransform.DOLocalMove(Vector3.up * 100, existTime);
        tipTween.SetUpdate(true);
        tipTween = tipText.DOFade(0, existTime)
            .OnComplete(() =>
            {
                Destroy(gameObject);
            });
        tipTween.SetUpdate(true);
    }
}
