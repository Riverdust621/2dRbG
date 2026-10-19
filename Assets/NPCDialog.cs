using UnityEngine;

[CreateAssetMenu(fileName = "New NPC Dialog", menuName = "NPC Dialog")]
public class NPCDialog : ScriptableObject
{
    public string npcName;
    public Sprite npcPortrait;
    public string[] dialogLines;
    public bool[] autoProgressLines;
    public bool[ ] endDialogueLines;
    public float typingSpeed = 0.05f;
    public AudioClip voiceSound;
    public float voicePitch = 1.0f;
    public float autoProgressDelays =  1.5f;

    public DialogueChoice[] choices;

    public int questInProgressIndex;
    public int questCompletedIndex;
    public Quest quest;
 // Time delay between each character
}
[System.Serializable]
public class DialogueChoice
{
    public int dialogueIndex;
    public string[] choices;
    public int[] nextDialogueIndexes;
    public bool[] givesQuest;

}
