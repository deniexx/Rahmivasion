using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class SceneLoader : MonoBehaviour
{
    [SerializeField] private string sceneToLoad = "";

    private void OnTriggerEnter2D(Collider2D col)
    {
        RSceneManager.GetInstance().LoadAsyncScene(sceneToLoad);
    }
}
