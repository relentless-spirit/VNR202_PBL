using TMPro;
using UnityEngine;

public class PlayerItemCollector : MonoBehaviour
{
    [Header("UI")]
    public TMP_Text countText;

    [Header("Config")]
    public int maxItem = 3;
    public string itemName = "Truyền đơn";

    [HideInInspector]
    public int currentCount = 0;

    private void Start()
    {
        currentCount = 0;
        UpdateUIText();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Item")) return;

        // 🔥 chặn nhặt quá số lượng
        if (currentCount >= maxItem)
        {
            Debug.Log("Đã đủ " + itemName);
            return;
        }

        Debug.Log("Nhặt " + itemName);

        // 🔥 chống double trigger (nếu collider chồng nhau)
        if (!collision.gameObject.activeSelf) return;
        collision.gameObject.SetActive(false);

        currentCount++;
        UpdateUIText();

        Destroy(collision.gameObject);
    }

    private void UpdateUIText()
    {
        if (countText == null) return;

        string result = "";

        for (int i = 0; i < maxItem; i++)
        {
            result += (i < currentCount) ? "[X] " : "[ ] ";
        }

        countText.text = $"{itemName}: {currentCount}/{maxItem}\n{result}";
    }

    public void ResetItems()
    {
        currentCount = 0;
        UpdateUIText();
    }
}