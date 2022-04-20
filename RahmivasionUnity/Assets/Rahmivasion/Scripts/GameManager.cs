    using System.Collections;
using System.Collections.Generic;
    using Unity.VisualScripting;
    using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    
    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
        // @TODO: Add singleton implementation

        if (instance != null || FindObjectOfType<GameManager>())
        {
            Destroy(gameObject);
        }
    }
}
