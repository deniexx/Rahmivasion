using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    private static bool isUsingSwipeInput = true;
    
    // Start is called before the first frame update
    void Awake()
    {
        if (instance != null || FindObjectOfType<GameManager>())
        {
            Destroy(gameObject);
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void SetIsUsingSwipeInput(bool isUsingSwipe)
    {
        isUsingSwipeInput = isUsingSwipe;
    }

    public static bool GetIsUsingSwipeInput()
    {
        return isUsingSwipeInput;
    }
}
