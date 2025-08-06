using System;
using System.Collections.Generic;
using UnityEngine;

public class PlacementTrigger : MonoBehaviour, IInteractable
{
    [SerializeField] private VoidEventChannelSO stopInteraction;
    [SerializeField] private VoidEventChannelSO itemPlaced;
    [SerializeField] private VoidEventChannelSO clearCrosshair;
    [SerializeField] private VoidEventChannelSO thumbsUpCrosshair;
    [SerializeField] private string requiredItem;
    [SerializeField] private Transform placementContainer;
    [SerializeField] private InteractableSettingsSO Settings;
    [Tooltip("Makes the entire object being placed fit into the placement container.")]
    [SerializeField] private bool scaleEntireObject; 
    [SerializeField] private bool isUpstreamPlacement;
    private LayerMask ignoreCollisionLayer;
    private Transform placementNode;
    private GameObject currentItemHeld;
    private GameObject lastItemheld;
    private readonly Collider[] potentialHits = new Collider[10];

    public bool IsUpstreamPlacement
    {
        get => isUpstreamPlacement; 
        set => isUpstreamPlacement = value;
    }

    private void Awake()
    {
        ignoreCollisionLayer = ~(1 << LayerMask.NameToLayer("IgnoreItemCollision"));

        placementNode = transform.parent;

        transform.parent.name = requiredItem + "PlacementNode";
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
            return requiredItem.Equals(currentItemHeld.name, StringComparison.OrdinalIgnoreCase);
        }

