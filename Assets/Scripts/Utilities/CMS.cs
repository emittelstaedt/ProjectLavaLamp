using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

public class CMS : MonoBehaviour
{
	public List<Material> disappear; //Add this in editor different for each alt part
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
					if(nameLength >= 8 || childName == "Lid1" || childName == "Lid2")
					{
						if(childName == "Lid1" || childName == "Lid2")
						{
							CMSpayload(child.gameObject);
							continue;
						}
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
		if(CMSmarked.name == "OutBoxCollider" || CMSmarked.name ==  "Lid1" || CMSmarked.name ==  "Lid2")
		{
			Shader fadeShader = Shader.Find("Shader Graphs/BoxFade");
			CMSmarked.GetComponent<Renderer>().material.shader = fadeShader;
		}
		else if(CMSmarked.name == "EmptyOutBox" || CMSmarked.name ==  "FullOutBox")
		{
			//Nothing
		}
		else
		{
			if(CMSmarked.GetComponent<Renderer>().materials.Length > 1)
			{
				CMSmarked.GetComponent<Renderer>().SetMaterials(disappear);
			}
			else if(CMSmarked.GetComponent<Renderer>().materials.Length == 1)
			{
				CMSmarked.GetComponent<Renderer>().material = disappear[0];
			}
			
		}
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
