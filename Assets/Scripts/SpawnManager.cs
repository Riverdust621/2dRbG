using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public spawnableobjects spawnData;
    public float spawnpercentage;
    public GameObject EnemyspawnpointsPanel;
    public GameObject chestspawnpointsPanel;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Spawnobjects();
    }

    private void Spawnobjects()
    {
        foreach (Transform enemyspawnpoints in EnemyspawnpointsPanel.transform)
        {
            Spawnpoint spawnscript = enemyspawnpoints.GetComponent<Spawnpoint>();
            spawnscript.Spawn(spawnpercentage, spawnData.enemys);
        }
        foreach (Transform enemyspawnpoints in chestspawnpointsPanel.transform)
        {
            Spawnpoint spawnscript = enemyspawnpoints.GetComponent<Spawnpoint>();
            spawnscript.Spawn(spawnpercentage, spawnData.chests);
        }
        

    }
}
