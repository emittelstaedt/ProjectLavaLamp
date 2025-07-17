using UnityEngine;

public class BuildItemRenamer : MonoBehaviour
{
    [Tooltip("Update to this when there are no placement nodes remaining. Blank keeps current name.")]
    [SerializeField] private string finishedName;

    public void CheckIfFinishedBuilding()
    {
        PlacementTrigger[] placementPoints = GetComponentsInChildren<PlacementTrigger>();
        
        if (placementPoints.Length == 0)
        {
            UpdateName(finishedName);
        }
    }

    private void UpdateName(string newName)
    {
        if (!string.IsNullOrEmpty(newName))
        {
            gameObject.name = newName;
        }
    }
}