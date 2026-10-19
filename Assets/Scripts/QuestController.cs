using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class QuestController : MonoBehaviour
{
    public static QuestController Instance { get; private set;}
    public List<QuestProgress> activeQuests = new();
    private QuestUi questUI;

    public List<string> handinQuestIDs = new();
    public int lastenemyid;
    private void OnEnable()
    {
        PlayerLive.OnKillChanged += CheckKillcountForQuest;
    }

    private void OnDisable()
    {
        PlayerLive.OnKillChanged -= CheckKillcountForQuest;
    }
    void Awake()
    {
        if( Instance == null) Instance = this;
        else Destroy(gameObject);
        questUI = FindAnyObjectByType<QuestUi>();
        InventoryController.Instance.OnInventoryChanged += CheckInventoryForQuests;
    }
    public void AcceptQuest(Quest quest)
    {
        if(IsQuestActive(quest.questID)) return;
        activeQuests.Add(new QuestProgress(quest));
        CheckInventoryForQuests();

        questUI.UpdateQuestUI();
    }
    public bool IsQuestActive(string questID) => activeQuests.Exists(q => q.QuestID == questID);

    public void GetLastEnemyId(int enemyid)
    {
        lastenemyid = enemyid;
    }
    public void CheckKillcountForQuest()
    {
        Debug.Log(lastenemyid);
        foreach (QuestProgress quest in activeQuests)
        {
            foreach(QuestObjective questObjective in quest.objectives)
            {
                if (questObjective.type != ObjectiveType.DefeatEnemy) continue;
                if (!int.TryParse(questObjective.objectiveID, out int enemyid)) continue;

                int nextAmount = (enemyid == lastenemyid) ? questObjective.currentAmount + 1 : questObjective.currentAmount;

                int newAmount = Mathf.Min(nextAmount, questObjective.requiredAmount);


                if (questObjective.currentAmount != newAmount)
                {
                    questObjective.currentAmount = newAmount;
                }
            }
        }

        questUI.UpdateQuestUI();

    }
        
    public void CheckInventoryForQuests()
    {
        Dictionary<int, int> itemCounts = InventoryController.Instance.GetItemCounts();

        foreach (QuestProgress quest in activeQuests)
        {
            foreach(QuestObjective questObjective in quest.objectives)
            {
                if (questObjective.type != ObjectiveType.Collectitem) continue;
                if (!int.TryParse(questObjective.objectiveID, out int itemID)) continue;

                int newAmount = itemCounts.TryGetValue(itemID, out int count) ? Mathf.Min(count, questObjective.requiredAmount) : 0;
                if (questObjective.currentAmount != newAmount)
                {
                    questObjective.currentAmount = newAmount;
                }
            }
        }

        questUI.UpdateQuestUI();

    }

    public bool IsQuestCompeleted(string questID)
    {
        QuestProgress quest = activeQuests.Find(q => q.QuestID == questID);
        return quest != null && quest.objectives.TrueForAll(o => o.isCompleted);
    }
    public void HandInQuest(string questID)
    {
        if (!RemoveRequiredItemsFromInventory(questID))
        {
            return;
        }
        QuestProgress quest = activeQuests.Find(q => q.QuestID == questID);
        if (quest != null)
        {
            handinQuestIDs.Add(questID);
            activeQuests.Remove(quest);
            questUI.UpdateQuestUI();
        }



    }
    public bool IsQuestHandedIn(string questID)
    {
        return handinQuestIDs.Contains(questID);
    }
    public bool RemoveRequiredItemsFromInventory(string questID)
    {
        QuestProgress quest = activeQuests.Find(q => q.QuestID == questID);
        if (quest == null) return false;
        Dictionary<int, int> requiredItems = new();

        foreach(QuestObjective objective in quest.objectives)
        {
            if(objective.type == ObjectiveType.Collectitem && int.TryParse(objective.objectiveID, out int itemID))
            {
                requiredItems[itemID] = objective.requiredAmount;
            }
        }


        Dictionary<int, int> itemCounts = InventoryController.Instance.GetItemCounts();
        foreach (var item in  requiredItems)
        {
            if(itemCounts.GetValueOrDefault(item.Key) < item.Value)
            {
                return false;
            }
        }
        foreach (var itemRequirment in requiredItems)
        {
            InventoryController.Instance.RemoveItemsFromInventory(itemRequirment.Key, itemRequirment.Value);
        }
        return true;
    }
    public void LoadQuestProgress(List<QuestProgress> savedQuests)
    {
        activeQuests = savedQuests ?? new();

        CheckInventoryForQuests();
        questUI.UpdateQuestUI();
    }

}
