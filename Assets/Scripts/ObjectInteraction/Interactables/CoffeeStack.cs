using UnityEngine;
using UnityEngine.SceneManagement;

public class CoffeeStack : MonoBehaviour, IUsableInteractable
{
	[SerializeField] private VoidEventChannelSO stopInteraction;
	
	[SerializeField] private GameObject emptyCupPrefab;
	[SerializeField] private InteractableSettingsSO settings;
	[SerializeField] private Vector3 cupLocation;
	private GameObject currentItemHeld;
	private Outline outline;
	
	private void Awake()
	{
		if (!TryGetComponent<Outline>(out outline))
        {
            outline = gameObject.AddComponent<Outline>();
        }
        outline.enabled = false;
        outline.OutlineWidth = settings.OutlineWidth;
        outline.OutlineMode = Outline.Mode.OutlineVisible;
	}
	
	public float GetInteractDistance()
    {
        return settings.InteractionDistance + 3;
    }

    public Vector3 GetPosition()
    {
        return transform.position;
    }
    
	public bool CanInteract()
    {
        if (currentItemHeld == null)
        {
            return true;
        }

        return false;
    }
	
	public void StartInteract()
    {
        stopInteraction.RaiseEvent();
		GameObject emptyCup = Instantiate(emptyCupPrefab, cupLocation, Quaternion.identity);
		emptyCup.name = emptyCupPrefab.name;
		SceneManager.MoveGameObjectToScene(emptyCup, SceneManager.GetSceneByName("OfficeWorkplace"));
    }
	
	public void StopInteract()
    {
    }
	
	    public void StartHover()
    {
        outline.OutlineColor = settings.HoverColor;
        outline.enabled = true;
    }

    public void StopHover()
    {
        outline.enabled = false;
    }
    
	public void SetCurrentItemHeld(GameObject newItemHeld)
    {
        currentItemHeld = newItemHeld;
    }
}
