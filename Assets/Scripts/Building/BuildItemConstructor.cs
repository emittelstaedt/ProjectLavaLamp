#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.Events;
using UnityEditor;
using System.IO;

public class BuildItemConstructor : MonoBehaviour
{
    [SerializeField] private GameObject placementNode;
    [SerializeField] private VoidEventChannelSO dropItem;
    [SerializeField] private GameObjectEventChannelSO heldItemChanged;
    [SerializeField] private InteractableSettingsSO Settings;
    [SerializeField] private VoidEventChannelSO itemPlaced;
    [SerializeField] private VoidEventChannelSO defaultCrosshair;
    [SerializeField] private VoidEventChannelSO openHandCrosshair;
    [SerializeField] private VoidEventChannelSO closedHandCrosshair;
    [SerializeField] private StringEventChannelSO itemNameHover;
    [SerializeField] private string saveLocation = "Assets/Prefabs/";
    private Vector3 scale;

    [ContextMenu("Construct Build Items")]
    private void ConstructBuildItems()
    {
        GameObject root = transform.GetChild(0).gameObject;

        scale = root.transform.localScale;
        root.transform.localScale = Vector3.one;

        CreateBuildItem(root);
    }

    private void CreateBuildItem(GameObject parent)
    {
        int childCount = parent.transform.childCount;
        for (int i = 0; i < childCount; i++)
        {
            GameObject child = parent.transform.GetChild(0).gameObject;

            CreateBuildItem(child);

            GameObject childNode = Instantiate(placementNode, parent.transform);
            childNode.name = child.name + "PlacementNode";
            SetPlacementNode(childNode, child);
        }

        GameObject grandparent = parent.transform.parent.gameObject;
        if (grandparent != null && grandparent != this.gameObject)
        {
            GameObject grandparentNode = Instantiate(placementNode, parent.transform);
            grandparentNode.name = grandparent.name + "PlacementNode";
            SetPlacementNode(grandparentNode, grandparent);
        }

        CreatePickupItem(parent);
        parent.transform.SetParent(null);

        string itemSaveLocation = saveLocation + parent.name + ".prefab";
        if (!File.Exists(itemSaveLocation))
        {
            PrefabUtility.SaveAsPrefabAsset(parent, itemSaveLocation);

            using var editingScope = new PrefabUtility.EditPrefabContentsScope(itemSaveLocation);
            var root = editingScope.prefabContentsRoot;

            root.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
            root.transform.localScale = scale;

            // Resize placement containers after object has been scaled.
            SetPlacementContainer[] containers = root.transform.GetComponentsInChildren<SetPlacementContainer>();
            foreach (SetPlacementContainer container in containers)
            {
                container.Reset();
            }
        }
        else
        {
            Debug.Log($"Skipping object {parent.name} as it already exists.");
        }
    }

    private void CreatePickupItem(GameObject parent)
    {
        parent.layer = LayerMask.NameToLayer("IgnoreItemCollision");
        GameObject child = Instantiate(parent, parent.transform);
        child.transform.SetAsFirstSibling();

        while (child.transform.childCount > 0)
        {
            DestroyImmediate(child.transform.GetChild(0).gameObject);
        }
        child.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
        child.name = parent.name + "Collider";

        MeshCollider childCollider = child.AddComponent<MeshCollider>();
        childCollider.convex = true;
        
        parent.GetComponent<MeshRenderer>().materials = new Material[0];

        parent.AddComponent<Rigidbody>();
        PickupItem pickupItem = parent.AddComponent<PickupItem>();
        BuildOrderEnforcer buildItemRenamer = parent.AddComponent<BuildOrderEnforcer>();
        GameObjectEventChannelSubscriber gameObjectSubscriber = parent.AddComponent<GameObjectEventChannelSubscriber>();
        VoidEventChannelSubscriber voidSubscriber = parent.AddComponent<VoidEventChannelSubscriber>();

        pickupItem.SetSettings(dropItem, heldItemChanged, Settings);
        pickupItem.SetCrosshairChannels(defaultCrosshair, openHandCrosshair, closedHandCrosshair, itemNameHover);

        UnityEvent<GameObject> gameObjectResponse = new();
        gameObjectResponse.AddListener(pickupItem.SetCurrentItemHeld);
        gameObjectSubscriber.SetChannelAndResponse(heldItemChanged, gameObjectResponse);

        UnityEvent voidResponse = new();
        voidResponse.AddListener(buildItemRenamer.CheckIfFinishedBuilding);
        voidResponse.AddListener(pickupItem.UpdateChildrenColliders);
        voidSubscriber.SetChannelAndResponse(itemPlaced, voidResponse);

        // Properly serialize the response fields inside the prefab so they show in the inspector.
        UnityEditor.Events.UnityEventTools.AddPersistentListener
        (
            gameObjectResponse, 
            pickupItem.SetCurrentItemHeld
        );
        UnityEditor.Events.UnityEventTools.AddVoidPersistentListener
        (
            voidResponse, 
            buildItemRenamer.CheckIfFinishedBuilding
        );
        UnityEditor.Events.UnityEventTools.AddVoidPersistentListener
        (
            voidResponse, 
            pickupItem.UpdateChildrenColliders
        );
    }

    private void SetPlacementNode(GameObject node, GameObject item)
    {
        for (int i = 0; i < node.transform.childCount; i++)
        {
            Transform child = node.transform.GetChild(i);

            CopyToContainer(child.gameObject, item);
        }
    }

    public void CopyToContainer(GameObject container, GameObject item)
    {
        item.transform.GetPositionAndRotation(out Vector3 position, out Quaternion rotation);
        Vector3 scale = GetBoundsScale(item);

        container.transform.SetPositionAndRotation(position, rotation);
        container.transform.localScale = scale;

        if (container.transform.TryGetComponent<PlacementTrigger>(out PlacementTrigger placementTrigger))
        {
            placementTrigger.SetRequiredItem(item.name);
        }
    }

    private Vector3 GetBoundsScale(GameObject item)
    {
        Mesh mesh = item.GetComponent<MeshFilter>().sharedMesh;
        Quaternion backup = item.transform.rotation;

        item.transform.rotation = Quaternion.identity;
        Vector3 scale = Vector3.Scale(mesh.bounds.size, item.transform.lossyScale);
        item.transform.rotation = backup;

        return scale;
    }
}
#endif