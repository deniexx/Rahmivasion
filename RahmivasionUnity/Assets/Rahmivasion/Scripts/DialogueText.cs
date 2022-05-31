using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DialogueText
{
    public string name;

    [TextArea(3, 10)]
    public List<string> sentences;
}
