using System.Collections.Generic;
using UnityEngine;

public class Spawnpoint : MonoBehaviour
{
    public void Spawn(float spawnWahrscheinlichkeit, List<GameObject> gameObjects)
    {
        bool isSpawned = Random.value < spawnWahrscheinlichkeit;
        if (isSpawned)
        {
            int randomIndex = Random.Range(0, gameObjects.Count);
            Instantiate(gameObjects[randomIndex], transform.position, Quaternion.identity);
        }
    }
}
