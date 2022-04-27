using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainUIManager : MonoBehaviour
{
    [SerializeField] private Button forwardButton;
    [SerializeField] private Button backButton;

    void Start()
    {
        if (GameManager.GetIsUsingSwipeInput())
        {
            Destroy(forwardButton.gameObject);
            Destroy(backButton.gameObject);
        }
    }
}