        return false;
    }

    public void StartInteract()
    {
        Transform baseItem = lastItemheld.transform.GetChild(0);
        Vector3 newPosition = placementContainer.position;
        Vector3 containerScale = placementContainer.lossyScale;
        
        if (scaleEntireObject)
        {
            MeshRenderer itemRenderer = lastItemheld.GetComponent<MeshRenderer>();
            lastItemheld.transform.rotation = placementContainer.rotation;

            Vector3 itemSize = itemRenderer.bounds.size;
            Vector3 scaleRatio = DivideVector3(containerScale, itemSize);

            float minimumScale = Mathf.Min(scaleRatio.x, scaleRatio.y, scaleRatio.z);
            lastItemheld.transform.localScale *= minimumScale;

            newPosition += lastItemheld.transform.position - itemRenderer.bounds.center;
        }
        else
        {
            // Gets the base child mesh in case objects have been added to parent mesh.
            Vector3 itemSize = GetWorldBoundSize(lastItemheld.transform.GetChild(0).gameObject);
            Vector3 scaleRatio = DivideVector3(containerScale, itemSize);

            lastItemheld.transform.localScale = Vector3.Scale(lastItemheld.transform.localScale, scaleRatio);
        }

        lastItemheld.transform.SetPositionAndRotation(newPosition, placementContainer.rotation);
        lastItemheld.transform.SetParent(placementNode, true);

        // Remove the previous item while keeping its children, i.e. its collider and placement points.
        while (lastItemheld.transform.childCount > 0)
        {
            Transform child = lastItemheld.transform.GetChild(0);
            child.SetParent(placementNode.parent, true);
        }

        if (isUpstreamPlacement)
        {
            placementNode.parent.name = lastItemheld.name;

            BuildOrderEnforcer lastItemBuildOrder = lastItemheld.GetComponentInParent<BuildOrderEnforcer>();
            BuildOrderEnforcer thisBuildOrder = placementNode.GetComponentInParent<BuildOrderEnforcer>();

            thisBuildOrder.SetBuildOrder
            (
                lastItemBuildOrder.GetBuildOrder().Item1, 
                lastItemBuildOrder.GetBuildOrder().Item2
            );
        }
        else
        {
            baseItem = placementNode.parent.GetChild(0);
        }

        Dictionary<Vector3, Transform> placementMap = new(new Vector3Comparer());

        // Works in reverse order so changing the parent of a duplicate doesn't affect the loop. 
        for (int i = placementNode.parent.childCount - 1; i >= 0; i--)
        {
            Transform child = GetPlacementLocation(placementNode.parent.GetChild(i));

            if (placementMap.ContainsKey(child.position))
            {
                Transform duplicateCandidate = GetPlacementLocation(placementMap[child.position]);

                // Ignore itself and the this placement container since it will be deleted anyways.
                if (child == duplicateCandidate || child == placementContainer || duplicateCandidate == placementContainer)
                {
                    continue;
                }

                Transform order1 = GetPlacementNodeIfMatches(child, duplicateCandidate);
                Transform order2 = GetPlacementNodeIfMatches(duplicateCandidate, child);

                Transform duplicateNode = order1 != null ? order1 : order2;
                if (duplicateNode != null)
                {
                    // Deletes the extra placement node if it exists for the item being placed.
                    duplicateNode.SetParent(null);
                    Destroy(duplicateNode.gameObject);
                    break;
                }
            }
            else
            {
                placementMap.Add(child.position, child);
            }
        }

        // Update local positions so that the base item is at the origin.
        Transform previousBaseItem = placementNode.parent.GetChild(0);
        baseItem.SetAsFirstSibling();

        Vector3 localOffset = -baseItem.localPosition;
        foreach (Transform child in placementNode.parent)
        {
            child.localPosition += localOffset;
        }

        // Update world position so that the object doesn't appear to move.
        Vector3 worldOffset = placementNode.parent.position - previousBaseItem.position;
        placementNode.parent.position += worldOffset;

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
        DestroyImmediate(meshCollider);

        AudioManager.Instance.PlaySound(MixerType.SFX, SoundType.ItemCombine, 0.6f, transform.position);

        // Once an item has been placed the placement point is no longer needed so stop interacting with it.
        stopInteraction.RaiseEvent();
    }

    public void StopInteract()
    {
        // Since destroy happens at end of frame detach it so the renamer script doesn't think placement points remain.
        placementNode.SetParent(null);
        Destroy(placementNode.gameObject);

        itemPlaced.RaiseEvent();
    }

    public void StartHover()
    {
        if (thumbsUpCrosshair != null)
        {
            thumbsUpCrosshair.RaiseEvent();
        }
    }

    public void StopHover()
    {
        if (clearCrosshair != null)
        {
            clearCrosshair.RaiseEvent();
        }
    }

    public void SetRequiredItem(string requiredItem)
    {
        this.requiredItem = requiredItem;
        transform.parent.name = requiredItem + "PlacementNode";
    }

    public void SetCurrentItemHeld(GameObject newItemHeld)
    {
        currentItemHeld = newItemHeld;
        if (currentItemHeld != null)
        {
            lastItemheld = currentItemHeld;
        }
    }

    private Vector3 DivideVector3(Vector3 numerator, Vector3 denominator)
    {
        return new
        (
            numerator.x / denominator.x,
            numerator.y / denominator.y,
            numerator.z / denominator.z
        );
    }
    private Transform GetPlacementLocation(Transform parent)
    {
        if (parent.name.EndsWith("PlacementNode", StringComparison.OrdinalIgnoreCase))
        {
            // Gets the placement container if it's a placement node.
            parent = parent.GetChild(1);
        }

        return parent;
    }

    private Transform GetPlacementNodeIfMatches(Transform possibleNode, Transform possibleCollider)
    {
        string nodeSuffix = "PlacementNode";
        string colliderSuffix = "Collider";

        string nodeName;
        string colliderName = possibleCollider.name;

        if (possibleNode.name.Equals("PlacementContainer", StringComparison.OrdinalIgnoreCase))
        {
            possibleNode = possibleNode.parent;
            nodeName = possibleNode.name;
        }
        else
        {
            return null;
        }

        if (nodeName.EndsWith(nodeSuffix, StringComparison.OrdinalIgnoreCase) && 
            colliderName.EndsWith(colliderSuffix, StringComparison.OrdinalIgnoreCase))
        {
            string base1 = nodeName[..^nodeSuffix.Length];
            string base2 = colliderName[..^colliderSuffix.Length];
            return base1 == base2 ? possibleNode : null;
        }

        return null;
    }

    private Vector3 GetWorldBoundSize(GameObject gameObject)
    {
        Mesh mesh = gameObject.GetComponent<MeshFilter>().sharedMesh;

        Quaternion backup = gameObject.transform.rotation;
        gameObject.transform.rotation = Quaternion.identity;

        Vector3 scaledSize = Vector3.Scale(mesh.bounds.size, gameObject.transform.lossyScale);

        gameObject.transform.rotation = backup;

        return scaledSize;
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