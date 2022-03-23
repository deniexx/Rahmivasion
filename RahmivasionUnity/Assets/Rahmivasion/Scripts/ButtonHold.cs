using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Button))]
public class ButtonHold : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
{
    public delegate void OnButtonReleased();

    public OnButtonReleased buttonReleased;
    
    private Button button;

    private bool buttonPressed = false;
    
    // Start is called before the first frame update
    void Awake()
    {
        button = GetComponent<Button>();
    }

    // Update is called once per frame
    void Update()
    {
        if (buttonPressed)
            button.onClick?.Invoke();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!button.interactable) return;
        buttonPressed = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!button.interactable) return;
        if (buttonPressed) buttonReleased();
        
        buttonPressed = false;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!button.interactable) return;
        if (buttonPressed) buttonReleased();
        
        buttonPressed = false;
    }
}
