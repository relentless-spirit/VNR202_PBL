using UnityEngine;

public class Teleport : MonoBehaviour
{
    [Header("Điểm dịch chuyển tới")]
    public Transform destination;

    [Header("Option")]
    public bool useOffset = true;
    public Vector3 offset = new Vector3(0, 1f, 0);

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        if (destination == null)
        {
            Debug.LogError("Chưa gán destination!");
            return;
        }

        // 🔥 teleport
        if (useOffset)
            collision.transform.position = destination.position + offset;
        else
            collision.transform.position = destination.position;
    }
}