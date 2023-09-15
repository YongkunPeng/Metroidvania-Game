using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SettingsUI : BasePanel
{
    private float soundValue = 0;
    private float BGMValue = 0;
    [SerializeField] private Slider soundSlider;
    [SerializeField] private Slider BGMSlider;
    [SerializeField] private AudioMixer master;
    [SerializeField] private Text soundText;
    [SerializeField] private Text BGMText;
    [SerializeField] private Button mainMeneBtn;

    private void Awake()
    {
        OpenPanel(UIConst.Settings);
        soundSlider = transform.GetChild(0).GetChild(3).GetComponent<Slider>();
        BGMSlider = transform.GetChild(0).GetChild(4).GetComponent<Slider>();
        soundText = transform.GetChild(0).GetChild(3).GetChild(0).GetComponent<Text>();
        BGMText = transform.GetChild(0).GetChild(4).GetChild(0).GetComponent<Text>();
        mainMeneBtn = transform.GetChild(0).GetChild(5).GetComponent<Button>();

        master.GetFloat("Sound", out soundValue);
        soundSlider.value = soundValue;
        master.GetFloat("BGM", out BGMValue);
        BGMSlider.value = BGMValue;
    }

    private void OnEnable()
    {
        mainMeneBtn.onClick.AddListener(() => SceneLoadManager.Instance.LoadLevelByIndexWithSlider(0));
    }

    private void Start()
    {
        if (SceneManager.GetActiveScene().buildIndex != 0)
        { // 非主菜单
            mainMeneBtn.interactable = true;
            GameManager.Instance.isPaused = true;
            Time.timeScale = 0;
        }
    }

    private void Update()
    {
        soundText.text = ((int)(soundSlider.value + 80f)).ToString() + "%";
        BGMText.text = ((int)(BGMSlider.value + 80f)).ToString() + "%";
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            CloseSettings();
        }
    }

    /// <summary>
    /// 关闭设置面板
    /// </summary>
    private void CloseSettings()
    {
        if (!isRemove)
        {
            ClosePanel();
            GameManager.Instance.isPaused = false;
            Time.timeScale = 1f;
        }
    }

    private void OnDestroy()
    {
        CloseSettings();
        ClosePanel();
    }


    /// <summary>
    /// 控制音效大小
    /// </summary>
    /// <param name="s"></param>
    public void SetSoundVolume(Slider s)
    {
        master.SetFloat("Sound", s.value);
    }

    /// <summary>
    /// 控制背景音乐大小
    /// </summary>
    /// <param name="s"></param>
    public void SetBGMVolume(Slider s)
    {
        master.SetFloat("BGM", s.value);
    }
}
