using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class CavePortal : MonoBehaviour
{
    public string nextSceneName = "Map2";
    public int requiredLeaflets = 3;

    [Header("UI Notify")]
    public GameObject notifyPanel;   // cả khung (ảnh)
    public TMP_Text notifyText;      // chữ bên trong

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        if (ChiefNPC.collectedLeaflets < requiredLeaflets)
        {
            ShowNotify("Bạn cần 3 truyền đơn để vào hang!");
            return;
        }

        ShowNotify("Đang tiến vào hang...");
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