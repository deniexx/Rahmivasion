using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void LoadScene(string sceneName)
    {
        if (sceneName == "Level01")
            RSceneManager.GetInstance().AfterLoadSceneCall.AddListener(AudioManager.GetInstance().PlayGameMusic);
        RSceneManager.GetInstance().LoadAsyncScene(sceneName);
    }

    public void Quit()
    {
        Application.Quit();
    }
}
