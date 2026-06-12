using UnityEngine;
    
public class CounteractDrillSpeed: MonoBehaviour
{

[SerializeField] private float speed;
[SerializeField] public Vector3 mapStartPosition;

    void Awake(){
       // Debug.Log($"Awake triggered on {gameObject.name}", gameObject);
       mapStartPosition = transform.position;
    }

    void Update()
    {
        transform.Translate(Vector3.left * speed * Time.deltaTime);
    }

    void OnDisable()
    {
        transform.position=mapStartPosition;
    }
}
