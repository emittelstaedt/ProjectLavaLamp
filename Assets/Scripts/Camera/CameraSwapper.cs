using UnityEngine;
using System;
using System.Collections;

public class CameraSwapper : MonoBehaviour
{
    [SerializeField] private float transitionTime = 0.05f;
    [SerializeField] private GameObject tempCamera;
    
    public static CameraSwapper Instance = null;

    void Awake()
    {
        // Singleton functionality.
        if (Instance == null)
        {
            Instance = GetComponent<CameraSwapper>();
        }
        else if (Instance != gameObject)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }
    
    // The action delegate code is run when the transition is completed.
    public void SwapCameras(Camera cameraStart, Camera cameraEnd, Action action)
    {
        StartCoroutine(Transition(cameraStart, cameraEnd, action));
    }
    
    public IEnumerator Transition(Camera cameraStart, Camera cameraEnd, Action action)
    {
        Transform start = cameraStart.gameObject.transform;
        Transform end = cameraEnd.gameObject.transform;

        start.GetPositionAndRotation(out Vector3 startPosition, out Quaternion startRotation);
        
        cameraStart.gameObject.SetActive(false);
        tempCamera.SetActive(true);
        
        float timer = 0f;
        while((tempCamera.transform.position - end.position).magnitude > 0.05f)
        {
            Vector3 newPosition = Vector3.Slerp(startPosition, end.position, timer / transitionTime);
            Quaternion newRotation = Quaternion.Slerp(startRotation, end.rotation, timer / transitionTime);
            tempCamera.transform.SetPositionAndRotation(newPosition, newRotation);
            timer += Time.deltaTime;
            yield return null;
        }
        
        tempCamera.SetActive(false);
        cameraEnd.gameObject.SetActive(true);

        action();
    }
}
