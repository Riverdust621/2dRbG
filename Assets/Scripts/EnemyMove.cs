using System.Threading;
using NUnit.Framework;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyMove : MonoBehaviour
{
    public float speed;
    public GameObject player;
    private Rigidbody2D rb2d;
    private Animator animator;
    private float maxRange = 5f;
    public LayerMask visibleLayers;
    private float nextAttackTime = 0f;
    public float attackRange = 2f;
    private bool isAttacking = false;
    // Ändere das von GameObject zu PlayerLive:
    private PlayerLive playerLive;
    private MenuController menuController;
    private NPC NPCState;


    private float attackCooldown = 1.5f;
    private HealthBar healthBar;
    private PlayerControll playerControll;
    private LighingDestroy lighingDestroy;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        healthBar = GameObject.FindGameObjectWithTag("HealthBar").GetComponent<HealthBar>();
        menuController = GameObject.FindGameObjectWithTag("Ui").GetComponent<MenuController>();
        playerLive = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerLive>();

        playerLive.SetLife();
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        EnemyMovement(speed);
    }
    public bool PlayerInRange(float maxRange)
    {
        Vector2 direction = player.transform.position - transform.position;
        RaycastHit2D hitInfo = Physics2D.Raycast(transform.position, direction.normalized, maxRange, visibleLayers);
        if (hitInfo.collider != null)
        {
            if (hitInfo.collider.gameObject.CompareTag("Player"))
            {
                return true;

            }
            else
            {
                return false;
            }

        }
        else
        {
            return false;
        }
    }
    private void EnemyMovement(float speed)
    {
        


        player = GameObject.FindWithTag("Player");

        if (player != null)
        {

            playerLive = player.GetComponent<PlayerLive>();
            playerControll = player.GetComponent<PlayerControll>();
        }
        GameObject NPC = GameObject.FindGameObjectWithTag("NPC");
        if (NPC != null)
        {
            NPCState = NPC.GetComponent<NPC>();

        }
        else
        {
            NPCState = null;
        }

        // 2. Nur wenn das Objekt in der Szene existiert, holen wir die Komponente


        if (player != null && playerControll != null && !playerControll.isTeleportingNow && !menuController.isInMenu &&  (NPCState == null || !NPCState.isDialogActive))
        {

            bool playerInRange = PlayerInRange(maxRange);
            // 1. Richtung berechnen und normalisieren
            if (playerInRange == true)
            {



                // 3. Distanz prüfen (Verhindert das "Zittern" direkt auf dem Spieler)
                float distanceToPlayer = Vector2.Distance(transform.position, player.transform.position);

                if (distanceToPlayer > attackRange) // Er läuft nur, wenn er weiter als 0.5 Einheiten weg ist
                {
                    Vector2 direction = player.transform.position - transform.position;

                    // 2. Bewegung in Unity 6 zuweisen
                    rb2d.linearVelocity = direction * speed;
                    animator.SetBool("isWalking", true);

                    // Hier nutzen wir direkt "direction" statt "linearVelocity"
                    // Das ist immun gegen Physik-Fehler!
                    if (Mathf.Abs(direction.y) > Mathf.Abs(direction.x))
                    {
                        animator.SetFloat("moveX", 0f);
                        animator.SetFloat("moveY", direction.y > 0 ? 1f : -1f);
                    }
                    else
                    {
                        animator.SetFloat("moveY", 0f);
                        animator.SetFloat("moveX", direction.x > 0 ? 1f : -1f);
                    }
                }
                else
                {


                    if (Time.time >= nextAttackTime)
                    {

                        StartAttack();


                    }
                    // Wenn er nah genug dran ist, stoppt er die Bewegung und die Animation

                }
            }
            else
            {
                rb2d.linearVelocity = Vector2.zero;
                animator.SetBool("isWalking", false);
            }
        }
        else
        {
            float distanceToPlayer = Vector2.Distance(transform.position, player.transform.position);

            if (distanceToPlayer < attackRange)
            {
                StartAttack();
            }
            rb2d.linearVelocity = Vector2.zero;
            animator.SetBool("isWalking", false);
        }


    }

    // Startet die Coroutine, die im Hintergrund mit Verzögerung läuft


    System.Collections.IEnumerator AttackRoutine()
    {
        animator.SetBool("isAttacking", true);
        rb2d.linearVelocity = Vector2.zero; // Während des Stichs nicht bewegen
        animator.SetBool("isWalking", false);

        // Wartet 0.1 Sekunden (100 Millisekunden), ohne das Spiel einzufrieren
        yield return new WaitForSeconds(1f);

        // Berechnet die nächste Angriffszeit basierend auf der aktuellen Spielzeit
        nextAttackTime = Time.time + attackCooldown;

        animator.SetBool("isAttacking", false);
    }

    void StartAttack()
    {
        StartCoroutine(AttackRoutine());


    }
    public void CheckHitAtEndOfAnimation()
    {
        if (player != null)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, player.transform.position);

            // Prüft am Ende der Animation, ob der Spieler noch in Reichweite ist
            if (distanceToPlayer <= attackRange)
            {
                Debug.Log("Spieler wurde getroffen!");
                // Hier dein Schadens-Code, z.B.:
                float damage = 1f;
                playerLive.TakeDamage(damage);
                healthBar.SetHealth(healthBar.slider.value - damage);
            }
            else
            {
                Debug.Log("Angriff ging ins Leere, Spieler ist ausgewichen!");
            }
        }

        // Angriff beendet, Gegner darf wieder laufen
        isAttacking = false;
    }
}
