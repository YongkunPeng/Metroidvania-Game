using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInfoUI : BasePanel
{
    public GameObject player;

    public Slider lifeBar;
    public Image lifeWhiteArea;
    public Image coldImage;
    public Image skillFrame;
    public Text coinText;
    public Text arrowText;
    
    public float life = 0;
    public float cold = 0;
    public int arrowCnt = 0;
    public int goldCoinCnt = 0;
    public bool isDash = false;

    public Color sprinkleColor; 

    private void Awake()
    {
        OpenPanel(UIConst.PlayerInfo);
        sprinkleColor.r = 0;
        sprinkleColor.g = 255;
        sprinkleColor.b = 255;
        sprinkleColor.a = 255;
        lifeBar = transform.GetChild(0).GetComponent<Slider>();
        coldImage = transform.GetChild(4).GetChild(0).GetChild(1).GetComponent<Image>();
        coinText = transform.GetChild(2).GetChild(2).GetComponent<Text>();
        arrowText = transform.GetChild(1).GetChild(2).GetComponent<Text>();
        skillFrame = transform.GetChild(4).GetChild(0).GetComponent<Image>();
        lifeWhiteArea = transform.GetChild(0).GetChild(1).GetChild(0).GetComponent<Image>();
        player = GameObject.Find("Player");
    }

    private void Update()
    {
        player.GetComponent<PlayerControll>().GetPlayerInfo(ref life, ref cold, ref goldCoinCnt, ref arrowCnt);
        LifeBarControll();
        DashColdControll();
        ArrowCntControll();
        CoinCntControll();
    }

    private void OnDestroy()
    {
        ClosePanel();
    }

    private void LifeBarControll()
    {
        if (life >= 0f)
        {
            lifeBar.value = life / 100f;
            IEnumerator ie = LifeDown();
            Coroutine cor = StartCoroutine(ie);
        }
    }

    private void DashColdControll()
    {
        if (cold > 0f)
        {
            isDash = true;
            coldImage.fillAmount = cold / 1f;
        }
        else if (cold < 0f && isDash)
        {
            isDash = false;
            IEnumerator ie = SkillComplete();
            Coroutine cor = StartCoroutine(ie);
        }
    }

    private void ArrowCntControll()
    {
        arrowText.text = arrowCnt.ToString();
    }

    private void CoinCntControll()
    {
        coinText.text = goldCoinCnt.ToString();
    }

    IEnumerator SkillComplete()
    {
        skillFrame.color = sprinkleColor;
        yield return new WaitForSeconds(0.1f);
        skillFrame.color = Color.white;
        yield return new WaitForSeconds(0.1f);
        skillFrame.color = sprinkleColor;
        yield return new WaitForSeconds(0.1f);
        skillFrame.color = Color.white;
        yield return null;
    }

    IEnumerator LifeDown()
    {
        while(Mathf.Abs(lifeBar.value - lifeWhiteArea.fillAmount) > 0.0001f)
        {
            lifeWhiteArea.fillAmount = Mathf.Lerp(lifeWhiteArea.fillAmount, lifeBar.value, 0.02f);
            yield return new WaitForSeconds(0.1f);
        }
    }
}
