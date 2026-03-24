using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemPickupUIController : MonoBehaviour
{
    public static ItemPickupUIController Instance { get; private set; }
    
    public GameObject popupPrefab; // Prefab của UI hiển thị khi nhặt item
    public int maxPopupCount = 5; // Số lượng popup tối đa hiển thị cùng lúc
    public float popupDuration = 3f;

    private readonly Queue<GameObject> activePopups = new();
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogError("Multiple instances of PickupUIController detected. Destroying duplicate.");
            Destroy(gameObject);
        }
    }

    public void ShowItemPickup(string itemName, Sprite itemSprite)
    {
        GameObject newPopup = Instantiate(popupPrefab, transform);
        newPopup.GetComponentInChildren<TMP_Text>().text = itemName;
        
        Image itemImage = newPopup.GetComponentInChildren<Image>();
        if (itemImage != null)
        {
            itemImage.sprite = itemSprite;
        }
        
        activePopups.Enqueue(newPopup);
        if (activePopups.Count > maxPopupCount)
        {
            Destroy(activePopups.Dequeue());
        }
        
        //fade out sau một khoảng thời gian
        StartCoroutine(FadeOutAndDestroy(newPopup));
    }
    
    private IEnumerator FadeOutAndDestroy(GameObject popup)
    {
        yield return new WaitForSeconds(popupDuration);
        if (popup == null) yield break; // Nếu popup đã bị hủy, dừng coroutine
        
        CanvasGroup canvasGroup = popup.GetComponent<CanvasGroup>();
        for (float timePassed = 0f; timePassed < 1f; timePassed += Time.deltaTime / popupDuration)
        {
            if (canvasGroup != null)
            {
                canvasGroup.alpha = 1f - timePassed; // Giảm dần độ mờ
            }
            yield return null;
        }
        Destroy(popup);
    }
}
