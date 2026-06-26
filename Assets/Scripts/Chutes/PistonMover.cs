using UnityEngine;
using System.Collections;

public class PistonMover : MonoBehaviour
{
    [SerializeField] private float pushTime = 0.1f;
    [SerializeField] private float pushForce = 1f;
    [SerializeField] private float pushDistance = 1.75f;
	[SerializeField] private int dimensionSwitch = 1; //1 x, 2 y, 3 z
    private Collider pistonCollider;
    private float defaultPos;
	private float backUpPos;
    private float pushVelocity;
    private bool isPushing;

    void Awake()
    {
		if(dimensionSwitch == 1)
		{
			defaultPos = transform.localPosition.x;
		}
		else if(dimensionSwitch == 2)
		{
			defaultPos = transform.localPosition.y;
		}
		else if(dimensionSwitch == 3)
		{
			defaultPos = transform.localPosition.z;
		}
		else
		{
			Debug.Log("Error choosing pushDimension");
		}
        backUpPos = defaultPos;
        pistonCollider = GetComponent<BoxCollider>();
    }

    public void StartPush()
    {
        StopAllCoroutines();
        StartCoroutine(Push());
    }

    void OnCollisionStay(Collision collision)
    {
        Rigidbody rigidbody = collision.gameObject.GetComponent<Rigidbody>();
        if (rigidbody != null && isPushing)
        {
            Transform collisionTransform = collision.gameObject.transform;

            // Prevents the pusher from clipping through the object.
            bool doesPenetrate = Physics.ComputePenetration
            (
                pistonCollider, transform.position, transform.rotation,
                collision.collider, collisionTransform.position, collisionTransform.rotation,
                out Vector3 _, out float moveDistance
            );
			if(dimensionSwitch == 1)
			{
				if (doesPenetrate)
				{
					collisionTransform.position += transform.right * moveDistance;
				}
				rigidbody.AddForce(transform.right * pushForce, ForceMode.VelocityChange);
			}
			else if(dimensionSwitch == 2)
			{
				if (doesPenetrate)
				{
					collisionTransform.position += transform.up * moveDistance;
				}
				rigidbody.AddForce(transform.up * pushForce, ForceMode.VelocityChange);
			}
			else if(dimensionSwitch == 3)
			{
				if (doesPenetrate)
				{
					collisionTransform.position += transform.forward * moveDistance;
				}
				rigidbody.AddForce(transform.forward * pushForce, ForceMode.VelocityChange);
			}
			else
			{
				Debug.Log("Error choosing pushDimension");
			}
            

            
        }
    }

    private IEnumerator Push()
    {
        isPushing = true;
        yield return StartCoroutine(MoveTo(defaultPos + pushDistance));

        yield return StartCoroutine(MoveTo(defaultPos));
        isPushing = false;
    }

    private IEnumerator MoveTo(float end)
    {
        while (Mathf.Abs(end - backUpPos) > 0.01f)
        {
            Vector3 newPosition = transform.localPosition;
			if(dimensionSwitch == 1)
			{
				backUpPos = transform.localPosition.x;
				newPosition.x = Mathf.SmoothDamp(backUpPos, end, ref pushVelocity, pushTime);
			}
			else if(dimensionSwitch == 2)
			{
				backUpPos = transform.localPosition.y;
				newPosition.y = Mathf.SmoothDamp(backUpPos, end, ref pushVelocity, pushTime);
			}
			else if(dimensionSwitch == 3)
			{
				backUpPos = transform.localPosition.z;
				newPosition.z = Mathf.SmoothDamp(backUpPos, end, ref pushVelocity, pushTime);
			}
			else
			{
				Debug.Log("Error choosing pushDimension");
			}
            transform.localPosition = newPosition;
            yield return null;
        }
    }
}