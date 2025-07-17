using UnityEngine;

public class SetPlacementContainer : MonoBehaviour
{
    [SerializeField] private Color gizmoColor = Color.red;
    [Tooltip("The scale of the item being placed here.")]
    [SerializeField] private Vector3 expectedScale = Vector3.one;

    private void Awake()
    {
        // Sets the local scale to match the expected scale. Does some matrix math to account for rotation.
        Matrix4x4 parentWorldMatrix = transform.parent.localToWorldMatrix;

        Matrix4x4 desiredWorldMatrix = Matrix4x4.TRS(transform.position, Quaternion.identity, expectedScale);

        Matrix4x4 desiredLocalMatrix = parentWorldMatrix.inverse * desiredWorldMatrix;

        Vector3 x = new(desiredLocalMatrix.m00, desiredLocalMatrix.m10, desiredLocalMatrix.m20);
        Vector3 y = new(desiredLocalMatrix.m01, desiredLocalMatrix.m11, desiredLocalMatrix.m21);
        Vector3 z = new(desiredLocalMatrix.m02, desiredLocalMatrix.m12, desiredLocalMatrix.m22);

        transform.localScale = new(x.magnitude, y.magnitude, z.magnitude);
    }

    private void OnDrawGizmos()
    {
        if (enabled)
        {
            Gizmos.color = gizmoColor;

            Matrix4x4 desiredLocalMatrix = Matrix4x4.TRS(transform.position, transform.rotation, expectedScale);

            Gizmos.matrix = desiredLocalMatrix;
            Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
        }
    }
}