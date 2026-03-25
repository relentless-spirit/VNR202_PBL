using UnityEngine;

public class ResetMapData : MonoBehaviour
{
    void Start()
    {
        ChiefNPC.collectedLeaflets = 0;

        // nếu có PlayerPrefs thì reset luôn
        PlayerPrefs.DeleteAll();
    }
}