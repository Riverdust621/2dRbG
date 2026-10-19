using UnityEngine;

public class projectile : MonoBehaviour
{
    public Vector2 direction;
    private Rigidbody2D proRb;
    public float speed;
    public float lifeTime;
    private PlayerLive playerLive;
    public int damage;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        proRb = GetComponent<Rigidbody2D>();
    }
    void Start()
    {
     
        Destroy(gameObject, lifeTime);

    }

    // Update is called once per frame
    void FixedUpdate()
    {

        proRb.MovePosition(proRb.position + direction * speed * Time.fixedDeltaTime);
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Obstacles"))
        {
            Destroy(gameObject);
        }
        if (collision.gameObject.CompareTag("Enemy"))
        {
            playerLive = collision.gameObject.GetComponent<PlayerLive>();
            playerLive.TakeDamage(damage);
            Destroy(gameObject);
        }
    }


}
