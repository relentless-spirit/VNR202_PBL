using UnityEngine;

public class InteractionDetector : MonoBehaviour
{
    private IInteractable _interactableInRange; //Closest interactable
    public GameObject interactionIcon;
    [SerializeField] private KeyCode interactKey = KeyCode.E;

    private float _horizontal;
    private float _vertical;
    public float Horizontal => _horizontal;
    public float Vertical => _vertical;


    void Start()
    {
        if (interactionIcon != null) interactionIcon.SetActive(false);
    }

    private void Update()
    {
        UpdateHorizontalAxis();
        UpdateVerticalAxis();

        if (_interactableInRange != null && Input.GetKeyDown(interactKey))
        {
            _interactableInRange.Interact();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out IInteractable interactable) && interactable.CanInteract())
        {
            _interactableInRange = interactable;
            if (interactionIcon != null) interactionIcon.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.TryGetComponent(out IInteractable interactable) && interactable == _interactableInRange)
        {
            _interactableInRange = null;
            if (interactionIcon != null) interactionIcon.SetActive(false);
        }
    }

    // NOTE about mapping:
    // The user asked for the following mapping (non-standard names but implemented as requested):
    // W -> horizontal = 1
    // S -> horizontal = -1
    // D -> vertical = 1
    // A -> vertical = -1
    // Also: only one axis should be affected at a time. Pressing W/S will zero vertical; pressing A/D will zero horizontal.

    private void UpdateHorizontalAxis()
    {
        // Pressing W/S sets horizontal and clears vertical so only one axis is active at a time
        if (Input.GetKeyDown(KeyCode.W))
        {
            _horizontal = 1f;
            _vertical = 0f; // ensure vertical is cleared
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            _horizontal = -1f;
            _vertical = 0f; // ensure vertical is cleared
        }

        // Release keys -> reset horizontal when corresponding key released
        if (Input.GetKeyUp(KeyCode.W) && _horizontal > 0f)
        {
            _horizontal = 0f;
        }
        else if (Input.GetKeyUp(KeyCode.S) && _horizontal < 0f)
        {
            _horizontal = 0f;
        }
    }

    private void UpdateVerticalAxis()
    {
        // Pressing D/A sets vertical and clears horizontal so only one axis is active at a time
        if (Input.GetKeyDown(KeyCode.D))
        {
            _vertical = 1f;
            _horizontal = 0f; // ensure horizontal is cleared
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            _vertical = -1f;
            _horizontal = 0f; // ensure horizontal is cleared
        }

        // Release keys -> reset vertical when corresponding key released
        if (Input.GetKeyUp(KeyCode.D) && _vertical > 0f)
        {
            _vertical = 0f;
        }
        else if (Input.GetKeyUp(KeyCode.A) && _vertical < 0f)
        {
            _vertical = 0f;
        }
    }
}
