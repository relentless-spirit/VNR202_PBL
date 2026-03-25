using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 5f;
    public Rigidbody2D rb;
    public Animator animator;

    private Vector2 movement;

    // 🔥 hướng cuối cùng
    private float lastInputX;
    private float lastInputY;

    void Start()
    {
        // 👉 mặc định đứng quay xuống
        lastInputY = -1f;
    }

    void Update()
    {
        // 🔥 khóa khi pause
        if (PauseController.IsGamePaused)
        {
            movement = Vector2.zero;
            UpdateAnimator();
            return;
        }

        movement = Vector2.zero;

        // 🎮 input (1 hướng duy nhất)
        if (Input.GetKey(KeyCode.W))
        {
            movement.y = 1f;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            movement.y = -1f;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            movement.x = 1f;
        }
        else if (Input.GetKey(KeyCode.A))
        {
            movement.x = -1f;
        }

        // 🔥 lưu hướng cuối
        if (movement != Vector2.zero)
        {
            lastInputX = movement.x;
            lastInputY = movement.y;
        }

        UpdateAnimator();
    }

    void FixedUpdate()
    {
        rb.MovePosition(rb.position + movement * (speed * Time.fixedDeltaTime));
    }

    // 🎬 cập nhật animation
    void UpdateAnimator()
    {
        animator.SetFloat("Horizontal", movement.x);
        animator.SetFloat("Vertical", movement.y);
        animator.SetFloat("Speed", movement.sqrMagnitude);

        animator.SetFloat("LastInputX", lastInputX);
        animator.SetFloat("LastInputY", lastInputY);
    }

    // 🔥 OPTIONAL: khóa di chuyển khi bị bắt
    public void DisableMovement(float time)
    {
        StartCoroutine(DisableMoveCoroutine(time));
    }

    IEnumerator DisableMoveCoroutine(float time)
    {
        enabled = false;
        yield return new WaitForSeconds(time);
        enabled = true;
    }
}