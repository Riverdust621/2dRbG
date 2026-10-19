using System;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Quest/Quest")]

public class Quest : ScriptableObject
{
    public string questID;
    public string questName;
    public string descripition;
    public List<QuestObjective> objectives;
    public List<QuestReward> questRewards;

    private void OnValidate()
    {
        if (string.IsNullOrEmpty(questID))
        {
            questID = questName + Guid.NewGuid().ToString();
        }
    }


}
[System.Serializable]
public class QuestObjective
{
    public string objectiveID;
    public string description;
    public ObjectiveType type;
    public int requiredAmount;
    public int currentAmount;

    public bool isCompleted => currentAmount >= requiredAmount;
}
public enum ObjectiveType { Collectitem, DefeatEnemy, Reachlocation, TalkNPC, Custum }
[System.Serializable] 
public class QuestProgress
{
    public Quest quest;
    public List<QuestObjective> objectives;

    public QuestProgress(Quest quest)
    {
        this.quest = quest;
        objectives = new List<QuestObjective>();
        foreach (var obj in quest.objectives)
        {
            objectives.Add(new QuestObjective
            {
                objectiveID = obj.objectiveID,
                description = obj.description,
                type = obj.type,
                requiredAmount = obj.requiredAmount,
                currentAmount = 0
            });
        }
    }
    public bool IsCompleted => objectives.TrueForAll(o => o.isCompleted);
    public string QuestID => quest.questID;
}
[System.Serializable]

public class QuestReward
{
    public RewardType type;
    public int rewardID;
    public int amount = 1;
    public GameObject border;
}
public enum RewardType { Item, Gold, Experience, Custom, LevelProgress}