using UnityEngine;

public class ObjectRelocator : MonoBehaviour
{
	private Transform objectTransform;
	private Rigidbody objectRigidbody;
	
    private void Awake()
	{
		objectTransform = GetComponent<Transform>();
		objectRigidbody = GetComponent<Rigidbody>();
	}
	private void Update()
    {
		if(objectTransform.position.y < -5f)
		{
			Debug.Log("Object relocated");
			teleportObject();
		}
    }
	
	private void teleportObject()
	{
		objectRigidbody.useGravity = false;
		objectRigidbody.linearVelocity = Vector3.zero;
		objectRigidbody.angularVelocity = Vector3.zero;
		objectTransform.position = new Vector3(0f, 3f, -27f);
		objectRigidbody.useGravity = true;
	}
}
