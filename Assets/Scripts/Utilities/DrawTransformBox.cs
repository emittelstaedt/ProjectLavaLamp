using UnityEngine;

public class DrawTransformGizmo : MonoBehaviour
{
    [SerializeField] private Color gizmoColor = Color.green;

    private void OnDrawGizmos()
    {
        if (enabled)
        {
            Gizmos.color = gizmoColor;

            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
        }
    }
}
