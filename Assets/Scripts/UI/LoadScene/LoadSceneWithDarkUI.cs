using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSceneWithDarkUI : BasePanel
{
    [SerializeField] private Animator animator;

    private void Awake()
    {
        OpenPanel(UIConst.LoadSceneWithDark);
        animator = GetComponent<Animator>();
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
        { // ʵʱ���¼��ؽ���
            if (operation.progress >= 0.9f)
            { // �������
                operation.allowSceneActivation = true;
                animator.SetTrigger("LoadComplete");
                if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.95f)
                {
                    Destroy(gameObject);
                }
            }
            yield return null;
        }

    }
}
