using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader Instance = null;
    [SerializeField] private LevelInfoSO levelInfo;
    [SerializeField] private BoxItemsSOEventChannelSO sendPackage;
    [SerializeField] private StringEventChannelSO setOutBoxItem;
    [SerializeField] private string mainSceneName;

    private void Awake()
    {
        // Singleton functionality.
        if (Instance == null)
        {
            Instance = GetComponent<SceneLoader>();
        }
        else if (Instance != gameObject)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }

    public void LoadScene(string sceneName)
    {
        StartCoroutine(LoadSceneCoroutine(sceneName));
    }

    public void UnloadScene(string sceneName)
    {
        SceneManager.UnloadSceneAsync(sceneName);
    }

    public bool IsSceneLoaded(string sceneName)
    {
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            if (SceneManager.GetSceneAt(i).name == sceneName)
            {
                return true;
            }
        }
        return false;
    }

    private IEnumerator LoadSceneCoroutine(string sceneName)
    {
        var loading = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        yield return loading;
        yield return null;

        if (sceneName == mainSceneName)
        {
            foreach (BoxItemsSO package in levelInfo.Packages)
            {
                sendPackage.RaiseEvent(package);
            }

            setOutBoxItem.RaiseEvent(levelInfo.BuildName);
        }
    }
}