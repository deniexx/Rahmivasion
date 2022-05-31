using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public class DialogueTriggerComponent : MonoBehaviour
{
    public DialogueText dialogue;

    private void OnTriggerEnter2D(Collider2D other) 
    {
        if (other.CompareTag("Player"))
        {
            DialogueManager.GetInstance().PrepareForDialogue(transform.position, dialogue);
        }
    }

    private void OnTriggerExit2D(Collider2D other) 
    {
        if (other.CompareTag("Player"))
        {
            DialogueManager.GetInstance().HideButton();
        }
    }
}
