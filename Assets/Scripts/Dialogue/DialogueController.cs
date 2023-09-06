using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class DialogueController : MonoBehaviour
{
    [Header("要显示的文本")]
    [SerializeField] private TextAsset textAsset;

    [SerializeField] private Text text;
    [SerializeField] private List<string> textList = new List<string>(); // 存储本次对话所有文本
    [SerializeField] private int index = 0;
    [SerializeField] private GameObject choosePanel;
    [SerializeField] private GameObject knight;
    [SerializeField] private Mission knightMisson;
    [SerializeField] private Mission.MissionStatus status;
    [SerializeField] private bool isTyping = true; // 是否正在打字
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

        // 根据任务状态加载不同对话
        knightMisson = knight.GetComponent<MissionDelegate>().mission;
        status = knightMisson.missionStatus;
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
            { // 未打字时按下E
                StartNextDialogue();
            }
            else
            { // 正在打字时按下E
                SkipTyping();
            }
        }
    }

    /// <summary>
    /// 切换到下一段对话，获取该段对话的文本
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
    /// 以打字机效果输出对话
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
                { // 承接任务前对话结束时，显示选择按钮
                    choosePanel.SetActive(true);
                }
                else if (index == textList.Count && !isTyping)
                { // 无选择需要，说完最后一句话时，启动携程数秒后关闭对话框
                    if (status == Mission.MissionStatus.Completed)
                    { // 若文本为完成任务文本，给予奖励，修改任务状态
                        GameObject.FindObjectOfType<PlayerControll>().ChangeCoinCnt(knightMisson.goldReward); // 获取任务赏金
                        foreach (var pair in knightMisson.itemsReward)
                        { // 获取物品奖励
                            GameManager.Instance.AddItem(GameManager.Instance.resourceDict[pair.Key], pair.Value);
                            TipsBoxManager.Instance.ShowTipsBox("获取任务奖励：<color=red>" + pair.Key + " * " + pair.Value + "</color>", 1.5f);
                        }
                        knight.GetComponent<MissionDelegate>().mission.missionStatus = Mission.MissionStatus.Rewarded; // 修改任务状态
                        TipsBoxManager.Instance.ShowTipsBox("以交付任务：" + knightMisson.missionName, 3f);
                    }
                    StartCoroutine(HideDialogue());
                }
            });
    }

    /// <summary>
    /// 跳过打字机效果
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
    /// 获取对话文本文件，并按换行符分割内容
    /// </summary>
    /// <param name="asset">文本文件</param>
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
        text.DOText("", 0); // 清空文本框
        text.DOText("嗯，我等你的消息！", 1)
            .OnComplete(() =>
            {
                StartCoroutine(HideDialogue());
            });
        choosePanel.SetActive(false);
    }

    public void Deny()
    {
        text.DOText("", 0); // 清空文本框
        text.DOText("这样吗，那我不能让你过去。", 1)
            .OnComplete(() =>
            {
                StartCoroutine(HideDialogue());
            });
        choosePanel.SetActive(false);
    }
}
