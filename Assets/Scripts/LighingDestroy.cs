using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class LighingDestroy : MonoBehaviour
{
    private PlayerLive playerLive;

    public float lifetime = 1;
    public float attackCooldown = 1f;

    public bool isInRange = false;
    public float damage = 5f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameObject enemyObject = GameObject.FindWithTag("Enemy");

        // 2. Prüfe, ob überhaupt ein Gegner existiert
        if (enemyObject != null)
        {
            playerLive = enemyObject.GetComponent<PlayerLive>();
        }
        Destroy(gameObject, lifetime);
    }
    private List<GameObject> enemiesInRange = new List<GameObject>();

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            if (!enemiesInRange.Contains(collision.gameObject))
            {
                enemiesInRange.Add(collision.gameObject);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            enemiesInRange.Remove(collision.gameObject);
        }
    }

    public void CheckHitAtEndOfAnimation()
    {
        // Wir säubern die Liste kurz von Gegnern, die evtl. schon zerstört wurden
        enemiesInRange.RemoveAll(item => item == null);

        // Wenn mindestens ein Gegner in der Liste ist, greifen wir an
        if (enemiesInRange.Count > 0)
        {
            // Wir holen uns den ersten Gegner aus der Liste
            GameObject targetEnemy = enemiesInRange[0];

            // Hier holen wir uns seine Lebens-Komponente (Passe "PlayerLive" ggf. an deinen Skriptnamen an!)
            if (targetEnemy.TryGetComponent<PlayerLive>(out var enemyLive))
            {
                enemyLive.TakeDamage(damage);
            }

            // Cooldown setzen
            PlayerControll.Instance.nextAttackTime = Time.time + attackCooldown;
        }
    }


}
