using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class InventoryController : MonoBehaviour
{
    private ItemDictionary itemDictionary;
    public GameObject inventoryPanel;
    public GameObject slotPrefab;
    public int slotCount;
    public GameObject[] itemPrefabs;

    public static InventoryController Instance { get; private set; }
    Dictionary<int, int> itemsCountCache = new();
    public event Action OnInventoryChanged;
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        itemDictionary = FindAnyObjectByType<ItemDictionary>();
        RebuildItemCounts();

    }
    public void RebuildItemCounts()
    {
        itemsCountCache.Clear();

        foreach (Transform slotTransform in inventoryPanel.transform)
        {
            Slot slot = slotTransform.GetComponent<Slot>();
            if (slot.currentItem != null)
            {
                item item = slot.currentItem.GetComponent<item>();
                if (item != null)
                {
                    itemsCountCache[item.ID] = itemsCountCache.GetValueOrDefault(item.ID, 0) + item.quantity;

                }
            }
        }
        OnInventoryChanged?.Invoke();
    }
    public Dictionary<int, int> GetItemCounts() => itemsCountCache;

    public bool AddItem(GameObject itemPrefab)
    {
        item itemToAdd = itemPrefab.GetComponent<item>();
        if (itemToAdd == null) return false;

        foreach (Transform slotTransform in inventoryPanel.transform)
        {
            Slot slot = slotTransform.GetComponent<Slot>();
            if (slot != null & slot.currentItem != null)
            {
                item slotItem = slot.currentItem.GetComponent<item>();
                if (slotItem != null && slotItem.ID == itemToAdd.ID)
                {
                    slotItem.AddToStack();
                    RebuildItemCounts();
                    return true;
                }

            }
        }


        foreach (Transform slotTransform in inventoryPanel.transform)
        {
            Slot slot = slotTransform.GetComponent<Slot>();
            if (slot != null & slot.currentItem == null)
            {

                GameObject newItem = Instantiate(itemPrefab, slotTransform);
                newItem.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                slot.currentItem = newItem;
                RebuildItemCounts();
                return true;
            }
        }
        Debug.Log("Inventory full");
        return false;
    }

    public List<InventorySaveData> GetInventoryItems()
    {
        List<InventorySaveData> invData = new List<InventorySaveData>();
        foreach (Transform slotTransform in inventoryPanel.transform)
        {
            Slot slot = slotTransform.GetComponent<Slot>();
            if (slot.currentItem != null)
            {
                item item = slot.currentItem.GetComponent<item>();
                invData.Add(new InventorySaveData
                {
                    itemID = item.ID,
                    slotIndex = slotTransform.GetSiblingIndex(),
                    quantity = item.quantity
                });

            }
        }
        return invData;
    }
    public void SetinventoryItems(List<InventorySaveData> inventorySaveData)
    {
        foreach (Transform child in inventoryPanel.transform)
        {
            Destroy(child.gameObject);
        }
        for (int i = 0; i < slotCount; i++)
        {
            Instantiate(slotPrefab, inventoryPanel.transform);

        }
        foreach (InventorySaveData data in inventorySaveData)
        {
            if (data.slotIndex < slotCount)
            {
                Slot slot = inventoryPanel.transform.GetChild(data.slotIndex).GetComponent<Slot>();
                GameObject itemPrefab = itemDictionary.GetItemPrefab(data.itemID);
                if (itemPrefab != null)
                {
                    GameObject item = Instantiate(itemPrefab, slot.transform, false);
                    item.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;

                    item itemComponent = item.GetComponent<item>();
                    if (itemComponent != null && data.quantity > 1)
                    {
                        itemComponent.quantity = data.quantity;
                        itemComponent.UpdateQuantityDisplay();
                    }
                    slot.currentItem = item;
                }
            }

        }
        RebuildItemCounts();


    }
    public void RemoveItemsFromInventory(int itemID, int amountToRemove)
    {
        foreach (Transform slotTransform in inventoryPanel.transform)
        {
            if (amountToRemove <= 0) break;

            Slot slot = slotTransform.GetComponent<Slot>();

            // Sicherer Unity-Null-Check statt ?.
            if (slot != null && slot.currentItem != null)
            {
                item itemComponent = slot.currentItem.GetComponent<item>();

                // Prüfen, ob die Komponente existiert und die ID übereinstimmt
                if (itemComponent != null && itemComponent.ID == itemID)
                {
                    // Nutze hier jetzt 'itemComponent' statt 'item'
                    int removed = Mathf.Min(amountToRemove, itemComponent.quantity);
                    itemComponent.RemoveFromStack(removed);
                    amountToRemove -= removed;

                    if (itemComponent.quantity == 0)
                    {
                        Destroy(slot.currentItem);
                        slot.currentItem = null;
                    }
                }
            }
        }
        RebuildItemCounts();
    }




    // Update is called once per frame

}
