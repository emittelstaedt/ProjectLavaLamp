using UnityEngine;
using UnityEngine.InputSystem;

public class EscapeToLeave : MonoBehaviour
{
    private InputAction pauseAction;
    [SerializeField] VoidEventChannelSO stopInteract;

    void Awake()
    {
        pauseAction = InputSystem.actions.FindAction("Pause");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (pauseAction != null)
            {
                if (!pauseAction.enabled)
                {
                    stopInteract.RaiseEvent();
                    //Debug.LogWarning("Escape called stopinteract!s");
                }
                else
                {
                    //Debug.LogWarning("The pause action is enabled.");
                }
            }
            else
            {
                Debug.LogError("Could not find an action named 'Pause' in the project!");
            }
        }
    }
}
