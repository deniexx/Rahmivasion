using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class RSceneManager : MonoBehaviour
{

    public UnityEvent AfterLoadSceneCall;

    private static RSceneManager instance;

    public static RSceneManager GetInstance()
    {
        return instance;
    }

    private void Awake()
    {
        // Singleton pattern, using in most Manager scripts
        if (instance != null)
        {
            gameObject.SetActive(false);
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    public void LoadAsyncScene(string newScene)
    {
        StartCoroutine(SwitchToScene(newScene));
    }

    // Asynchronous loads the scene and calls the UnityEvent after if it has been assigned
    private IEnumerator SwitchToScene(string newScene)
    {
        // Start loading the scene
        AsyncOperation asyncLoadLevel = SceneManager.LoadSceneAsync(newScene, LoadSceneMode.Single);
        // Wait until the level finish loading
        while (!asyncLoadLevel.isDone)
            yield return null;
        // Wait a frame so every Awake and Start method is called
        yield return new WaitForEndOfFrame();

        AfterLoadSceneCall?.Invoke();
    }
}
