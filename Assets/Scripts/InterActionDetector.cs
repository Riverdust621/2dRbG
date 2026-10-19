
using UnityEngine;
using UnityEngine.InputSystem;

public class InterActionDetector : MonoBehaviour
{
    private IInteractable interactableInRange = null;
    public GameObject interactionIcon;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        interactionIcon.SetActive(false);
    }
    public void OnInteract(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            // Prüfen, ob überhaupt ein Objekt in Reichweite ist
            if (interactableInRange != null)
            {
                interactableInRange.Interact();

                if (!interactableInRange.CanInteract())
                {
                    interactionIcon.SetActive(false);
                }
            }
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out IInteractable interactable) && interactable.CanInteract())
        {
            interactableInRange = interactable;
            interactionIcon.SetActive(true);
        }
    }
    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out IInteractable interactable) && interactable == interactableInRange)
        {
            interactableInRange = null;
            interactionIcon.SetActive(false);
        }
    }
    
}
