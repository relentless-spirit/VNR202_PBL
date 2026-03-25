using UnityEngine;

public class PopupUI : MonoBehaviour
{
    public GameObject popupPanel;

    public void ShowPopup()
    {
        popupPanel.SetActive(true);
    }

    public void HidePopup()
    {
        popupPanel.SetActive(false);
    }

    public void TogglePopup()
    {
        popupPanel.SetActive(!popupPanel.activeSelf);
    }
}