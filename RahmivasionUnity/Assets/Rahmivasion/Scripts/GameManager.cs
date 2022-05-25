using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    [SerializeField] private bool isUsingSwipeInput = true;
    
    public static GameManager GetInstance()
    {
        return instance;
    }
    
    // Start is called before the first frame update
    void Awake()
    {
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

    public void SetIsUsingSwipeInput()
    {
        isUsingSwipeInput = !isUsingSwipeInput;
    }

    public bool GetIsUsingSwipeInput()
    {
        return isUsingSwipeInput;
    }
}
