using System.Collections.Generic;
using UnityEngine;

public class enemydictionary : MonoBehaviour
{
    public List<enemyid> enemyPrefabs;
    private Dictionary<int, GameObject> enemyDictionary;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        enemyDictionary = new Dictionary<int, GameObject>();

        for (int i = 0; i < enemyPrefabs.Count; i++)
        {
            // Überprüfen, dass der Eintrag im Inspector nicht leer/missing ist
            if (enemyPrefabs[i] != null)
            {
                // ID dynamisch anhand der Listenposition vergeben
                enemyPrefabs[i].ID = i + 1;

                // Direkt in das Dictionary eintragen, um eine zweite Schleife zu sparen
                enemyDictionary[enemyPrefabs[i].ID] = enemyPrefabs[i].gameObject;
            }
        }
    }
    
}
