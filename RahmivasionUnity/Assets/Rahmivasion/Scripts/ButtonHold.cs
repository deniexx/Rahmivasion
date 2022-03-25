using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Button))]
public class ButtonHold : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
{
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

        buttonPressed = false;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!button.interactable) return;

        buttonPressed = false;
    }
}
