using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyResetPoint : MonoBehaviour
{
    public Transform respawnPoint; // kéo vị trí spawn vào

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        Debug.Log("Bị lính bắt!");

        // 🔥 dịch chuyển player về đầu map
        collision.transform.position = respawnPoint.position;
    }
}
