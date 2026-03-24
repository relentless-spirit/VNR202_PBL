using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

[System.Serializable]
public class QuestionAnswers
{
    public string question;
    public string[] answers;
    public int correctAnswerIndex;
}

public class QuizNPC : MonoBehaviour
{
    [Header("Dữ liệu Câu hỏi")]
    public List<QuestionAnswers> qnA;

    [Header("Phần thưởng")]
    public GameObject[] rewardItems;

    [Header("UI")]
    public GameObject quizPanel;
    public TMP_Text nameText;
    public TMP_Text questionText;

    public Transform choicesLeft;
    public Transform choicesRight;

    public GameObject buttonPrefab;

    private bool isPlayerInRange = false;
    private bool isQuizActive = false;

    private QuestionAnswers currentQuestion;

    private int correctCount = 0;
    private int questionCount = 0;
    private List<QuestionAnswers> remainingQuestions;

    private int maxQuestions = 5;
    private int requiredCorrect = 3;
    
    private bool hasGivenReward = false;
    
    // =========================
    // UPDATE
    // =========================
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (!isPlayerInRange) return;

            if (!isQuizActive)
                OpenQuiz();
            else
                CloseQuiz();
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            PlayerPrefs.DeleteAll();
            Debug.Log("Reset Save");
        }
    }

    // =========================
    // CHECK ĐÃ NHẬN CHƯA
    // =========================
    bool HasReceivedReward()
    {
        return PlayerPrefs.GetInt(gameObject.name, 0) == 1;
    }

    // =========================
    // MỞ QUIZ
    // =========================
    void OpenQuiz()
    {
        correctCount = 0;
        questionCount = 0;
        remainingQuestions = new List<QuestionAnswers>(qnA);

        quizPanel.SetActive(true);
        isQuizActive = true;

        if (ChiefNPC.Instance == null || !ChiefNPC.Instance.hasGivenQuest)
        {
            ClearButtons();
            questionText.text = "Hãy đi gặp Trưởng làng!";
            return;
        }

        if (HasReceivedReward())
        {
            ClearButtons();
            questionText.text = "Bạn đã nhận vật phẩm từ tôi rồi!";
            return;
        }

        nameText.text = "Liên Lạc Viên";

        AskQuestion();
    }

    // =========================
    // HIỂN THỊ CÂU HỎI
    // =========================
    void AskQuestion()
    {
        ClearButtons();

        if (remainingQuestions.Count == 0 || questionCount >= maxQuestions)
        {
            EndQuiz();
            return;
        }

        int randomIndex = Random.Range(0, remainingQuestions.Count);
        currentQuestion = remainingQuestions[randomIndex];
        remainingQuestions.RemoveAt(randomIndex);

        questionText.text = currentQuestion.question;

        for (int i = 0; i < currentQuestion.answers.Length; i++)
        {
            int index = i;

            // 🔥 chia trái phải
            Transform parent = (i % 2 == 0) ? choicesLeft : choicesRight;

            GameObject btn = Instantiate(buttonPrefab, parent);

            TMP_Text btnText = btn.GetComponentInChildren<TMP_Text>();
            if (btnText != null)
                btnText.text = currentQuestion.answers[i];

            Button btnComponent = btn.GetComponent<Button>();
            if (btnComponent != null)
                btnComponent.onClick.AddListener(() => CheckAnswer(index));
        }
    }

    // =========================
    // CHECK ĐÁP ÁN
    // =========================
    void CheckAnswer(int selectedIndex)
    {
        ClearButtons();
        questionCount++;

        if (selectedIndex == currentQuestion.correctAnswerIndex)
        {
            correctCount++;
            questionText.text = "Đúng!";
        }
        else
        {
            questionText.text = "Sai!";
        }

        CancelInvoke();
        Invoke(nameof(NextStep), 1.2f);
    }

    void NextStep()
    {
        if (questionCount >= maxQuestions)
            EndQuiz();
        else
            AskQuestion();
    }

    // =========================
    // KẾT THÚC QUIZ
    // =========================
    void EndQuiz()
    {
        ClearButtons();

        if (hasGivenReward) return; // 🔥 CHẶN DOUBLE

        if (correctCount >= requiredCorrect)
        {
            questionText.text = $"Hoàn thành! ({correctCount}/5)\nNhận vật phẩm!";

            GiveReward();

            hasGivenReward = true; 
            PlayerPrefs.SetInt(gameObject.name, 1);
        }
        else
        {
            questionText.text = $"Thất bại ({correctCount}/5)\nNhấn Q để thử lại!";
        }
    }

    // =========================
    // SPAWN ITEM
    // =========================
    void GiveReward()
    {
        Debug.Log("Spawn từ NPC: " + gameObject.name); // 👈 đặt ở đây

        if (rewardItems == null || rewardItems.Length == 0) return;

        int rand = Random.Range(0, rewardItems.Length);
        Instantiate(rewardItems[rand], transform.position + Vector3.down, Quaternion.identity);
    }

    // =========================
    void CloseQuiz()
    {
        quizPanel.SetActive(false);
        isQuizActive = false;
    }

    void ClearButtons()
    {
        foreach (Transform child in choicesLeft)
            Destroy(child.gameObject);

        foreach (Transform child in choicesRight)
            Destroy(child.gameObject);
    }

    // =========================
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
            isPlayerInRange = true;
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerInRange = false;
            CloseQuiz();
        }
    }
    
    public void ResetGame()
    {
        PlayerPrefs.DeleteAll();
    }
}