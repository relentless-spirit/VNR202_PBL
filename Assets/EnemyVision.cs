using UnityEngine;

public class EnemyVision : MonoBehaviour
{
    public Transform enemy;
    public float viewDistance = 5f;
    public LayerMask obstacleLayer;

    [Header("Respawn")]
    public Transform respawnPoint;

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        Vector2 direction = (collision.transform.position - enemy.position);

        // 🔥 check không bị che
        RaycastHit2D hit = Physics2D.Raycast(
            enemy.position,
            direction.normalized,
            viewDistance,
            obstacleLayer
        );

        if (hit.collider == null)
        {
            PlayerCaught(collision);
        }
    }

    void PlayerCaught(Collider2D player)
    {
        Debug.Log("Bị bắt!");

        // =========================
        // 🔥 RESET ITEM
        // =========================
        PlayerItemCollector collector = player.GetComponent<PlayerItemCollector>();

        if (collector != null)
        {
            collector.ResetItems();
        }

        // =========================
        // 🔥 RESET NPC
        // =========================
        QuizNPC[] allNPC = FindObjectsOfType<QuizNPC>();

        foreach (var npc in allNPC)
        {
            PlayerPrefs.DeleteKey(npc.gameObject.name);
        }

        // =========================
        // 🔥 TELEPORT PLAYER
        // =========================
        player.transform.position = respawnPoint.position;
    }
}