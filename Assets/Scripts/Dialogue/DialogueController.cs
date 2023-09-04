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
    [SerializeField] private List<string> textList = new List<string>(); // �洢���ζԻ������ı�
    [SerializeField] private int index = 0;
    [SerializeField] private GameObject choosePanel;
    [SerializeField] private GameObject knight;
    [SerializeField] private Mission.MissionStatus status;
    [SerializeField] private bool isTyping = true; // �Ƿ����ڴ���
    public Tweener typeTween;

    private void Awake()
    {
        text = transform.GetChild(2).GetComponent<Text>();
        knight = GameObject.FindObjectOfType<Knight>().gameObject;
        choosePanel = transform.GetChild(3).gameObject;
    }

    private void OnEnable()
    {
        index = 0;

        // ��������״̬���ز�ͬ�Ի�
        status = knight.GetComponent<MissionDelegate>().mission.missionStatus;
        if (status == Mission.MissionStatus.Unaccepted)
        {
            textAsset = Resources.Load<TextAsset>("Text/Knight/Knight Dialog1");
        }
        else if (status == Mission.MissionStatus.Accepted)
        {
            textAsset = Resources.Load<TextAsset>("Text/Knight/Knight Dialog2");
        }
        else if (status == Mission.MissionStatus.Completed)
        {
            textAsset = Resources.Load<TextAsset>("Text/Knight/Knight Dialog3");
        }
        else if (status == Mission.MissionStatus.Rewarded)
        {
            textAsset = Resources.Load<TextAsset>("Text/Knight/Knight Dialog4");
        }
        GetTextByTXT(textAsset);

        StartNextDialogue();
        choosePanel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && !GameManager.Instance.isPaused)
        {
            if (!isTyping)
            { // δ����ʱ����E
                StartNextDialogue();
            }
            else
            { // ���ڴ���ʱ����E
                SkipTyping();
            }
        }
    }

    /// <summary>
    /// �л�����һ�ζԻ�����ȡ�öζԻ����ı�
    /// </summary>
    private void StartNextDialogue()
    {
        if (index < textList.Count)
        {
            string currText = textList[index];
            StartTyping(currText);
        }
    }

    /// <summary>
    /// �Դ��ֻ�Ч������Ի�
    /// </summary>
    private void StartTyping(string currText)
    {
        isTyping = true;
        if (text.text != "")
        {
            text.text = "";
        }
        typeTween = text.DOText(currText, currText.Length * 0.1f)
            .OnComplete(() =>
            {
                isTyping = false;
                index++;
                if (index == textList.Count && choosePanel != null && status == Mission.MissionStatus.Unaccepted)
                { // �н�����ǰ�Ի�����ʱ����ʾѡ��ť
                    choosePanel.SetActive(true);
                }
                else if (index == textList.Count && !isTyping)
                { // ��ѡ����Ҫ��˵�����һ�仰ʱ������Я�������رնԻ���
                    if (status == Mission.MissionStatus.Completed)
                    { // ���ı�Ϊ��������ı������轱�����޸�����״̬
                        knight.GetComponent<MissionDelegate>().mission.missionStatus = Mission.MissionStatus.Rewarded;
                    }
                    StartCoroutine(HideDialogue());
                }
            });
    }

    /// <summary>
    /// �������ֻ�Ч��
    /// </summary>
    private void SkipTyping()
    {
        if (typeTween != null && isTyping)
        {
            isTyping = false;
            typeTween.Complete();
        }
    }

    /// <summary>
    /// ��ȡ�Ի��ı��ļ����������з��ָ�����
    /// </summary>
    /// <param name="asset">�ı��ļ�</param>
    void GetTextByTXT(TextAsset asset)
    {
        textList.Clear();
        var splitText = asset.text.Split("\n");

        foreach (var text in splitText)
        {
            textList.Add(text);
        }
    }

    IEnumerator HideDialogue()
    {
        if (gameObject.activeInHierarchy == true)
        {
            yield return new WaitForSeconds(2);
            transform.parent.gameObject.SetActive(false);
        }
    }

    public void Accept()
    {
        text.DOText("", 0); // ����ı���
        text.DOText("�ţ��ҵ������Ϣ��", 1)
            .OnComplete(() =>
            {
                StartCoroutine(HideDialogue());
            });
        choosePanel.SetActive(false);
    }

    public void Deny()
    {
        text.DOText("", 0); // ����ı���
        text.DOText("���������Ҳ��������ȥ��", 1)
            .OnComplete(() =>
            {
                StartCoroutine(HideDialogue());
            });
        choosePanel.SetActive(false);
    }
}
