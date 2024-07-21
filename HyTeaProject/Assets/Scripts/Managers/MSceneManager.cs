using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MSceneManager : Singleton<MSceneManager>
{
    public float fadeDuration = 1f;
    [SerializeField] private CanvasGroup canvasGroup;
    private bool isFading = false;
    
    bool IsSceneLoadableFromPath(string scenePath)
    {
        if (String.IsNullOrWhiteSpace(scenePath))
            return false;
        if (SceneUtility.GetBuildIndexByScenePath(scenePath) >= 0)
            return true;

        // Allow some extra variations on scenePath since SceneManager.LoadSceneAsync is
        // more relaxed about scene path
        if (SceneUtility.GetBuildIndexByScenePath(scenePath + ".unity") >= 0)
            return true;
        if (SceneUtility.GetBuildIndexByScenePath("Assets/" + scenePath) >= 0)
            return true;
        if (SceneUtility.GetBuildIndexByScenePath("Assets/" + scenePath + ".unity") >= 0)
            return true;
        return false;
    }

    protected override void Awake()
    {
        base.Awake();
    }

    public void FadeToScene(string sceneName)
    {
        if (!isFading && IsSceneLoadableFromPath(sceneName))
        {
            StartCoroutine(FadeOutIn(sceneName));
        }
    }

    private IEnumerator FadeOutIn(string sceneName)
    {
        isFading = true;
        yield return StartCoroutine(FadeOut());

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        yield return StartCoroutine(FadeIn());
        isFading = false;
    }

    private IEnumerator FadeOut()
    {
        float timer = 0f;
        canvasGroup.alpha = 0;
        while (timer <= fadeDuration)
        {
            timer += Time.deltaTime;
            canvasGroup.alpha = timer / fadeDuration;
            yield return null;
        }
        canvasGroup.alpha = 1f;
    }

    private IEnumerator FadeIn()
    {
        float timer = 0f;
        canvasGroup.alpha = 1f;
        while (timer <= fadeDuration)
        {
            timer += Time.deltaTime;
            canvasGroup.alpha = 1f - (timer / fadeDuration);
            yield return null;
        }
        canvasGroup.alpha = 0f;
    }

}

