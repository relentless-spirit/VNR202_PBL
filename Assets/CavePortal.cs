using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class CavePortal : MonoBehaviour
{
    [Header("Scene")]
    public string nextSceneName;

    [Header("Requirement")]
    public int requiredItem = 3;
    public string itemName = "vật phẩm";

    [Header("UI Notify")]
    public GameObject notifyPanel;
    public TMP_Text notifyText;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        // 🔥 lấy collector từ player
        PlayerItemCollector collector = collision.GetComponent<PlayerItemCollector>();

        if (collector == null)
        {
            Debug.LogError("Player chưa có PlayerItemCollector!");
            return;
        }

        if (collector.currentCount < requiredItem)
        {
            ShowNotify($"Bạn cần {requiredItem} {itemName}!");
            return;
        }

        ShowNotify("Đang chuyển khu vực...");
        Invoke(nameof(LoadScene), 1f);
    }

    void LoadScene()
    {
        SceneManager.LoadScene(nextSceneName);
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