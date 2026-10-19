using System.Collections;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NPC : MonoBehaviour, IInteractable
{
    public NPCDialog dialogueData;
    private DialogueController dialogueUI;
    private int dialogIndex;
    public bool isTyping, isDialogActive;


    private enum QuestState { NotStarted, InProgress, Completed }
    private QuestState questState = QuestState.NotStarted;

    private void Start()
    {
        dialogueUI = DialogueController.Instance;

    }

    public bool CanInteract()
    {
        return !isDialogActive;
    }

    public void Interact()
    {
        if (dialogueData == null && !isDialogActive)
        {

            return;
        }
        if (isDialogActive)
        {
            NextLine();
        }
        else
        {
            StartDialog();
        }
    }
    void StartDialog()
    {
        dialogueUI.SetPlayerDirection(transform.position);

        SyncQueststate();
        if (questState == QuestState.NotStarted)
        {
            dialogIndex = 0;
        }
        else if (questState == QuestState.InProgress)
        {
            dialogIndex = dialogueData.questInProgressIndex;
        }
        else if (questState == QuestState.Completed)
        {
            dialogIndex = dialogueData.questCompletedIndex;
        }



        isDialogActive = true;



        dialogueUI.SetNPCInfo(dialogueData.npcName, dialogueData.npcPortrait);

        dialogueUI.showDialogueUi(true);
        DisplayCurrentLine();


    }
    private void SyncQueststate()
    {
        if (dialogueData.quest == null) return;

        string questID = dialogueData.quest.questID;

        if (QuestController.Instance.IsQuestCompeleted(questID) || QuestController.Instance.IsQuestHandedIn(questID))
        {
            questState = QuestState.Completed;
        }

        else if (QuestController.Instance.IsQuestActive(questID))
        {
            questState = QuestState.InProgress;
        }
        else
        {
            questState = QuestState.NotStarted;
        }

    }
    void NextLine()
    {
        if (isTyping)
        {
            StopAllCoroutines();
            dialogueUI.SetDialogueText(dialogueData.dialogLines[dialogIndex]);
            isTyping = false;
        }

        dialogueUI.ClearChoices();
        if (dialogueData.endDialogueLines.Length > dialogIndex && dialogueData.endDialogueLines[dialogIndex])
        {
            EndDialogue();
            return;
        }
        foreach (DialogueChoice dialogueChoice in dialogueData.choices)
        {
            if (dialogueChoice.dialogueIndex == dialogIndex)
            {
                DisplayChoices(dialogueChoice);
                return;
            }
        }



        if (++dialogIndex < dialogueData.dialogLines.Length)
        {
            DisplayCurrentLine();

        }
        else
        {
            EndDialogue();
        }
    }

    IEnumerator TypeLine()
    {
        isTyping = true;
        dialogueUI.SetDialogueText("");

        foreach (char letter in dialogueData.dialogLines[dialogIndex])
        {

            dialogueUI.SetDialogueText(dialogueUI.dialogText.text += letter);
            yield return new WaitForSeconds(dialogueData.typingSpeed);
        }

        isTyping = false;

        if (dialogueData.autoProgressLines.Length > dialogIndex && dialogueData.autoProgressLines[dialogIndex])
        {
            yield return new WaitForSeconds(dialogueData.autoProgressDelays);
            NextLine();
        }

    }
    void DisplayChoices(DialogueChoice choice)
    {
        for (int i = 0; i < choice.choices.Length; i++)
        {
            int nextIndex = choice.nextDialogueIndexes[i];
            bool givesQuest = choice.givesQuest[i];
            dialogueUI.CreateChoiceButton(choice.choices[i], () => ChooseOption(nextIndex, givesQuest));
        }
    }
    void ChooseOption(int nextIndex, bool givesQuest)
    {
        if (givesQuest)
        {
            QuestController.Instance.AcceptQuest(dialogueData.quest);
            questState = QuestState.InProgress;
        }
        dialogIndex = nextIndex;
        dialogueUI.ClearChoices();
        DisplayCurrentLine();
    }
    void DisplayCurrentLine()
    {
        StopAllCoroutines();
        StartCoroutine(TypeLine());
    }
    public void EndDialogue()
    {

        if (questState == QuestState.Completed && !QuestController.Instance.IsQuestHandedIn(dialogueData.quest.questID))
        {
            HandleQuestCompletion(dialogueData.quest);
        }
        StopAllCoroutines();
        isDialogActive = false;
        dialogueUI.SetDialogueText("");
        dialogueUI.showDialogueUi(false);
    }
    public void HandleQuestCompletion(Quest quest)
    {
        if (quest == null)
        {
            Debug.LogError("Fehler: 'quest' ist null in HandleQuestCompletion!");
            return;
        }
        if (RewardController.Instance == null)
        {
            Debug.LogError("Fehler: RewardController.Instance fehlt in der Szene!");
            return;
        }

        if (QuestController.Instance == null)
        {
            Debug.LogError("Fehler: QuestController.Instance fehlt in der Szene!");
            return;
        }
        RewardController.Instance.GiveQuestReward(quest);
        QuestController.Instance.HandInQuest(quest.questID);
    }
}
