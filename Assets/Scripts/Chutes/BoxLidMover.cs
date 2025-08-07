using UnityEngine;
using System.Collections;

public class BoxLidMover : MonoBehaviour
{
    [SerializeField] private Collider lid1Collider;
    [SerializeField] private Collider lid2Collider;
    Animator animator;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void Open()
    {
        // Colliders must be disabled at the start so the pusher doesn't push it super hard.
        lid1Collider.enabled = true;
        lid2Collider.enabled = true;
        animator.SetTrigger("Open");
    }

    public void Close()
    {
        animator.SetTrigger("Close");
    }
}