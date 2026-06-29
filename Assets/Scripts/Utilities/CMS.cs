using UnityEngine;

public class CMS : MonoBehaviour
{
    void Start()
    {
        
    }

    void Update()
    {
        if(this.gameObject.transform.root != this.gameObject.transform && this.gameObject.transform.root.GetComponent<CMS>() == null)
		{
			Debug.Log("CMS viral tag");
			this.gameObject.transform.root.gameObject.AddComponent<CMS>();
			foreach(Transform child in this.gameObject.transform.root)
			{
				string childName = child.name;
				int nameLength = childName.Length;
				if(nameLength >= 8)
				{
					if(childName[nameLength - 8] == 'C')
					{
						child.gameObject.AddComponent<CMS>();
					}
				}
			}
			Destroy(this);
		}
    }
}
