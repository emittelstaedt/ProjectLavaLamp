using UnityEngine;

public class PlacementTrigger : MonoBehaviour, IInteractable
{
    [SerializeField] private VoidEventChannelSO stopInteraction;
    [SerializeField] private VoidEventChannelSO updateName;
    [SerializeField] private string requiredItem;
    [SerializeField] private Transform placementContainer;
    [SerializeField] private InteractableSettingsSO Settings;
    private LayerMask ignoreCollisionLayer;
    private Transform placementNode;
    private Outline outline;
    private GameObject currentItemHeld;
    private GameObject lastItemheld;
    private readonly Collider[] potentialHits = new Collider[10];

    private void Awake()
    {
        ignoreCollisionLayer = ~(1 << LayerMask.NameToLayer("IgnoreItemCollision"));

        placementNode = transform.parent;

        outline = GetComponent<Outline>();
        if (outline == null)
        {
            outline = gameObject.AddComponent<Outline>();
            outline.enabled = false;
            outline.OutlineWidth = 5;
            outline.OutlineMode = Outline.Mode.OutlineVisible;
        }
    }

    public float GetInteractDistance()
    {
        return Settings.InteractionDistance;
    }

    public Vector3 GetPosition()
    {
        return transform.position;
    }

    public bool CanInteract()
    {
        if (currentItemHeld != null)
        {
            return requiredItem.Equals(currentItemHeld.name);
        }

        return false;
    }

    public void StartInteract()
    {
        lastItemheld.transform.SetPositionAndRotation(placementContainer.position, placementContainer.rotation);
        lastItemheld.transform.localScale = placementContainer.lossyScale;
        lastItemheld.transform.SetParent(placementNode, true);

        // Remove the previous item while keeping its children, i.e. its collider and placement points.
        while (lastItemheld.transform.childCount > 0)
        {
            Transform child = lastItemheld.transform.GetChild(0);
            child.SetParent(placementNode.parent, true);
        }
        Destroy(lastItemheld);

        // Combine meshes so that the outline will show for the entire new combined object.
        MeshFilter parentMeshFilter = placementNode.parent.GetComponent<MeshFilter>();
        MeshFilter itemMeshFilter = lastItemheld.GetComponent<MeshFilter>();

        parentMeshFilter.sharedMesh = AddChildMeshToParent(parentMeshFilter, itemMeshFilter);

        // Move the object if placement container is inside another collider in the direction of the base object.
        MeshCollider meshCollider = lastItemheld.AddComponent<MeshCollider>();
        meshCollider.sharedMesh = itemMeshFilter.sharedMesh;
        meshCollider.convex = true;

        int hitCount = Physics.OverlapSphereNonAlloc
        (
            meshCollider.transform.position,
            meshCollider.bounds.extents.magnitude, 
            potentialHits, 
            ignoreCollisionLayer
        );

        for (int i = 0; i < hitCount; i++)
        {
            Collider potentialHit = potentialHits[i];

            if (potentialHit.isTrigger)
            {
                continue;
            }

            bool doesPenetrate = Physics.ComputePenetration
            (
                meshCollider, meshCollider.transform.position, meshCollider.transform.rotation,
                potentialHit, potentialHit.transform.position, potentialHit.transform.rotation,
                out _, out _
            );

            if (doesPenetrate)
            {
                Renderer lastItemRenderer = lastItemheld.GetComponent<Renderer>();
                Bounds itemBounds = lastItemRenderer.bounds;
                Vector3 baseItemPosition = placementNode.parent.position;

                Vector3 closestPoint = potentialHit.ClosestPointOnBounds(baseItemPosition);
                Vector3 directionToMove = (closestPoint - baseItemPosition).normalized;

                float distanceToBounds = Vector3.Scale(directionToMove, itemBounds.extents).magnitude;
                Vector3 farthestPoint = itemBounds.center + directionToMove * distanceToBounds;

                float distanceToMove = Vector3.Dot(closestPoint - farthestPoint, directionToMove);

                placementNode.parent.position += directionToMove * distanceToMove;
            }
        }

        // Play a locking sound when items are combined.
        AudioManager.Instance.PlaySound(MixerType.SFX, SoundType.ItemCombine, 0.3f, transform.position);

        DestroyImmediate(meshCollider);

        // Once an item has been placed the placement point is no longer needed so stop interacting with it.
        stopInteraction.RaiseEvent();
    }

    public void StopInteract()
    {
        // Since destroy happens at end of frame detach it so the renamer script doesn't think placement points remain.
        placementNode.SetParent(null);
        Destroy(placementNode.gameObject);

        updateName.RaiseEvent();
    }

    public void StartHover()
    {
        outline.OutlineColor = Settings.HoverColor;
        outline.enabled = true;
    }

    public void StopHover()
    {
        outline.enabled = false;
    }

    public void SetCurrentItemHeld(GameObject newItemHeld)
    {
        currentItemHeld = newItemHeld;
        if (currentItemHeld != null)
        {
            lastItemheld = currentItemHeld;
        }
    }

    private Mesh AddChildMeshToParent(MeshFilter parent, MeshFilter child)
    {
        CombineInstance[] combine = new CombineInstance[2];

        combine[0].mesh = parent.sharedMesh;
        combine[0].transform = Matrix4x4.identity;

        Matrix4x4 childToParent = parent.transform.worldToLocalMatrix * child.transform.localToWorldMatrix;
        combine[1].mesh = child.sharedMesh;
        combine[1].transform = childToParent;

        Mesh mesh = new();
        mesh.CombineMeshes(combine);
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();

        return mesh;
    }
}