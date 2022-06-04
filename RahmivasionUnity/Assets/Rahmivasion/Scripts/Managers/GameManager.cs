using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    
    [SerializeField] private bool isUsingSwipeInput = true;
    private Vector3 checkpointLocation;
    
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
        
        checkpointLocation = new Vector3(-10, -1.05f, 0);
    }

    private void Start()
    {
        RSceneManager.GetInstance().AfterLoadSceneCall.AddListener(ResetSpawnCheckpointOnNewLevel);
    }

    private void ResetSpawnCheckpointOnNewLevel()
    {
        checkpointLocation = new Vector3(-10, -1.05f, 0);
    }

    public void SetIsUsingSwipeInput()
    {
        isUsingSwipeInput = !isUsingSwipeInput;
    }

    public bool GetIsUsingSwipeInput()
    {
        return isUsingSwipeInput;
    }

    public void SetCheckpointLocation(Vector3 newPosition)
    {
        checkpointLocation = newPosition;
    }

    public void RespawnPlayer(GameObject player)
    {
        player.transform.position = checkpointLocation;
    }
}
