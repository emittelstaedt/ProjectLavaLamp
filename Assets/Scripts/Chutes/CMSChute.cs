using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class CMSChute : MonoBehaviour
{
	[SerializeField] private GameObject packagedBox;
	[SerializeField] private BoxItemsSO CMSItem;
	private Vector3 itemSpawnPosition;
	private PistonMover ventMover;
	public void Awake()
	{
		CMSItem = null;
		ventMover = GetComponentInChildren<PistonMover>();
		itemSpawnPosition = new Vector3(-6.5f, 0.75f, -3f);
		StartCoroutine(CMSDeposit());
	}
	
	public void AddBox(BoxItemsSO items)
    {
        CMSItem = items;
    }

    private void GiveBox()
    {
		if(CMSItem != null)
		{
			GameObject newBox = Instantiate(packagedBox, itemSpawnPosition, Quaternion.identity);
			// Removes "(Clone)" from the name.
			newBox.name = packagedBox.name;
			newBox.GetComponent<BoxExploder>().BoxItems = CMSItem;
			SceneManager.MoveGameObjectToScene(newBox, SceneManager.GetSceneByName("OfficeWorkplace"));
		}
        //StartCoroutine(GiveItemAnimation());
    }
	
	private IEnumerator CMSDeposit()
	{
		yield return new WaitForSeconds(0.5f);
		GiveBox();
	}
}
