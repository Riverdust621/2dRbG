using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueController : MonoBehaviour
{

    public static DialogueController Instance { get; private set; }
    public GameObject dialoguePanel;
    public TMP_Text dialogText, nameText;
    public Image portraitImage;
    public Transform choiceContainer;
    public GameObject choiceButtonPrefab;
    public GameObject player;
    private PlayerControll playerControll;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        if (player != null) playerControll = player.GetComponent<PlayerControll>();
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void showDialogueUi(bool show)
    {
        dialoguePanel.SetActive(show);
    }
    public void SetNPCInfo(string npcName, Sprite portrait)
    {
        nameText.text = npcName;
        portraitImage.sprite = portrait;
    }
    public void SetDialogueText(string text)
    {
        dialogText.text = text;
    }


    public void ClearChoices()
    {
        foreach (Transform child in choiceContainer)
        {
            Destroy(child.gameObject);
        }
    }
    public void CreateChoiceButton(string choiceText, UnityEngine.Events.UnityAction onClick)
    {
        GameObject choiceButton = Instantiate(choiceButtonPrefab, choiceContainer);
        choiceButton.GetComponentInChildren<TMP_Text>().text = choiceText;
        choiceButton.GetComponent<Button>().onClick.AddListener(onClick);
    }
    public void SetPlayerDirection(Vector3 npcTransform)
    {
        Vector3 rawDirection = player.transform.position - npcTransform;
        Vector3 direction = Vector3.zero;

        // Ermittle die absolute Größe jeder Achse
        float absX = Mathf.Abs(rawDirection.x);
        float absY = Mathf.Abs(rawDirection.y);
        float absZ = Mathf.Abs(rawDirection.z);

        // Finde die Achse mit dem größten Ausschlag und setze sie auf 1 oder -1
        if (absX > absY && absX > absZ)
        {
            direction.x = -Mathf.Sign(rawDirection.x);
        }
        else if (absY > absX && absY > absZ)
        {
            direction.y = -Mathf.Sign(rawDirection.y);
        }
        else if (absZ > absX && absZ > absY)
        {
            direction.z = Mathf.Sign(rawDirection.z);
        }

        playerControll.SetPlayerAnimator(0.2f, direction.x, direction.y);
    }
}
