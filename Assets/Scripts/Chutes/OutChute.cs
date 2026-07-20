using UnityEngine;
using System.Collections;

public class OutChute : MonoBehaviour
{
    [SerializeField] private InteractableSettingsSO settings;
    [SerializeField] private VoidEventChannelSO outBoxSecured;
    [SerializeField] private string acceptedItemName;
    [SerializeField] private float delay = 0.5f;
    private Material chuteMaterial;
    private Mover doorMover;
    private PistonMover pistonMover;
    private Plane openingPlane;
    private bool isWaitingForItem;

    void Awake()
    {
        chuteMaterial = GetComponent<Renderer>().material;
        chuteMaterial.SetColor("_EmissiveColor", Color.white);
        
        doorMover = GetComponentInChildren<Mover>();
        pistonMover = GetComponentInChildren<PistonMover>();

        BoxCollider collider = GetComponent<BoxCollider>();
        Vector3 planeLocalNormal = Vector3.left * (collider.size.x / 2);
        Vector3 planeWorldPoint = transform.TransformPoint(collider.center - planeLocalNormal);
        openingPlane = new Plane(transform.TransformDirection(planeLocalNormal), planeWorldPoint);
    }

    public void Open()
    {
        isWaitingForItem = true;

        doorMover.Move();
    }

    void OnTriggerStay(Collider collider)
    {
        bool isInChute = false;
        if (collider.transform.parent.TryGetComponent<Renderer>(out Renderer objectRenderer))
        {
            Vector3 objectCenter = objectRenderer.bounds.center;
        
            isInChute = openingPlane.GetSide(objectCenter);
        }

        if (isWaitingForItem && collider.gameObject != pistonMover.gameObject && isInChute)
        {
            isWaitingForItem = false;

            StopAllCoroutines();

            GameObject colliderParent = collider.gameObject.transform.parent.gameObject;
            if (collider.gameObject.name == acceptedItemName || colliderParent.name == acceptedItemName)
            {
                chuteMaterial.SetColor("_EmissiveColor", Color.green);
                doorMover.MoveBack();

                StartCoroutine(AcceptItem());
            }
            else
            {
                StartCoroutine(RejectItem());
            }
        }
    }

    private IEnumerator AcceptItem()
    {
        chuteMaterial.SetColor("_EmissiveColor", Color.green);

        yield return new WaitForSeconds(delay);

        outBoxSecured.RaiseEvent();
    }

    private IEnumerator RejectItem()
    {
        chuteMaterial.SetColor("_EmissiveColor", Color.red);

        pistonMover.StartPush();
        yield return new WaitForSeconds(delay);

        isWaitingForItem = true;
        chuteMaterial.SetColor("_EmissiveColor", Color.white);
    }
}