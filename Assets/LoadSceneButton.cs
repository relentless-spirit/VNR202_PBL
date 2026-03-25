using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSceneButton : MonoBehaviour
{
    public string sceneName;

    public void LoadScene()
    {
        ResetData(); // 🔥 reset trước khi load
        SceneManager.LoadScene(sceneName);
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Thoát game");
    }

    void ResetData()
    {
        // 🔥 reset item (Map 1 + Map 2)
        PlayerItemCollector[] collectors = FindObjectsOfType<PlayerItemCollector>();

        foreach (var c in collectors)
        {
            c.ResetItems();
        }

        // 🔥 reset NPC đã nhận thưởng
        QuizNPC[] allNPC = FindObjectsOfType<QuizNPC>();

        foreach (var npc in allNPC)
        {
            PlayerPrefs.DeleteKey(npc.gameObject.name);
        }

        // 🔥 nếu bạn có dùng PlayerPrefs khác
        // PlayerPrefs.DeleteAll(); // (⚠️ dùng nếu muốn reset sạch toàn game)
    }
}