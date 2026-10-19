
using UnityEngine;

public class PlayerItemCollecter : MonoBehaviour
{
    private InventoryController inventoryController;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        inventoryController = FindAnyObjectByType<InventoryController>();
    }

    // Update is called once per frame
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Item"))
        {
            item item = collision.GetComponent<item>();
            if (item != null)
            {
                
                if (item.name.Contains("Book"))
                {
                    bool itemAddedToHotbar = HotbarController.Instance.AddItem(collision.gameObject);
                    if (itemAddedToHotbar == true)
                    {
                        Destroy(collision.gameObject);
                    }

                }
                else
                {
                    bool itemAddedToInventory = inventoryController.AddItem(collision.gameObject);
                    if (itemAddedToInventory == true)
                    {
                        Destroy(collision.gameObject);
                    }
                }
            }

        }
    }
}
