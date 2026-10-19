using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class LightningSpell : MonoBehaviour
{
    private Vector3 spawnPosition;
    [SerializeField]private float maxattackrange = 10;

    public float attackCooldown = 1f;
    public LayerMask layerMask;
    public void ShotTarget(GameObject projectilesPrefab)
    {
        List<GameObject> enemys = new List<GameObject>(GameObject.FindGameObjectsWithTag("Enemy"));
        PlayerControll player = GameObject.FindWithTag("Player").GetComponent<PlayerControll>();
        if (enemys != null)
        {
            foreach (GameObject enemy in enemys)
            {
                PlayerInRange(maxattackrange, enemy, projectilesPrefab);

            }
        }


            
    }
    public void PlayerInRange(float maxRange, GameObject enemy,GameObject gameObject)
    {

        PlayerControll player = GameObject.FindWithTag("Player").GetComponent<PlayerControll>();
        Vector2 direction = enemy.transform.position - player.transform.position;
        
        RaycastHit2D hitInfo = Physics2D.Raycast(player.transform.position, direction.normalized, maxRange, layerMask);
        if (hitInfo.collider != null)
        {
           
            if (hitInfo.collider.gameObject.CompareTag("Enemy"))
            {
                
                if (player.activeSpell == "Explosion Book")
                {
                    spawnPosition = enemy.transform.position;
                }
                else
                {
                    spawnPosition = enemy.transform.position + new Vector3(0, 1, 0);
                }
                Instantiate(gameObject, spawnPosition, Quaternion.identity);
                 
                

            }
            

        }
        
    }

}
