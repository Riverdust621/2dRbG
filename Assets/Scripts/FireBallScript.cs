using UnityEngine;

public class FireBallScript : MonoBehaviour
{
    public GameObject summener;
    public float attackCooldown = 1f;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    public void ShotMoveing(GameObject projectilesPrefab, float shotRange, Vector2 moveInput, float spawnDistance)
    {
        Vector2 direction;
        if (moveInput != Vector2.zero)
        {
            direction = moveInput.normalized;

        }
        else
        {
            direction = (transform.localScale.x > 0) ? Vector2.right : Vector2.left;

        }
        float winkel = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + 180f;
        Quaternion passendeRotation = Quaternion.Euler(0, 0, winkel);
        Vector2 spawnPosition = (Vector2)summener.transform.position + (direction * spawnDistance);
        // 1. Speichere das instantiierte Objekt in einer Variable (neuesObjekt)
        GameObject neuesObjekt = Instantiate(projectilesPrefab, spawnPosition, passendeRotation);

        // 2. Suche das Skript auf dem NEUEN OBJEKT, nicht auf dem Spieler!
        projectile projectile = neuesObjekt.GetComponent<projectile>();
        PlayerControll.Instance.nextAttackTime = Time.time + attackCooldown;

        if (projectile != null)
        {
            if (direction != Vector2.zero)
            {
                projectile.lifeTime = shotRange;
                projectile.direction = direction;
                Debug.Log($"direction: {direction}");

            }
            Debug.LogWarning("direction is zero");

        }
    }
}
