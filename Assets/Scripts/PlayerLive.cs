using System;
using UnityEngine;

public class PlayerLive : MonoBehaviour
{
    public float playerlife;
    private HealthBar healthBar;
    public static event Action OnKillChanged;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        healthBar = GameObject.FindGameObjectWithTag("HealthBar").GetComponent<HealthBar>();
    }
    public void TakeDamage(float damage)
    {
        playerlife -= damage;
        if (playerlife <= 0)
        {
            Debug.Log(damage);
            if (gameObject.CompareTag("Enemy"))
            {
                enemyid enemyid = gameObject.GetComponent<enemyid>();
                QuestController.Instance.GetLastEnemyId(enemyid.ID);
                OnKillChanged?.Invoke();

            }
            Destroy(gameObject);
            
        }
        
    }
    public void SetLife()
    {
        playerlife = healthBar.slider.maxValue;
    }


}
