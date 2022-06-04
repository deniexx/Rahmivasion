using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    private Queue<string> sentences;

    private static DialogueManager instance;

    [SerializeField] private Canvas canvas;
    [SerializeField] private Button button;
    [SerializeField] private Text sentenceText;
    [SerializeField] private Text nameText;
    [SerializeField] private GameObject dialogueGO;
    [SerializeField] private Animator animator;
    [SerializeField] private AudioSource typeSound;

    private DialogueText currDialogue;

    private static readonly int IsOpen = Animator.StringToHash("_IsOpen");

    private PlayerScript ps;


    public static DialogueManager GetInstance()
    {
        return instance;
    }

    // Start is called before the first frame update
    void Awake()
    {
        // Singleton pattern, using in most Manager scripts
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

        ps = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerScript>();
    }

    // Start is called before the first frame update
    void Start()
    {
        sentences = new Queue<string>();
    }

    public void StartDialogue()
    {
        animator.SetBool(IsOpen, true);

        foreach (string sentence in currDialogue.sentences)
        {
            sentences.Enqueue(sentence);
        }

        DisplayNextSentence();

        
        if (ps) ps.SetFrozen(true);
    }

    public void HideButton()
    {
        button.gameObject.SetActive(false);
    }

    public void PrepareForDialogue(Vector3 newPosition, DialogueText dialogue)
    {
        button.gameObject.SetActive(true);

        newPosition.y += 1.5f;
        button.transform.position = newPosition;

        sentences.Clear();
        currDialogue = dialogue;

        nameText.text = currDialogue.name;
    }

    public void DisplayNextSentence()
    {
        if (sentences.Count == 0)
        {
            EndDialogue();
            return;
        }

        StopAllCoroutines();
        typeSound.Stop();
        string currSentence = sentences.Dequeue();

        StartCoroutine(TypeSentence(currSentence));
    }

    IEnumerator TypeSentence(string sentence)
    {
        sentenceText.text = "";
        typeSound.Play();
        foreach(char letter in sentence.ToCharArray())
        {
            sentenceText.text += letter;
            yield return new WaitForEndOfFrame();
        }
        typeSound.Stop();
    }

    private void EndDialogue()
    {
        animator.SetBool("_IsOpen", false);
        if (ps) ps.SetFrozen(false);
    }
}
