
using UnityEngine;

public class SlimeControll : MonoBehaviour
{

    public float speed;
    public GameObject player;
    private Rigidbody2D rb2d;
    private Animator animator;
    public float maxRange = 5f;
    public LayerMask visibleLayers;
    // Ändere das von GameObject zu PlayerLive:
    private PlayerLive playerLive;
    private MenuController menuController;
    private NPC NPCState;
    private HealthBar healthBar;
    private PlayerControll playerControll;
    private LighingDestroy lighingDestroy;
    private  float nextAttackTime;
    public float attackCooldown = 1f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        IntComponents();
        playerLive.SetLife();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        SlimeMovement(speed);
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
    private void SlimeMovement(float speed)
    {
        CheckForNPCs();
        CheckForPlayer();
        if ( !playerControll.isTeleportingNow && !menuController.isInMenu && (NPCState == null || !NPCState.isDialogActive))
        {
            


            bool playerInRange = PlayerInRange(maxRange);
            // 1. Richtung berechnen und normalisieren
            if (playerInRange == true)
            {
                
                Vector2 direction = player.transform.position - transform.position;
                rb2d.linearVelocity = direction * speed;
                SetSlimeAnimator(true, false);
                
            }
            else
            {
                rb2d.linearVelocity = Vector2.zero;
                SetSlimeAnimator(false, false);
            }
        }
    }

    public void SlimeAttack(float damage)
    {
        playerLive.TakeDamage(damage);
        healthBar.SetHealth(healthBar.slider.value - damage);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(Time.time >= nextAttackTime)
        {
            playerLive.TakeDamage(1);
            nextAttackTime = Time.time + attackCooldown;
        }
    }
    private void CheckForPlayer()
    {
        player = GameObject.FindWithTag("Player");

        if (player != null)
        {
            playerLive = player.GetComponent<PlayerLive>();
            playerControll = player.GetComponent<PlayerControll>();
            
        }
        else
        {
            player = null;
        }
    }
    private void CheckForNPCs()
    {
        GameObject NPC = GameObject.FindGameObjectWithTag("NPC");
        if (NPC != null)
        {
            NPCState = NPC.GetComponent<NPC>();
            

        }
        else
        {
            NPCState = null;
            
        }
    }
    public void SetSlimeAnimator(bool isJumping, bool isHurt)
    {
        animator.SetBool("IsJumping", isJumping);
        animator.SetBool("SlimeIsHittet", isHurt);
    }
    private void IntComponents()
    {
        rb2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        healthBar = GameObject.FindGameObjectWithTag("HealthBar").GetComponent<HealthBar>();
        menuController = GameObject.FindGameObjectWithTag("Ui").GetComponent<MenuController>();
        playerLive = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerLive>();
    }
}
