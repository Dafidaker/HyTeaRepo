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
        if (SceneUtility.GetBuildIndexByScenePath(scenePath + ".unity") >= 0)
            return true;
        if (SceneUtility.GetBuildIndexByScenePath("Assets/" + scenePath) >= 0)
            return true;
        if (SceneUtility.GetBuildIndexByScenePath("Assets/" + scenePath + ".unity") >= 0)
            return true;
        return false;
    }

    public void FadeToScene(string sceneName)
    {
        if (sceneName != null && !isFading && IsSceneLoadableFromPath(sceneName))
        {
            StartCoroutine(FadeOutIn(sceneName));
        }
    }
    public string GetNextSceneName()
    {
        // Get the current scene index
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;

        // Calculate the next scene index
        int nextSceneIndex = (currentSceneIndex + 1) % SceneManager.sceneCountInBuildSettings;

        // Get the path of the next scene
        string nextScenePath = SceneUtility.GetScenePathByBuildIndex(nextSceneIndex);

        // Extract the scene name from the path
        string nextSceneName = System.IO.Path.GetFileNameWithoutExtension(nextScenePath);

        return nextSceneName;
    }
    
    public void FadeToNextScene()
    {
        /*int nextSceneIndex = (SceneManager.GetActiveScene().buildIndex + 1) % SceneManager.sceneCountInBuildSettings;
        
        var sceneName = SceneManager.GetSceneByBuildIndex(SceneManager.GetActiveScene().buildIndex + 1).name;*/
        
        FadeToScene(GetNextSceneName());
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

