using UnityEngine;

public class SirenStateSwitcher : MonoBehaviour
{
    private Animator animator;

    [Tooltip("Toggles the siren state (Spin if true, Inactive if false)")]
    public bool isActive;

    private void Start()
    {
        animator = GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogError("Animator component not found on " + gameObject.name);
        }
    }

    private void Update()
    {
        if (animator != null)
        {
            animator.SetBool("Active", isActive);
        }
    }
}
