using UnityEngine;
using System.Collections;

public class PistonMover : MonoBehaviour
{
    [SerializeField] private float pushTime = 0.1f;
    [SerializeField] private float pushForce = 1f;
    [SerializeField] private float pushDistance = 1.75f;
    private new Collider collider;
    private float defaultX;
    private float pushVelocity;
    private bool isPushing;

    void Awake()
    {
        defaultX = transform.localPosition.x;
        collider = GetComponent<BoxCollider>();
    }

    public void StartPush()
    {
        StopAllCoroutines();
        StartCoroutine(Push());
    }

    void OnCollisionStay(Collision collision)
    {
        Rigidbody rigidbody = collision.gameObject.GetComponent<Rigidbody>();
        if (rigidbody && isPushing)
        {
            Transform collisionTransform = collision.gameObject.transform;

            // Prevents the pusher from clipping through the object.
            bool doesPenetrate = Physics.ComputePenetration
            (
                collider, transform.position, transform.rotation,
                collision.collider, collisionTransform.position, collisionTransform.rotation,
                out Vector3 _, out float moveDistance
            );

            if (doesPenetrate)
            {
                collisionTransform.position += transform.right * moveDistance;
            }

            rigidbody.AddForce(transform.right * pushForce, ForceMode.VelocityChange);
        }
    }

    private IEnumerator Push()
    {
        isPushing = true;
        yield return StartCoroutine(MoveTo(defaultX + pushDistance));

        yield return StartCoroutine(MoveTo(defaultX));
        isPushing = false;
    }

    private IEnumerator MoveTo(float endX)
    {
        while (Mathf.Abs(endX - transform.localPosition.x) > 0.01f)
        {
            Vector3 newPosition = transform.localPosition;
            newPosition.x = Mathf.SmoothDamp(transform.localPosition.x, endX, ref pushVelocity, pushTime);
            transform.localPosition = newPosition;

            yield return null;
        }
    }
}
