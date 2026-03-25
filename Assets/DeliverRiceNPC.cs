using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class DeliverRiceNPC : MonoBehaviour
{
    public int requiredRice = 3;
    public string nextScene = "Map4";

    [Header("UI")]
    public GameObject notifyPanel;
    public TMP_Text notifyText;

    private bool isTriggered = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isTriggered) return;
        if (!collision.CompareTag("Player")) return;

        PlayerItemCollector collector = collision.GetComponent<PlayerItemCollector>();

        if (collector == null) return;

        if (collector.currentCount < requiredRice)
        {
            ShowNotify("Bạn chưa đủ bao gạo!");
            return;
        }

        // 🔥 ĐỦ ĐIỀU KIỆN
        isTriggered = true;

        ShowNotify("Hoàn thành nhiệm vụ! Đang chuyển đi...");

        Invoke(nameof(LoadNextScene), 1.5f);
    }

    void LoadNextScene()
    {
        SceneManager.LoadScene(nextScene);
    }

    void ShowNotify(string message)
    {
        if (notifyPanel == null || notifyText == null) return;

        notifyPanel.SetActive(true);
        notifyText.text = message;

        CancelInvoke(nameof(HideNotify));
        Invoke(nameof(HideNotify), 2f);
    }

    void HideNotify()
    {
        notifyPanel.SetActive(false);
    }
}