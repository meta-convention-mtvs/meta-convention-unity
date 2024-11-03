using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoadingWithFadeOut : MonoBehaviour
{
    public GameObject blackUi;
    public float sceneTransitionTime = 3.0f;
    public float fadeDuration = 2.0f;

    void Start()
    {
        GameObject go = Instantiate(blackUi);
        CanvasGroup canvasGroup = go.GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0f;
        StartCoroutine(NextScene(canvasGroup));
    }
    IEnumerator NextScene(CanvasGroup canvasGroup)
    {
        yield return new WaitForSeconds(sceneTransitionTime);
        canvasGroup.alpha = 0f; // 알파 값을 투명으로 설정

        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsed / fadeDuration);
            yield return null;
        }

        // 최종 알파 값 설정 (완전 불투명)
        canvasGroup.alpha = 1f;
        yield return null;

        SceneManager.LoadScene("MainScene_CHJ");
    }

}
