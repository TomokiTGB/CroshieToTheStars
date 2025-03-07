using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

//https://www.youtube.com/watch?v=NyFYNsC3H8k
public class SceneLoading : MonoBehaviour
{
    public static SceneLoading instance;

    [SerializeField] private GameObject canvas;
    [SerializeField] private Image img_black;

    [SerializeField] private Slider progressBar;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void LoadLevel(int sceneIndex)
    {
        StartCoroutine(LoadLevelAsynch(sceneIndex));
    }
    IEnumerator LoadLevelAsynch(int sceneIndex)
    {
        progressBar.value = 0;
        canvas.SetActive(true);
        canvas.transform.GetChild(0).gameObject.SetActive(true);
        AsyncOperation loadOperation = SceneManager.LoadSceneAsync(sceneIndex);

        while (!loadOperation.isDone)
        {
            float progress = Mathf.Clamp01(loadOperation.progress / 0.9f);
            progressBar.value = progress;
            yield return null;
        }
        canvas.transform.GetChild(0).gameObject.SetActive(false);
        Coroutine fade = StartCoroutine(FadeImage(true, img_black, 0.3f));
        yield return fade;
        canvas.SetActive(false);
    }
    public IEnumerator FadeImage(bool fadeAway, Image img, float duration)
    {
        for (float i = 0; i <= duration; i += Time.unscaledDeltaTime)
        {
            img.color = new Color(img.color.r, img.color.g, img.color.b, fadeAway ? (duration - i) / duration : i / duration);
            yield return null;
        }
    }
}
