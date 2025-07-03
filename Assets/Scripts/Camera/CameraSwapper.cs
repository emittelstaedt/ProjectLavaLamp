using UnityEngine;
using System;
using System.Collections;
using UnityEngine.InputSystem;

public class CameraSwapper : MonoBehaviour
{
    [SerializeField] private Camera tempCamera;
    [SerializeField] private float transitionTime = 0.05f;
    private Vector3 positionVelocity;
    private Quaternion rotationVelocity;
    
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
    
    // The action allows the calling function to run code after the transition is complete.
    public void SwapCameras(Camera cameraFrom, Camera cameraTo, Action action)
    {
        StartCoroutine(Transition(cameraFrom, cameraTo, action));
    }
    
    public IEnumerator Transition(Camera cameraFrom, Camera cameraTo, Action action)
    {
        Transform from = cameraFrom.gameObject.transform;
        Transform to = cameraTo.gameObject.transform;

        transform.position = from.position;
        Vector3 startPosition = from.position;
        Vector3 endPosition = to.position;

        Quaternion startRotation = from.rotation;
        Quaternion endRotation = to.rotation;
        float timer = 0f;
        
        cameraFrom.enabled = false;
        tempCamera.enabled = true;
        
        while((transform.position - to.position).magnitude > 0.05f)
        {
            transform.position = Vector3.Slerp(startPosition, to.position, timer / transitionTime);
            transform.rotation = Quaternion.Slerp(startRotation, to.rotation, timer / transitionTime);
            timer += Time.deltaTime;
            yield return null;
        }
        
        tempCamera.enabled = false;
        cameraTo.enabled = true;
        
        action();
    }
}
