using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerItemCollector : MonoBehaviour
{
    [Header("UI Cài đặt")]
    public TMP_Text countText; // Kéo dòng Text hiển thị số lượng vào đây
    
    private int truyenDonCount = 0;
    private int maxTruyenDon = 3;
    
    private void Start()
    {
        UpdateUIText();
    }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Item")) return;

        // 🔥 chặn double trigger
        if (!collision.gameObject.activeSelf) return;

        Debug.Log("Nhặt item");

        collision.gameObject.SetActive(false); // 🔥 disable ngay lập tức

        ChiefNPC.collectedLeaflets++;
        UpdateUIText();

        Destroy(collision.gameObject);
    }

    private void UpdateUIText()
    {
        if (countText == null) return;

        string result = "";

        for (int i = 0; i < maxTruyenDon; i++)
        {
            if (i < ChiefNPC.collectedLeaflets)
                result += "[X] ";
            else
                result += "[ ] ";
        }

        countText.text = result;
    }
}
