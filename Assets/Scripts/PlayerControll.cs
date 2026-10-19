using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControll : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator animator;
    public float speed = 5f;
    public GameObject player;
    private Vector2 moveInput;
    private HealthBar healthBar;
    private MenuController menuController;
    public GameObject Lightning;
    public GameObject explotion;
    public static PlayerControll Instance { get; private set; }

    private InputControll controls;
    private float teleportDistance = 4f;
    public bool isTeleportingNow;
    private float spawnDistance = 0.5f;
    public GameObject projectilesPrefab;
    public float shotRange = 2f;
    public Vector2 direction;
    public float health = 5f;
    public string activeSpell;
    private LightningSpell lightningSpell;
    private FireBallScript fireBallScript;
    private NPC nPCScript;
    public float nextAttackTime;
    public float attackCooldown = 1f;
    public item activeItem;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        //Gives the variables some kind of use
        healthBar = GameObject.FindGameObjectWithTag("HealthBar").GetComponent<HealthBar>();
        menuController = GameObject.FindGameObjectWithTag("Ui").GetComponent<MenuController>();
        lightningSpell = GameObject.FindGameObjectWithTag("GameController").GetComponent<LightningSpell>();
        fireBallScript = GameObject.FindGameObjectWithTag("GameController").GetComponent<FireBallScript>();

        controls = new InputControll();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        healthBar.SetMaxHealth(health);
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    void OnEnable()
    {
        Debug.Log("Input System: Teleport Action wurde abonniert.");
        controls.Player.Enable();
        controls.Player.Teleport.performed += OnTeleport;
        controls.Player.Shot.performed += OnShot;
    }
    /////////////
    //Updates////
    /////////////

    void Update()
    {
        moveInput = controls.Player.Move.ReadValue<Vector2>();
        GameObject Npc = GameObject.FindGameObjectWithTag("NPC");
        if (Npc != null)
        {
            nPCScript = Npc.GetComponent<NPC>();
        }
        if(!nPCScript.isDialogActive || nPCScript == null)
        {
            SetPlayerAnimator(moveInput.sqrMagnitude, moveInput.x, moveInput.y);
        }
        
        
    }
    void FixedUpdate()
    {
        GameObject Npc = GameObject.FindGameObjectWithTag("NPC");
        if (Npc != null)
        {
            nPCScript = Npc.GetComponent<NPC>();
        }
        if (!isTeleportingNow && (!nPCScript.isDialogActive || nPCScript == null))
        {
            rb.MovePosition(rb.position + moveInput.normalized * speed * Time.fixedDeltaTime);
        }
            
    }
    /////////////////////
    ///NormalFunktions//
    ///////////////////

    private void OnTeleport(InputAction.CallbackContext context)
    {
        //checks for an Keyboard press
        Debug.Log("F presst");
        Teleport();

    }
    private void OnShot(InputAction.CallbackContext context)
    {
        GameObject Npc = GameObject.FindGameObjectWithTag("NPC");
        if (Npc != null)
        {
            nPCScript = Npc.GetComponent<NPC>();
        }
        if (!menuController.isInMenu && (!nPCScript.isDialogActive || nPCScript == null) && Time.time >= nextAttackTime)
        {
            if (activeSpell == "LightningBook")
            {
                lightningSpell.ShotTarget(Lightning);
                activeItem.RemoveBookUse();

            }
            //ShotMoveing(projectilesPrefab, shotRange);
            if (activeSpell == "FireBook")
            {
                fireBallScript.ShotMoveing(projectilesPrefab, 3f, moveInput, spawnDistance);
                activeItem.RemoveBookUse();
            }
            if (activeSpell == "Explosion Book")
            {
                lightningSpell.ShotTarget(explotion);
                activeItem.RemoveBookUse();
            }

        }

    }
    public void Teleport()
    {
        //Start the teleport is no telport is already used
        if (isTeleportingNow) return;

        StartCoroutine(TeleportAnimationRoutine());
    }
    public void ShotMoveing(GameObject projectilesPrefab, float shotRange)
    {
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
        Vector2 spawnPosition = (Vector2)transform.position + (direction * spawnDistance);
        // 1. Speichere das instantiierte Objekt in einer Variable (neuesObjekt)
        GameObject neuesObjekt = Instantiate(projectilesPrefab, spawnPosition, passendeRotation);

        // 2. Suche das Skript auf dem NEUEN OBJEKT, nicht auf dem Spieler!
        projectile projectile = neuesObjekt.GetComponent<projectile>();
        ;

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



    System.Collections.IEnumerator TeleportAnimationRoutine()
    {
        isTeleportingNow = true;
        rb.bodyType = RigidbodyType2D.Kinematic;
        // 1. Animation starten
        SetPlayerAnimator(0f , moveInput.x, moveInput.y);
        animator.SetBool("isTeleporting", true);
        yield return new WaitForSeconds(1f);


        // 2. Bild unsichtbar machen (falls gewünscht, sonst diese Zeilen löschen)
        SpriteRenderer spriteRenderer = player.GetComponent<SpriteRenderer>();
        if (spriteRenderer != null) spriteRenderer.enabled = false;

        // 3. Warte 1 Sekunde, während die Teleport-Animation läuft
        yield return new WaitForSeconds(0.1f);

        // 4. Erst JETZT nach der Wartezeit die Position verändern!
        Vector2 direction = moveInput.normalized;
        if (direction == Vector2.zero)
        {
            direction = (transform.localScale.x > 0) ? Vector2.right : Vector2.left;
        }
        Vector2 desiredTarget = rb.position + direction * teleportDistance;

        // 2. Schieße einen unsichtbaren Laserstrahl von der aktuellen Position zum Ziel
        // Wichtig: Nutze eine LayerMask für deine Wände/Böden, damit der Strahl nicht den Spieler selbst trifft!
        int wallLayerMask = LayerMask.GetMask("Ground", "Wall"); // Passe die Namen deiner Layer an!

        RaycastHit2D hit = Physics2D.Linecast(rb.position, desiredTarget, wallLayerMask);

        Vector2 finalTargetPosition;

        if (hit.collider != null)
        {
            // Wenn der Strahl eine Wand trifft, setze die Zielposition auf den Aufprallpunkt.
            // Wir ziehen ein kleines Stück ab (z.B. direction * 0.5f), damit der Spieler nicht IN der Wand stecken bleibt.
            finalTargetPosition = hit.point - (direction * 0.5f);
            Debug.Log($"Wand blockiert! Teleport verkürzt auf: {finalTargetPosition}");
        }
        else
        {
            // Wenn der Weg frei ist, nimm die volle Teleport-Distanz
            finalTargetPosition = desiredTarget;
        }

        // 3. Die Position sicher zuweisen
        rb.position = finalTargetPosition;

        Debug.Log($"Teleport erfolgreich auf Position: {finalTargetPosition}");

        // 5. Animation beenden & Bild wieder einschalten
        animator.SetBool("isTeleporting", false);
        animator.SetBool("isTeleporting", true);
        if (spriteRenderer != null) spriteRenderer.enabled = true;
        rb.bodyType = RigidbodyType2D.Dynamic;




        // Falls du danach noch einmal warten willst:
        yield return new WaitForSeconds(0.5f);
        animator.SetBool("isTeleporting", false);
        isTeleportingNow = false;
    }
    public void SetPlayerAnimator(float speed, float xdirection, float ydirection)
    {
        animator.SetFloat("Horizontal", xdirection);
        animator.SetFloat("Vertical", ydirection);
        animator.SetFloat("Speed", speed);
    }
    
     
    
    
}
