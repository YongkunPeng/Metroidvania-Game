using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadSceneUI : BasePanel
{
    [SerializeField] private Text loadValue;
    [SerializeField] private Text loadTip;
    [SerializeField] private Slider loadSlider;

    private void Awake()
    {
        OpenPanel(UIConst.LoadScene);

        loadValue = transform.GetChild(1).GetChild(0).GetComponent<Text>();
        loadSlider = transform.GetChild(1).GetChild(1).GetComponent<Slider>();
        loadTip = transform.GetChild(1).GetChild(2).GetComponent<Text>();
    }

    public void LoadLevelByIndex(int index)
    {
        StartCoroutine(LoadLevel(index));
    }

    private void OnDestroy()
    {
        ClosePanel();
    }

    IEnumerator LoadLevel(int index)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(index);
        operation.allowSceneActivation = false;
        while (!operation.isDone)
        { // 实时更新加载进度
            loadSlider.value = operation.progress;
            loadValue.text = (operation.progress * 100) + "%";
            if (operation.progress >= 0.9f)
            { // 加载完成
                loadSlider.value = 1;
                loadTip.text = "加载完成\n点击任意键继续";
                loadValue.text = "100%";
                if (Input.anyKeyDown)
                {
                    operation.allowSceneActivation = true;
                }
            }
            yield return null;
        }
    }
}
