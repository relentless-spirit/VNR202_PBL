using UnityEngine;

public class PauseController : MonoBehaviour
{
    public static bool IsGamePaused = false;

    public static void SetPaused(bool isPaused)
    {
        IsGamePaused = isPaused;
    
        // Lệnh này giúp đóng băng hoàn toàn thời gian trong game khi nói chuyện
        if (isPaused) {
            Time.timeScale = 0f; 
        } else {
            Time.timeScale = 1f;
        }
    }
}
