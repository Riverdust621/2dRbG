using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using UnityEngine.UI;

public class HotbarController : MonoBehaviour
{
    public GameObject hotbarPanel;
    public GameObject slotPrefab;
    public int slotCount = 10;
    public static HotbarController Instance {get; private set;}

    private ItemDictionary itemDictionary;


    private Key[] hotKeys;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        itemDictionary = FindAnyObjectByType<ItemDictionary>();
        hotKeys = new Key[slotCount];
        for (int i = 0; i < slotCount; i++)
        {
            hotKeys[i] = i < 9 ? (Key)((int)Key.Digit1 + i) : Key.Digit0;
        }
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < slotCount; i++)
        {
            if (Keyboard.current[hotKeys[i]].wasPressedThisFrame)
            {
                UseItemInSlot(i);
            }
        }
    }
    void UseItemInSlot(int index)
    {
        HotbarUpdate();
        Slot slot = hotbarPanel.transform.GetChild(index).GetComponent<Slot>();
        Image slotimage = hotbarPanel.transform.GetChild(index).GetComponent<Image>();
        slotimage.color = Color.skyBlue;
        if (slot.currentItem != null)
        {
            item item = slot.currentItem.GetComponent<item>();
            item.UseItem();
        }
    }
    public void HotbarUpdate()
    {
        foreach (Transform slottransform in hotbarPanel.transform)
        {
            Image image = slottransform.GetComponent<Image>();
            image.color = Color.gray;
        }
    }
    public bool AddItem(GameObject itemPrefab)
    {
        item itemToAdd = itemPrefab.GetComponent<item>();
        if (itemToAdd == null) return false;

        foreach (Transform slotTransform in hotbarPanel.transform)
        {
            Slot slot = slotTransform.GetComponent<Slot>();
            if (slot != null & slot.currentItem != null)
            {
                item slotItem = slot.currentItem.GetComponent<item>();
                if (slotItem != null && slotItem.ID == itemToAdd.ID)
                {
                    slotItem.AddToStack();
                    InventoryController.Instance.RebuildItemCounts();
                    return true;
                }

            }
        }


        foreach (Transform slotTransform in hotbarPanel.transform)
        {
            Slot slot = slotTransform.GetComponent<Slot>();
            if (slot != null & slot.currentItem == null)
            {

                GameObject newItem = Instantiate(itemPrefab, slotTransform);
                newItem.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                slot.currentItem = newItem;
                InventoryController.Instance.RebuildItemCounts();
                return true;
            }
        }
        Debug.Log("Inventory full");
        return false;
    }



    public List<InventorySaveData> GetHotbarItems()
    {
        List<InventorySaveData> hotbarData = new List<InventorySaveData>();
        foreach (Transform slotTransform in hotbarPanel.transform)
        {
            Slot slot = slotTransform.GetComponent<Slot>();

            // SICHERHEITSABFRAGE: Existiert die Slot-Komponente auf dem Objekt?
            if (slot != null)
            {
                if (slot.currentItem != null)
                {
                    item item = slot.currentItem.GetComponent<item>();

                    // Stelle sicher, dass auch das item-Skript existiert, bevor du die ID abfragst
                    if (item != null)
                    {
                        hotbarData.Add(new InventorySaveData
                        {
                            itemID = item.ID,
                            slotIndex = slotTransform.GetSiblingIndex()
                        });
                    }
                }
            }
        }
        return hotbarData;
    }
    public void SetHotbarItems(List<InventorySaveData> hotbarSaveData)
    {
        // 1. ALTE SLOTS SOFORT LÖSCHEN
        // Wir laufen rückwärts durch die Schleife, da sich die Kind-Anzahl beim Löschen ändert
        for (int i = hotbarPanel.transform.childCount - 1; i >= 0; i--)
        {
            // DestroyImmediate löscht das Objekt augenblicklich aus der Hierarchie
            DestroyImmediate(hotbarPanel.transform.GetChild(i).gameObject);
        }

        // 2. NEUE SLOTS ERSTELLEN
        for (int i = 0; i < slotCount; i++)
        {
            Instantiate(slotPrefab, hotbarPanel.transform);
        }

        // 3. ITEMS IN DIE SLOTS LADEN
        foreach (InventorySaveData data in hotbarSaveData)
        {
            if (data.slotIndex < slotCount)
            {
                Transform childTransform = hotbarPanel.transform.GetChild(data.slotIndex);
                Slot slot = childTransform.GetComponent<Slot>();

                // Sicherheitscheck, falls ein Slot-Skript fehlt
                if (slot != null)
                {
                    GameObject itemPrefab = itemDictionary.GetItemPrefab(data.itemID);
                    if (itemPrefab != null)
                    {
                        GameObject item = Instantiate(itemPrefab, slot.transform, false);
                        item.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                        slot.currentItem = item;
                    }
                }
            }
        }
    }
}
