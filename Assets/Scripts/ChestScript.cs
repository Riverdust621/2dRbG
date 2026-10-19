using UnityEngine;

public class ChestScript : MonoBehaviour,IInteractable
{
    public bool isOpened {get; private set;}
    public string ChestID { get; private set;}
    public GameObject itemPrefab;
    public Sprite openedSprite;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ChestID ??= GlobalHelper.GenerateUniqueID(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public bool CanInteract()
    {
        return !isOpened;
    }

    public void Interact()
    {
        if(!CanInteract()) return;
        OpenChest();

    }
    private void OpenChest()
    {
        SetOpened(true);
        if (itemPrefab)
        {
            GameObject dropItem = Instantiate(itemPrefab, transform.position + Vector3.down, Quaternion.identity );
        }
    }
    public void SetOpened(bool opened)
    {
        
        if (isOpened = opened)
        {
            GetComponent<SpriteRenderer>().sprite = openedSprite;
        }
    }

}
