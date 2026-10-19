using System.Collections.Generic;
using UnityEngine;

public class ItemDictionary : MonoBehaviour
{
    public List<item> itemPrefabs;
    private Dictionary<int, GameObject> itemDictionary;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        itemDictionary = new Dictionary<int, GameObject>();

        for (int i = 0; i < itemPrefabs.Count; i++)
        {
            // Überprüfen, dass der Eintrag im Inspector nicht leer/missing ist
            if (itemPrefabs[i] != null)
            {
                // ID dynamisch anhand der Listenposition vergeben
                itemPrefabs[i].ID = i + 1;

                // Direkt in das Dictionary eintragen, um eine zweite Schleife zu sparen
                itemDictionary[itemPrefabs[i].ID] = itemPrefabs[i].gameObject;
            }
        }
    }
    public GameObject GetItemPrefab(int itemID)
    {
        itemDictionary.TryGetValue(itemID, out GameObject prefab);
        if (prefab == null)
        {
            Debug.LogWarning($"Item with ID (itemID) not found ");
        }
        return prefab;
    }

}
