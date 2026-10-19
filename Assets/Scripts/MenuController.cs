using UnityEngine;
using UnityEngine.InputSystem;


public class MenuController : MonoBehaviour
{
    public bool isInMenu = false;
    public GameObject menuCanvas;
    private InputControll controls;
    public GameObject lifeBar;

    void Awake()
    {
        controls = new InputControll();
    }

    void Start()
    {
        menuCanvas.SetActive(false);
        lifeBar.SetActive(true);
    }

    void OnEnable()
    {
        // 2. Deine "UI Actions" Action-Map aktivieren
        controls.Ui.Enable();

        // 3. Auf den Knopfdruck der "MenuControll"-Action hören
        controls.Ui.MenuControll.performed += ToggleMenu;
    }

    void OnDisable()
    {
        // 4. Immer sauber abmelden, wenn das Objekt deaktiviert wird
        controls.Ui.MenuControll.performed -= ToggleMenu;
        controls.Ui.Disable();
    }

    // 5. Diese Methode wird jetzt automatisch aufgerufen, sobald du TAB drückst!
    private void ToggleMenu(InputAction.CallbackContext context)
    {
        isInMenu = !isInMenu;
        lifeBar.SetActive(!lifeBar.activeSelf);
        menuCanvas.SetActive(!menuCanvas.activeSelf);
    }
}