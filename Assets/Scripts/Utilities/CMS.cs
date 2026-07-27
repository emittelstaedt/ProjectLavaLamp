using UnityEngine;
using UnityEngine.Events;

public class CMS : MonoBehaviour
{
	public Material disappear; //Add this in editor different for each alt part
	public VoidEventChannelSO itemPlaced;
	public VoidEventChannelSO CMSPlaced;
	
	public void spreadCMS()
	{
		if(transform.root.GetComponent<CMS>() == null)
		{
			CMSpayload(transform.root.gameObject);
		}
		if(transform.parent == null) //if this is the root object
		{
			foreach(Transform child in this.gameObject.transform.root)
			{
				if(child.gameObject.GetComponent<CMS>() == null)
				{
					string childName = child.name;
					int nameLength = childName.Length;
					if(nameLength >= 8)
					{
						if(childName[nameLength - 8] == 'C')
						{
							CMSpayload(child.gameObject);
						}
					}
				}
			}
		}
		CMSPlaced.RaiseEvent();
	}
	
	private void CMSpayload(GameObject CMSmarked)
	{
		CMS newCMS = CMSmarked.AddComponent<CMS>();
		newCMS.disappear = disappear;
		newCMS.itemPlaced = itemPlaced;
		newCMS.CMSPlaced = CMSPlaced;
		CMSmarked.GetComponent<Renderer>().material = disappear;
		VoidEventChannelSubscriber cmsPlaced = CMSmarked.AddComponent<VoidEventChannelSubscriber>();
		UnityEvent cmsResponse = new();
		cmsResponse.AddListener(newCMS.spreadCMS);
		cmsPlaced.SetChannelAndResponse(itemPlaced, cmsResponse);
		UnityEditor.Events.UnityEventTools.AddPersistentListener
		(
			cmsResponse, 
			newCMS.spreadCMS
		);
		cmsPlaced.OnEnable();
		newCMS.spreadCMS();
	}
}
