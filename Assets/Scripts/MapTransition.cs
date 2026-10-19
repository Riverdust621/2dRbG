using Unity.Cinemachine;
using UnityEngine;

public class MapTransition : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] PolygonCollider2D mapBoundry;
    [SerializeField] Direction direction;
    CinemachineConfiner2D confiner2D;
    enum Direction { Up, Down, Left, Right }
    private void Awake()
    {
        confiner2D = FindAnyObjectByType<CinemachineConfiner2D>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            confiner2D.BoundingShape2D = mapBoundry;
            UpdatePlayersPosition(collision.gameObject);
        }
    }
    private void UpdatePlayersPosition(GameObject player)
    {
        player.transform.position += direction switch
        {
            Direction.Up => new Vector3(0, 2, 0),
            Direction.Down => new Vector3(0, -2, 0),
            Direction.Left => new Vector3(2, 0, 0),  // Behält deine Logik (+2 bei Left) bei
            Direction.Right => new Vector3(-2, 0, 0), // Behält deine Logik (-2 bei Right) bei
            _ => Vector3.zero
        };

    }
}
