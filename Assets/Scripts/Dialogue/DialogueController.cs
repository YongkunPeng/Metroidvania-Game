using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class DialogueController : MonoBehaviour
{
    [Header("Ҫ��ʾ���ı�")]
    [SerializeField] private TextAsset textAsset;

    [SerializeField] private Text text;
    [SerializeField] private List<string> textList = new List<string>();
    [SerializeField] private int index;
    [SerializeField] private GameObject choosePanel;
    [SerializeField] private bool isOver;

    private void Awake()
    {
        text = transform.GetChild(2).GetComponent<Text>();
        choosePanel = transform.GetChild(3).gameObject;
    }

    private void OnEnable()
    {
        index = 1;
        GetTextByTXT(textAsset);
        text.DOText(textList[0], textList[0].Length / 10);
        isOver = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && index != textList.Count)
        {
            text.DOText("", 0); // ����ı���
            text.DOText(textList[index], textList[index].Length / 10);
            index++;
        }
        if (index == textList.Count && !isOver)
        {
            choosePanel.SetActive(true);
            isOver = true;
        }
    }

    void GetTextByTXT(TextAsset asset)
    {
        textList.Clear();
        var splitText = asset.text.Split("\n");

        foreach (var text in splitText)
        {
            textList.Add(text);
        }
    }

    public void Accept()
    {
        text.DOText("", 0); // ����ı���
        text.DOText("�ţ��ҵ������Ϣ��", 1);
        choosePanel.SetActive(false);
    }

    public void Deny()
    {
        text.DOText("", 0); // ����ı���
        text.DOText("���������Ҳ��������ȥ��", 1);
        choosePanel.SetActive(false);
    }

    public void Genshin()
    {
        text.DOText("", 0); // ����ı���
        text.DOText("�Բۣ�ԭ��", 1);
        choosePanel.SetActive(false);
    }
}
