using UnityEngine;
using System.Collections;

public class OutChute : MonoBehaviour
{
    [SerializeField] private InteractableSettingsSO settings;
    [SerializeField] private VoidEventChannelSO winLevel;
    [SerializeField] private string acceptedItemName;
    [SerializeField] private float delay = 0.5f;
    private Material material;
    private Mover doorMover;
    private PistonMover pistonMover;
    private Plane openingPlane;
    private bool isWaitingForItem;

    void Awake()
    {
        material = GetComponent<Renderer>().material;
        material.color = Color.white;
        
        doorMover = GetComponentInChildren<Mover>();
        pistonMover = GetComponentInChildren<PistonMover>();

        BoxCollider collider = GetComponent<BoxCollider>();
        Vector3 planeNormal = Vector3.right * (collider.size.x / 2);
        Vector3 planePoint = collider.center + planeNormal;
        openingPlane = new Plane(transform.TransformDirection(planeNormal), transform.TransformPoint(planePoint)).flipped;
        Debug.DrawRay(transform.TransformPoint(planePoint), transform.TransformDirection(planeNormal) * 5f, Color.red, 100f);
    }

    public void Open()
    {
        isWaitingForItem = true;

        doorMover.Move();
    }

    void OnTriggerStay(Collider collider)
    {
        bool isInChute = openingPlane.GetSide(collider.transform.position);
        if (isWaitingForItem && collider.gameObject != pistonMover.gameObject && isInChute)
        {
            StopAllCoroutines();
            isWaitingForItem = false;
            GameObject colliderParent = collider.gameObject.transform.parent.gameObject;
            if (collider.gameObject.name == acceptedItemName || colliderParent.name == acceptedItemName)
            {
                material.color = Color.green;
                doorMover.MoveBack();

                StartCoroutine(TakeItem(collider.gameObject));
            }
            else
            {
                StartCoroutine(RejectItem());
            }
        }
    }

    private IEnumerator TakeItem(GameObject item)
    {
        material.color = Color.green;

        yield return new WaitForSeconds(delay);

        winLevel.RaiseEvent();
    }

    private IEnumerator RejectItem()
    {
        material.color = Color.red;

        pistonMover.StartPush();
        yield return new WaitForSeconds(delay);

        isWaitingForItem = true;
        material.color = Color.white;
    }
}