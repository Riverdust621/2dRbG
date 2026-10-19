using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class QuestUi : MonoBehaviour
{
    public Transform questListContent;
    public GameObject questEntryPrefab;
    public GameObject objectiveTextPrefab;
    
    void Start()
    {
       

        UpdateQuestUI();
    }

    // Update is called once per frame
    public void UpdateQuestUI()
    {
        foreach (Transform child in questListContent)
        {
            Destroy(child.gameObject);
        } 
        foreach(var quest in QuestController.Instance.activeQuests)
        {
            GameObject entry = Instantiate(questEntryPrefab, questListContent);
            TMP_Text questNameText = entry.transform.Find("QuestName").GetComponent<TMP_Text>();
            Transform objectiveList = entry.transform.Find("ObjectivesList");


            questNameText.text = quest.quest.name;

            foreach(var Objective in quest.objectives)
            {
                GameObject objTextGo = Instantiate(objectiveTextPrefab, objectiveList);
                TMP_Text objText = objTextGo.GetComponent<TMP_Text>();
                objText.text = $"{Objective.description} ({Objective.currentAmount}/{Objective.requiredAmount})";
            }
        }
    }
}
