using UnityEngine;

public class ObjectRelocator : MonoBehaviour
{
	private Transform objectTransform;
	private Rigidbody objectRigidbody;
	private Collider[] itemColliders; 
	private bool isOutofBounds;
	
    private void Awake()
	{
		isOutofBounds = false;
		itemColliders = GetComponentsInChildren<Collider>();
		objectTransform = GetComponent<Transform>();
		objectRigidbody = GetComponent<Rigidbody>();
	}
	private void Update()
    {
		if(objectTransform.position.y < -0.5f)
		{
			isOutofBounds = true;
			Debug.Log("Object relocated");
			teleportObject();
		}
		if(objectTransform.position.y >= 0.35 && isOutofBounds == true)
		{
			objectRigidbody.linearVelocity = Vector3.zero;
			objectRigidbody.angularVelocity = Vector3.zero;
			foreach(Collider col in itemColliders)
			{
				col.enabled = true;
			}
			objectRigidbody.useGravity = true;
			isOutofBounds = false;
		}
    }
	
	private void teleportObject()
	{
		foreach(Collider col in itemColliders)
		{
			if(col != null){
				col.enabled = false;
			}
		}
		objectRigidbody.useGravity = false;
		objectRigidbody.linearVelocity = Vector3.zero;
		objectRigidbody.angularVelocity = Vector3.zero;
		Vector3 currentPosition = objectTransform.position;
		Vector3 trajectoryVector = new Vector3(0f, 6f, -27f) - currentPosition;
		//objectTransform.position = new Vector3(0f, 3f, -27f);
		objectRigidbody.linearVelocity = trajectoryVector;
		
		
	}
}
