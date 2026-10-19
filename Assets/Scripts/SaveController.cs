using System.IO;
using System.Linq;
using UnityEngine;
using Unity.Cinemachine;
using System.Collections.Generic;

public class SaveController : MonoBehaviour
{
    private string saveLocation;
    private InventoryController inventoryController;
    private HotbarController hotbarController;
    private ChestScript[] chests;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        InitalalizeComponents();
        LoadGame();
    }
    private void InitalalizeComponents()
    {
        saveLocation = Path.Combine(Application.persistentDataPath, "saveData.json");
        inventoryController = FindAnyObjectByType<InventoryController>();
        hotbarController = FindAnyObjectByType<HotbarController>();
        chests = FindObjectsByType<ChestScript>(FindObjectsSortMode.None);
    }

    // Update is called once per frame
    public void Savegame()
    {


        SaveData saveData = new SaveData
        {
            playerPosition = GameObject.FindGameObjectWithTag("Player").transform.position,
            mapBoundary = FindAnyObjectByType<CinemachineConfiner2D>().BoundingShape2D.gameObject.name,
            inventorySaveData = inventoryController.GetInventoryItems(),
            hotbarSaveData = hotbarController.GetHotbarItems(),
            chestSaveData = GetChestState(),
            questProgressData = QuestController.Instance.activeQuests,
            handinQuestIDs = QuestController.Instance.handinQuestIDs
        };
        File.WriteAllText(saveLocation, JsonUtility.ToJson(saveData));
    }
    private List<ChestSaveData> GetChestState()
    {
        List<ChestSaveData> chestStates = new List<ChestSaveData>();
        foreach (ChestScript chest in chests)
        {
            ChestSaveData chestSaveData = new ChestSaveData
            {
                chestID = chest.ChestID,
                isOpened = chest.isOpened
            };
            chestStates.Add(chestSaveData);
        }
        return chestStates;
    }
    public void LoadGame()
    {
        if (File.Exists(saveLocation))
        {

            SaveData saveData = JsonUtility.FromJson<SaveData>(File.ReadAllText(saveLocation));
            GameObject.FindGameObjectWithTag("Player").transform.position = saveData.playerPosition;
            FindAnyObjectByType<CinemachineConfiner2D>().BoundingShape2D = GameObject.Find(saveData.mapBoundary).GetComponent<PolygonCollider2D>();
            inventoryController.SetinventoryItems(saveData.inventorySaveData);
            hotbarController.SetHotbarItems(saveData.hotbarSaveData);
            LoadChestStates(saveData.chestSaveData);

            QuestController.Instance.LoadQuestProgress(saveData.questProgressData);
            QuestController.Instance.handinQuestIDs = saveData.handinQuestIDs;


        }
        else
        {
            Savegame();
            inventoryController.SetinventoryItems(new List<InventorySaveData>());
            hotbarController.SetHotbarItems(new List<InventorySaveData>());
        }
    }
    private void LoadChestStates(List<ChestSaveData> chestStates)
    {

        foreach (ChestScript chest in chests)
        {
            ChestSaveData chestSaveData = chestStates.FirstOrDefault(c => c.chestID == chest.ChestID);
            if (chestSaveData != null)
            {
                chest.SetOpened(chestSaveData.isOpened);
            }
        }

    }
}
