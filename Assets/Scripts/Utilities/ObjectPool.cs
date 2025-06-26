using UnityEngine;
using System.Collections.Generic;

public class ObjectPool
{
    private GameObject prefab;
    private Transform parent;
    private List<GameObject> objects = new List<GameObject>();
    
    // Sets the prefab to be used.
    public ObjectPool(GameObject prefab, Transform parent)
    {
        this.prefab = prefab;
        this.parent = parent;
    }
    
    // Returns an instance of the prefab from the pool.
    // Objects must be SetActive(false) to give the object back to the pool.
    public GameObject GetInstance()
    {
        // Ensures the prefab has been set.
        if (prefab == null)
        {
            Debug.LogError("No prefab set in object pool");
            return null;
        }
        
        for (int i = 0; i < objects.Count; i++)
        {
            GameObject obj = objects[i];
        
            // Remove an object from the list if it gets destroyed.
            if (obj == null)
            {
                objects.RemoveAt(i);
            }
            // Ensure that the object has the correct parent.
            else if (!obj.activeSelf && obj.transform.parent != parent)
            {
                obj.transform.SetParent(parent);
            }
            // Return the first inactive object.
            if (obj != null && !obj.activeSelf)
            {
                obj.SetActive(true);
                return obj;
            }
        }
        
        // Adds a new object to the pool if there is no inactive one.
        objects.Add(GameObject.Instantiate(prefab, parent));
        return objects[objects.Count - 1];
    }
}
