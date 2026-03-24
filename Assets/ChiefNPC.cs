using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ChiefNPC : MonoBehaviour, IInteractable
{
    // ===== QUEST STATE =====
    public static ChiefNPC Instance;
    public bool hasGivenQuest = false;
    public static int collectedLeaflets = 0;
    public static int requiredLeaflets = 3; // số lượng cần thu

    void Awake()
    {
        Instance = this;
    }
    
    [Header("UI")]
    public GameObject dialoguePanel;
    public TMP_Text dialogueText, nameText;
    public Transform choicesPanel;
    public GameObject choiceButtonPrefab;

    private bool isDialogueActive;

    public void Interact()
    {
        Debug.Log("Interact CALLED");

        if (isDialogueActive) return;

        isDialogueActive = true;
        dialoguePanel.SetActive(true);
        nameText.text = "Trưởng làng";

        Debug.Log("Before: " + hasGivenQuest);

        if (!hasGivenQuest)
        {
            hasGivenQuest = true;
            Debug.Log("SET TRUE!");
        }

        Debug.Log("After: " + hasGivenQuest);
    }

    void StartQuest()
    {
        dialogueText.text =
            "Tình hình rất nguy cấp! Hãy đi tìm các đồng đội và trả lời mật khẩu để nhận truyền đơn!";
        
        hasGivenQuest = true;
        collectedLeaflets = 0;
    }

    void ProgressDialogue()
    {
        dialogueText.text =
            $"Đồng chí đã thu được {collectedLeaflets}/{requiredLeaflets} truyền đơn.\nHãy tiếp tục!";
    }

    void CompleteQuest()
    {
        dialogueText.text =
            "Xuất sắc! Đồng chí đã hoàn thành nhiệm vụ. Chúng ta sẽ phản công!";

        // Reset hoặc mở quest tiếp theo
        hasGivenQuest = false;
    }

    public bool CanInteract()
    {
        return !isDialogueActive;
    }

    void ClearChoices()
    {
        foreach (Transform child in choicesPanel)
            Destroy(child.gameObject);
    }

    void CreateCloseButton()
    {
        GameObject btnObj = Instantiate(choiceButtonPrefab, choicesPanel);

        TMP_Text txt = btnObj.GetComponentInChildren<TMP_Text>();
        if (txt != null)
            txt.text = "Rõ, thưa đồng chí!";

        Button btn = btnObj.GetComponent<Button>();
        if (btn != null)
        {
            btn.onClick.AddListener(() =>
            {
                isDialogueActive = false;
                dialoguePanel.SetActive(false);
                ClearChoices();
            });
        }
    }
}