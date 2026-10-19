using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ItemDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
{
    Transform originalParent;
    CanvasGroup canvasGroup;
    private InventoryController inventoryController;

    // Start is called before the first frame update
    void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        inventoryController = InventoryController.Instance;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        originalParent = transform.parent; //Save OG parent
        transform.SetParent(transform.root); //Above other canvas'
        canvasGroup.blocksRaycasts = false;
        canvasGroup.alpha = 0.6f; //Semi-transparent during drag
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position; //Follow the mouse
    }
    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true; //Enables raycasts
        canvasGroup.alpha = 1f; //No longer transparent

        Slot dropSlot = eventData.pointerEnter?.GetComponent<Slot>(); //Slot where item dropped
        if (dropSlot == null)
        {
            GameObject dropItem = eventData.pointerEnter;
            if (dropItem != null)
            {
                dropSlot = dropItem.GetComponentInParent<Slot>();
                QuestController.Instance.CheckInventoryForQuests();
            }
        }
        Slot originalSlot = originalParent.GetComponent<Slot>();

        if (dropSlot == originalSlot)
        {
            transform.SetParent(originalParent);
            GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
            QuestController.Instance.CheckInventoryForQuests();
            return;
        }

        if (dropSlot != null)
        {
            //Is a slot under drop point
            if (dropSlot.currentItem != null)
            {
                item draggedItem = GetComponent<item>();
                item targetItem = dropSlot.currentItem.GetComponent<item>();

                if (draggedItem.ID == targetItem.ID)
                {
                    targetItem.AddToStack(draggedItem.quantity);
                    originalSlot.currentItem = null;
                    Destroy(gameObject);
                    QuestController.Instance.CheckInventoryForQuests();
                }
                else
                {
                    //Slot has an item - swap items
                    dropSlot.currentItem.transform.SetParent(originalSlot.transform);
                    originalSlot.currentItem = dropSlot.currentItem;
                    dropSlot.currentItem.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;

                    transform.SetParent(dropSlot.transform);
                    dropSlot.currentItem = gameObject;
                    GetComponent<RectTransform>().anchoredPosition = Vector2.zero; //Center
                    QuestController.Instance.CheckInventoryForQuests();
                }
            }
            else
            {
                originalSlot.currentItem = null;
                transform.SetParent(dropSlot.transform);
                dropSlot.currentItem = gameObject;
                GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                QuestController.Instance.CheckInventoryForQuests(); //Center
            }
        }
        else
        {

            //Snap back to og slot
            transform.SetParent(originalParent);
            GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
            QuestController.Instance.CheckInventoryForQuests(); //Center

        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if(eventData.button == PointerEventData.InputButton.Right)
        {
            SplitStack();
        }
    }
    private void SplitStack()
    {
        item item = GetComponent<item>();
        if(item == null || item.quantity <= 1) return;
        int spiltamount = item.quantity / 2;
        if (spiltamount <= 0) return;
        item.RemoveFromStack(spiltamount);

        GameObject newitem = item.CloneItem(spiltamount);
        if (inventoryController == null || newitem == null) return;
        foreach (Transform slotTransform in inventoryController.inventoryPanel.transform)
        {
            Slot slot = slotTransform.GetComponent<Slot>();
            if (slot != null  && slot.currentItem == null)
            {
                slot.currentItem = newitem;
                newitem.transform.SetParent(slot.transform);
                newitem.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                return;
            }
        }
        item.AddToStack(spiltamount);
        Destroy(newitem);
        

    }
}
