using System;
using System.Collections;
using UnityEngine;

public class WaypointMover : MonoBehaviour
{
    [Header("Waypoints")]
    public Transform waypointParent;

    [Header("Movement")]
    public float moveSpeed = 2f;
    public float waitTime = 1.5f;
    public bool loopWaypoints = true;

    private Transform[] waypoints;
    private int currentWaypointIndex = 0;

    private bool isWaiting = false;
    private Animator animator;

    private Vector2 moveDir;

    void Start()
    {
        animator = GetComponent<Animator>();

        waypoints = new Transform[waypointParent.childCount];
        for (int i = 0; i < waypointParent.childCount; i++)
        {
            waypoints[i] = waypointParent.GetChild(i);
        }
    }

    void Update()
    {
        if (PauseController.IsGamePaused || isWaiting)
        {
            UpdateAnimator(Vector2.zero);
            return;
        }

        MoveToWaypoint();
    }

    void MoveToWaypoint()
    {
        Transform target = waypoints[currentWaypointIndex];

        Vector2 rawDir = target.position - transform.position;

        // 🔥 ép về 4 hướng (fix lỗi luôn đi trái)
        if (Mathf.Abs(rawDir.x) > Mathf.Abs(rawDir.y))
        {
            moveDir = new Vector2(Mathf.Sign(rawDir.x), 0);
        }
        else
        {
            moveDir = new Vector2(0, Mathf.Sign(rawDir.y));
        }

        // 🎬 animation
        UpdateAnimator(moveDir);

        // 🚶 di chuyển
        transform.position = Vector2.MoveTowards(
            transform.position,
            target.position,
            moveSpeed * Time.deltaTime
        );

        // 📍 tới waypoint
        if (Vector2.Distance(transform.position, target.position) < 0.05f)
        {
            StartCoroutine(WaitAtWaypoint());
        }
    }

    IEnumerator WaitAtWaypoint()
    {
        isWaiting = true;

        // đứng im nhưng vẫn giữ hướng
        UpdateAnimator(Vector2.zero);

        yield return new WaitForSeconds(waitTime);

        if (loopWaypoints)
        {
            currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
        }
        else
        {
            currentWaypointIndex = Math.Min(currentWaypointIndex + 1, waypoints.Length - 1);
        }

        isWaiting = false;
    }

    // 🎯 CORE ANIMATOR (GIỐNG PLAYER)
    void UpdateAnimator(Vector2 direction)
    {
        // speed
        animator.SetFloat("Speed", direction.sqrMagnitude);

        // hướng di chuyển
        animator.SetFloat("Horizontal", direction.x);
        animator.SetFloat("Vertical", direction.y);

        // lưu hướng cuối (để đứng đúng)
        if (direction != Vector2.zero)
        {
            animator.SetFloat("LastInputX", direction.x);
            animator.SetFloat("LastInputY", direction.y);
        }
    }
}